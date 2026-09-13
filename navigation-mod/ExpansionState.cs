using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OldMarket.Navigation
{
    /// <summary>Read-only unlock snapshot. No scene object scans and no game state writes.</summary>
    public sealed class ExpansionState : IDisposable
    {
        private static readonly FieldInfo ActiveField = typeof(GameManager).GetField("activeExpansions", BindingFlags.Instance | BindingFlags.NonPublic);
        private GameManager manager;
        private NetworkList<long> networkList;
        private readonly List<long> unlocked = new List<long>();
        private bool dirty = true, forceRefresh = true, disposed;
        private float refreshAfter, nextIdentityCheck, nextFallbackCheck;
        public IReadOnlyList<long> UnlockedIds => unlocked;
        public string Fingerprint { get; private set; } = "";
        public int Revision { get; private set; }

        public ExpansionState()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode) => Invalidate();
        private void SceneUnloaded(Scene scene) => Invalidate();
        private void ListChanged(NetworkListEvent<long> change) => Invalidate();
        private void Invalidate()
        {
            dirty = true;
            forceRefresh = true;
            // Let the game's CheckExpansions / scene activation finish before a consumer resamples geometry.
            refreshAfter = Time.unscaledTime + .25f;
        }

        /// <summary>Call from the main thread. True means map layers/cache may need refreshing.</summary>
        public bool Poll(float now)
        {
            if (disposed) return false;
            if (now >= nextIdentityCheck)
            {
                nextIdentityCheck = now + .5f;
                var current = GameManager.Instance;
                if (current != manager)
                {
                    Detach(); manager = current;
                    networkList = current != null ? ActiveField?.GetValue(current) as NetworkList<long> : null;
                    if (networkList != null) networkList.OnListChanged += ListChanged;
                    Invalidate();
                }
            }
            // Public API fallback if the private event source changes in a future game build.
            if (networkList == null && now >= nextFallbackCheck)
            {
                nextFallbackCheck = now + 2f;
                if (!dirty) { dirty = true; refreshAfter = now; }
            }
            if (!dirty || now < refreshAfter) return false;
            dirty = false;
            var values = new List<long>();
            if (manager != null && manager.IsSpawned)
                foreach (var item in manager.GetActiveExpansions())
                    if (item != null && !values.Contains(item.id)) values.Add(item.id);
            values.Sort();
            var parts = new string[values.Count];
            for (int i = 0; i < values.Count; i++) parts[i] = values[i].ToString(CultureInfo.InvariantCulture);
            string fingerprint = string.Join(",", parts);
            bool changed = forceRefresh || fingerprint != Fingerprint;
            forceRefresh = false;
            // Scene transitions also invalidate geometry even when unlocked IDs stay the same.
            unlocked.Clear(); unlocked.AddRange(values); Fingerprint = fingerprint;
            if (changed) Revision++;
            return changed;
        }

        private void Detach()
        {
            if (networkList != null) networkList.OnListChanged -= ListChanged;
            networkList = null;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnloaded;
            Detach(); manager = null;
        }
    }
}
