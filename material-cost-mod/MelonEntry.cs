#if MELONLOADER
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

[assembly: MelonInfo(typeof(OldMarket.MaterialCost.MelonEntry), "Old Market Material Cost", "0.5.1", "Local Mod")]
[assembly: MelonGame("Alcedo Games", "Old Market Simulator")]

namespace OldMarket.MaterialCost
{
    public sealed class MelonEntry : MelonMod
    {
        private MelonPreferences_Category category;
        private MelonPreferences_Entry<bool> materialsOnly;
        private MelonPreferences_Entry<bool> diagnostics;

        public override void OnInitializeMelon()
        {
            category = MelonPreferences.CreateCategory("OldMarketMaterialCost");
            category.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "OldMarket.MaterialCost.cfg"));
            materialsOnly = category.CreateEntry("MaterialsOnly", false,
                description: "Show recipe material cost estimates in daily sale rows. Cash transactions stay unchanged.");
            diagnostics = category.CreateEntry("Diagnostics", false,
                description: "Enable frame sampling and diagnostic logs for troubleshooting. Requires restart.");
        }

        // Unity APIs are available at this stage. Scene callbacks also recover a destroyed runtime.
        public override void OnLateInitializeMelon() => EnsureRuntime();
        public override void OnSceneWasLoaded(int buildIndex, string sceneName) => EnsureRuntime();

        private void EnsureRuntime()
        {
            if (Plugin.Instance != null || materialsOnly == null) return;
            var host = new GameObject("OldMarketMaterialCost") { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(host);
            var runtime = host.AddComponent<Plugin>();
            try
            {
                runtime.Initialize(() => materialsOnly.Value, value =>
                {
                    materialsOnly.Value = value;
                    category.SaveToFile(false);
                }, message => LoggerInstance.Msg(message), error => LoggerInstance.Error(error), diagnostics.Value);
            }
            catch
            {
                Object.Destroy(host);
                throw;
            }
        }
    }
}
#endif
