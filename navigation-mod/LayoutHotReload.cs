using System;
using System.IO;
using UnityEngine;

namespace OldMarket.Navigation
{
    // Poll metadata on the Unity thread: no watcher callback can mutate Unity UI off-thread.
    public sealed class LayoutHotReload
    {
        private readonly string path;
        private readonly Action<string> log;
        private DateTime lastWrite;
        private long lastSize = -1;
        private float nextPoll;
        public NavigationLayout Current { get; private set; } = new NavigationLayout();
        public LayoutHotReload(string file, Action<string> logger)
        {
            path = file;
            log = logger;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                if (!File.Exists(path)) File.WriteAllText(path, JsonUtility.ToJson(Current, true));
            }
            catch (Exception error) { log("Layout defaults active; config initialization failed: " + error.Message); }
        }

        public bool Poll(float now)
        {
            if (now < nextPoll) return false;
            nextPoll = now + .5f;
            try
            {
                var info = new FileInfo(path);
                if (!info.Exists || (info.LastWriteTimeUtc == lastWrite && info.Length == lastSize)) return false;
                lastWrite = info.LastWriteTimeUtc;
                lastSize = info.Length;
                if (lastSize <= 0 || lastSize > 16384) { log("Layout size invalid; keeping previous layout."); return false; }
                var candidate = new NavigationLayout();
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), candidate);
                if (!candidate.IsValid()) { log("Layout values invalid; keeping previous layout."); return false; }
                Current = candidate;
                return true;
            }
            catch (Exception error) { log("Layout reload failed; keeping previous layout: " + error.Message); return false; }
        }
    }
}
