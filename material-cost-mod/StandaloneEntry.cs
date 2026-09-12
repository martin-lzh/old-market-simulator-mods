#if STANDALONE
using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OldMarket.MaterialCost
{
    public static class StandaloneEntry
    {
        private static string directory;
        private static Action<string> log;
        private static bool materialsOnly;
        private static bool prepared;

        public static void Prepare(string root, Action<string> logger)
        {
            if (prepared) return;
            directory = root;
            log = logger;
            string setting = Path.Combine(directory, "materials-only.txt");
            materialsOnly = File.Exists(setting) && bool.TryParse(File.ReadAllText(setting).Trim(), out bool value) && value;
            SceneManager.sceneLoaded += SceneLoaded;
            prepared = true;
            log("Scene callback registered; culture=" + CultureInfo.CurrentCulture.Name
                + "; decimal=" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

        private static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Plugin.Instance != null) return;
            GameObject host = null;
            try
            {
                host = new GameObject("OldMarketMaterialCost") { hideFlags = HideFlags.HideAndDontSave };
                UnityEngine.Object.DontDestroyOnLoad(host);
                var runtime = host.AddComponent<Plugin>();
                runtime.Initialize(() => materialsOnly, value =>
                {
                    File.WriteAllText(Path.Combine(directory, "materials-only.txt"), value ? "true" : "false");
                    materialsOnly = value;
                }, log, error => log(error.ToString()), false);
                log("Standalone UI ready; culture=" + CultureInfo.CurrentCulture.Name
                    + "; decimal=" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
            catch (Exception error)
            {
                SceneManager.sceneLoaded -= SceneLoaded;
                log("UI initialization failed; no retry: " + error);
                if (host != null) UnityEngine.Object.Destroy(host);
            }
        }
    }
}
#endif
