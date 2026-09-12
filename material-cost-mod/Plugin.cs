using System;
#if !MELONLOADER && !STANDALONE
using BepInEx;
#endif
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OldMarket.MaterialCost
{
#if MELONLOADER || STANDALONE
    public sealed class Plugin : MonoBehaviour
#else
    [BepInPlugin(Id, "Old Market Material Cost", "0.5.0")]
    public sealed class Plugin : BaseUnityPlugin
#endif
    {
        public const string Id = "local.oldmarket.materialcost";
        internal static Plugin Instance;
        private Func<bool> readMaterialsOnly;
        private Action<bool> writeMaterialsOnly;
        private Action<string> log;
        private Action<Exception> logError;
        internal bool MaterialsOnly { get => readMaterialsOnly(); set => writeMaterialsOnly(value); }
        private HarmonyLib.Harmony harmony;
        internal static int LastUiFrame = -100;
        internal static int RecipeCalls, ReportCalls;
        private bool diagnostics;

        internal static void Trace(string message) => Instance?.log?.Invoke(message);

#if !MELONLOADER && !STANDALONE
        private void Awake()
        {
            var setting = Config.Bind("Report", "MaterialsOnly", false,
                "Show recipe material cost estimates in daily sale rows. Cash transactions stay unchanged.");
            var diagnosticsSetting = Config.Bind("Diagnostics", "Enabled", false,
                "Enable frame sampling and diagnostic logs for troubleshooting. Requires restart.");
            Initialize(() => setting.Value, value => setting.Value = value,
                message => Logger.LogInfo(message), error => Logger.LogError(error), diagnosticsSetting.Value);
        }
#endif

        internal void Initialize(Func<bool> read, Action<bool> write, Action<string> info, Action<Exception> error,
            bool enableDiagnostics = false)
        {
            if (Instance != null) throw new InvalidOperationException("Material cost runtime already exists.");
            readMaterialsOnly = read;
            writeMaterialsOnly = write;
            log = info;
            logError = error;
            diagnostics = enableDiagnostics;
            Instance = this;
            // This game's initial scene unload otherwise destroys the manager at frame zero.
            gameObject.hideFlags |= HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
            harmony = new HarmonyLib.Harmony(Id);
            harmony.Patch(AccessTools.Method(typeof(UIManager), nameof(UIManager.OpenEndOfDayPanel)),
                postfix: new HarmonyMethod(typeof(Plugin), nameof(AfterReport)));
            harmony.Patch(AccessTools.Method(typeof(UIManager), nameof(UIManager.UpdateRecipeIngredients)),
                postfix: new HarmonyMethod(typeof(Plugin), nameof(AfterRecipe)));
            harmony.Patch(AccessTools.Method(typeof(UIManager), nameof(UIManager.OpenRecipesPanel)),
                postfix: new HarmonyMethod(typeof(Plugin), nameof(AfterRecipesOpened)));
            harmony.Patch(AccessTools.Method(typeof(DockOrderTile), nameof(DockOrderTile.SetData)),
                postfix: new HarmonyMethod(typeof(Plugin), nameof(AfterDockItem)));
            RuntimeRules.Clear();
            SceneManager.sceneLoaded += SceneLoaded;
            Trace($"Material cost loaded; on-demand quotes, diagnostics={diagnostics}.");
            if (diagnostics)
            {
                gameObject.AddComponent<RuntimeDiagnostics>();
                foreach (var method in harmony.GetPatchedMethods())
                    Trace($"Registered patch: {method.DeclaringType?.FullName}.{method.Name}");
            }
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode) => RuntimeRules.Clear();

        private static void AfterDockItem(DockOrderTile __instance, ProductSO productSO)
        {
            try
            {
                var ui = UIManager.Instance;
                if (ui == null || !__instance.transform.IsChildOf(ui.listDockOrders)) return;
                var view = ui.panelDockOrders.GetComponent<OrderPriceView>()
                    ?? ui.panelDockOrders.AddComponent<OrderPriceView>();
                view.Bind(ui, productSO);
                var target = __instance.GetComponent<OrderPriceTarget>() ?? __instance.gameObject.AddComponent<OrderPriceTarget>();
                target.View = view;
                target.Product = productSO;
                foreach (var button in new[] { __instance.buttonAdd, __instance.buttonRemove })
                {
                    if (button == null) continue;
                    var child = button.GetComponent<OrderPriceTarget>() ?? button.gameObject.AddComponent<OrderPriceTarget>();
                    child.View = view; child.Product = productSO;
                }
            }
            catch (Exception error)
            {
                Instance.logError(error);
                UIManager.Instance?.panelDockOrders.GetComponent<OrderPriceView>()?.Restore();
            }
        }

        private static void AfterRecipe(UIManager __instance, RecipeSO recipeSO)
        {
            bool diagnose = Instance.diagnostics;
            long started = diagnose ? System.Diagnostics.Stopwatch.GetTimestamp() : 0;
            RecipeCalls++;
            if (diagnose) LastUiFrame = Time.frameCount;
            if (diagnose && RecipeCalls <= 4) Trace($"Recipe callback {RecipeCalls}: {recipeSO?.name}");
            try
            {
                var view = __instance.panelRecipes.GetComponent<RecipeCostView>()
                    ?? __instance.panelRecipes.AddComponent<RecipeCostView>();
                view.Bind(__instance, recipeSO);
                if (diagnose && RecipeCalls <= 4) Instance.StartCoroutine(RuntimeDiagnostics.DescribeNextFrame(__instance.panelRecipes, "RecipeMaterialCost"));
            }
            catch (Exception error)
            {
                Instance.logError(error);
                var view = __instance.panelRecipes.GetComponent<RecipeCostView>();
                if (view != null) view.Restore();
            }
            finally
            {
                if (diagnose && RecipeCalls <= 4)
                    Trace($"Recipe callback duration: {(System.Diagnostics.Stopwatch.GetTimestamp() - started) * 1000.0 / System.Diagnostics.Stopwatch.Frequency:F2} ms");
            }
        }

        private static void AfterRecipesOpened(UIManager __instance, System.Collections.Generic.List<RecipeSO> recipes)
        {
            if (recipes == null || recipes.Count == 0)
                __instance.panelRecipes.GetComponent<RecipeCostView>()?.Restore();
        }

        private static void AfterReport(UIManager __instance, Sale[] dailySales)
        {
            ReportCalls++;
            if (Instance.diagnostics)
            {
                LastUiFrame = Time.frameCount;
                Trace($"Report callback {ReportCalls}: {dailySales?.Length ?? 0} sales");
            }
            try
            {
                var view = __instance.panelEndOfDay.GetComponent<ReportView>()
                    ?? __instance.panelEndOfDay.AddComponent<ReportView>();
                view.Bind(__instance, dailySales ?? Array.Empty<Sale>());
            }
            catch (Exception error)
            {
                // A failed display enhancement must not break sleeping/next day.
                Instance.logError(error);
                var view = __instance.panelEndOfDay.GetComponent<ReportView>();
                if (view != null) view.Restore();
            }
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            GetComponent<RuntimeDiagnostics>()?.Flush();
            Trace($"Plugin destroyed; frame={Time.frameCount}, recipes={RecipeCalls}, reports={ReportCalls}");
            harmony?.UnpatchSelf();
            SceneManager.sceneLoaded -= SceneLoaded;
            RuntimeRules.Clear();
            foreach (var view in FindObjectsOfType<ReportView>(true)) { view.Restore(); Destroy(view); }
            foreach (var view in FindObjectsOfType<RecipeCostView>(true)) { view.Restore(); Destroy(view); }
            foreach (var target in FindObjectsOfType<OrderPriceTarget>(true)) Destroy(target);
            foreach (var view in FindObjectsOfType<OrderPriceView>(true)) { view.Restore(); Destroy(view); }
            Instance = null;
        }
    }
}
