using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OldMarket.Navigation
{
    /// <summary>Projects a verified loaded collision surface, never an invented marker elevation.</summary>
    public sealed class WorldTargetProjection : IDisposable
    {
        private string targetId = "", scope = "";
        private float x, z, nextProbe;
        private bool hasPoint;
        private Vector3 point;
        private Collider surface;

        public WorldTargetProjection()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }
        private void SceneLoaded(Scene scene, LoadSceneMode mode) => Invalidate();
        private void SceneUnloaded(Scene scene) => Invalidate();
        private void Invalidate() { hasPoint = false; surface = null; targetId = ""; }

        public void Refresh(NavigationState state, Camera camera, float now, bool mapOpen, bool enabled)
        {
            state.WorldTargetVisible = false;
            var target = enabled && state.Available ? state.Target : null;
            if (target == null) { Invalidate(); return; }
            if (target.Id != targetId || state.ScopeId != scope || target.X != x || target.Z != z)
            {
                Invalidate(); targetId = target.Id; scope = state.ScopeId; x = target.X; z = target.Z;
            }
            if (mapOpen || camera == null) return;
            if (now >= nextProbe)
            {
                nextProbe = now + 1f;
                hasPoint = false; surface = null;
                // One bounded read-only query per second, against loaded non-trigger colliders only.
                var origin = new Vector3(x, state.PlayerPosition.y + 1000f, z);
                if (Physics.Raycast(origin, Vector3.down, out var hit, 2000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    surface = hit.collider;
                    point = hit.point + Vector3.up * 1.25f;
                    hasPoint = true;
                }
            }
            if (!hasPoint || surface == null || !surface.enabled || !surface.gameObject.activeInHierarchy) return;
            var viewport = camera.WorldToViewportPoint(point);
            if (viewport.z <= camera.nearClipPlane || viewport.x <= .02f || viewport.x >= .98f || viewport.y <= .02f || viewport.y >= .98f) return;
            var pixels = camera.pixelRect;
            state.WorldTargetScreen = new Vector2(pixels.x + viewport.x * pixels.width, pixels.y + viewport.y * pixels.height);
            state.WorldTargetVisible = true;
        }
        public void Dispose()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnloaded;
            Invalidate();
        }
    }
}
