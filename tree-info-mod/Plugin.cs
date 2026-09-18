using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace OldMarket.TreeInfo
{
    [BepInPlugin(Id, "Old Market Tree Info", "0.1.1")]
    [BepInProcess("Old Market Simulator.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Id = "local.oldmarket.treeinfo";
        private Harmony harmony;
        private static Plugin instance;
        private static FieldInfo dayCounter;
        private static FieldInfo isWatered;
        private static bool failed;

        private void Awake()
        {
            instance = this;
            failed = false;
            harmony = new Harmony(Id);
            try
            {
                dayCounter = RequiredField("dayCounter", typeof(NetworkVariable<int>));
                isWatered = RequiredField("isWatered", typeof(NetworkVariable<bool>));
                var method = AccessTools.Method(typeof(PlayerInteraction), "InteractionRay")
                    ?? throw new MissingMethodException("PlayerInteraction.InteractionRay");
                harmony.Patch(method, postfix: new HarmonyMethod(typeof(Plugin), nameof(ShowTreeInfo)));
                Logger.LogInfo("Tree Info 0.1.1 ready; hover display only.");
            }
            catch (Exception error)
            {
                harmony.UnpatchSelf();
                Logger.LogError(error);
                enabled = false;
            }
        }

        private static FieldInfo RequiredField(string name, Type type)
        {
            var field = AccessTools.Field(typeof(BlockTree), name);
            if (field == null || field.FieldType != type) throw new MissingFieldException("BlockTree", name);
            return field;
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            instance = null;
        }

        private static void ShowTreeInfo(PlayerInteraction __instance, Camera ___mainCamera, bool ___isInteractionEnabled)
        {
            if (failed || instance == null || !instance.enabled || !__instance.IsOwner || !___isInteractionEnabled ||
                ___mainCamera == null || __instance.interactionUI == null || !__instance.interactionUI.activeInHierarchy ||
                __instance.textSubtitle == null) return;
            try
            {
                // Match the native ray, distance, layer mask and first Interactable exactly.
                var ray = ___mainCamera.ViewportPointToRay(Vector3.one / 2f);
                if (!Physics.Raycast(ray, out var hit, __instance.interactionDistance,
                    __instance.layerMask, QueryTriggerInteraction.Ignore)) return;
                if (!(hit.collider.GetComponentInParent<Interactable>() is BlockTree tree) ||
                    !tree.IsSpawned || !(tree.blockSO is TreeSO config) || config.productSO == null) return;
                var game = GameManager.Instance;
                if (game == null || !game.IsSpawned) return;
                var season = config.productSO.season;
                var text = Texts.For(LocalizationSettings.SelectedLocale?.Identifier.Code);
                string name = season == null ? text.AllSeasons : season.GetLocalizedName();
                int counter = ((NetworkVariable<int>)dayCounter.GetValue(tree)).Value;
                bool watered = ((NetworkVariable<bool>)isWatered.GetValue(tree)).Value;
                string status = HarvestEstimate.Describe(counter, config.daysToHarvestAfterGrowth,
                    season == null || season == game.GetCurrentSeason(), game.GetCurrentDay(), watered, text);
                string addition = string.Format(text.Season, name) + "\n" + status;
                string existing = __instance.textSubtitle.text;
                __instance.textSubtitle.text = string.IsNullOrEmpty(existing) ? addition : existing + "\n" + addition;
            }
            catch (Exception error)
            {
                // Stop retries to avoid per-frame errors after an incompatible game update.
                failed = true;
                instance.Logger.LogError(error);
            }
        }
    }
}
