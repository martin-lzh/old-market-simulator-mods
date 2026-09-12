using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OldMarket.CheckoutAll
{
    [BepInPlugin(Id, "Old Market Checkout All", "0.1.3")]
    [BepInProcess("Old Market Simulator.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        private const string Id = "local.oldmarket.checkoutall";
        private static Plugin instance;
        private Harmony harmony;
        private ConfigEntry<float> holdSeconds;
        private ConfigEntry<Key> toggleKey;
        private readonly CheckoutSession session = new CheckoutSession();
        private readonly ConfirmationWait confirmation = new ConfirmationWait();
        private CheckoutItem pendingItem;
        private CoinPouch pendingPayment;
        private readonly HashSet<CheckoutItem> sentItems = new HashSet<CheckoutItem>();
        private readonly HashSet<CoinPouch> sentPouches = new HashSet<CoinPouch>();
        private Checkout checkout;
        private float nextAction;
        private int actionFrame;
        private readonly CheckoutHint hint = new CheckoutHint();
        private PlayerInteraction hintPlayer;
        private string hintMessage;

        private void Awake()
        {
            instance = this;
            holdSeconds = Config.Bind("Checkout", "HoldSeconds", .6f,
                new ConfigDescription("Hold E to keep processing customers at the same checkout until released.", new AcceptableValueRange<float>(.3f, 2f)));
            toggleKey = Config.Bind("Checkout", "ToggleKey", Key.F9,
                "Keyboard key to toggle continuous checkout at the current counter. None disables the shortcut.");
            gameObject.hideFlags |= HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
            harmony = new Harmony(Id);
            harmony.Patch(AccessTools.Method(typeof(PlayerInteraction), "InteractionRay"),
                postfix: new HarmonyMethod(typeof(Plugin), nameof(AfterInteraction)));
            // Prevent the initial native E press and the batch from sending duplicate requests for one object.
            harmony.Patch(AccessTools.Method(typeof(CheckoutItem), nameof(CheckoutItem.Interact)),
                prefix: new HarmonyMethod(typeof(Plugin), nameof(BeforeItem)));
            harmony.Patch(AccessTools.Method(typeof(CoinPouch), nameof(CoinPouch.Interact)),
                prefix: new HarmonyMethod(typeof(Plugin), nameof(BeforePouch)));
            Logger.LogInfo($"Checkout All ready: hold E or toggle {toggleKey.Value} for continuous checkout.");
        }

        private static bool BeforeItem(CheckoutItem __instance) => instance == null || instance.AllowItem(__instance);
        private bool AllowItem(CheckoutItem item)
        {
            sentItems.RemoveWhere(x => x == null || !x.IsSpawned);
            return item != null && item.IsSpawned && sentItems.Add(item);
        }
        private static bool BeforePouch(CoinPouch __instance)
        {
            if (instance == null) return true;
            instance.sentPouches.RemoveWhere(x => x == null || !x.IsSpawned);
            // Taking a pouch (including the initial native press) must not end a held session.
            return __instance != null && __instance.IsSpawned && instance.sentPouches.Add(__instance);
        }

        private static void AfterInteraction(PlayerInteraction __instance, Camera ___mainCamera, bool ___isInteractionEnabled)
        {
            if (instance == null || !__instance.IsLocalPlayer) return;
            try { instance.Tick(__instance, ___mainCamera, ___isInteractionEnabled); }
            catch (Exception error) { instance.Cancel(); instance.Logger.LogError(error); }
        }

        private Checkout LookAtCheckout(PlayerInteraction player, Camera camera)
        {
            if (camera == null || !Physics.Raycast(camera.ViewportPointToRay(Vector3.one / 2f), out var hit,
                player.interactionDistance, player.layerMask, QueryTriggerInteraction.Ignore)) return null;
            var interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable is CheckoutBell bell) return bell.checkout;
            if (interactable is CoinPouch pouch) return MatchPouchToCheckout(pouch);
            return interactable is Checkout || interactable is CheckoutItem
                ? interactable.GetComponentInParent<Checkout>() : null;
        }

        // CoinPouch.checkout is server-only. Clients can identify its native spawn point, but never guess between counters.
        private static Checkout MatchPouchToCheckout(CoinPouch pouch)
        {
            Checkout match = null;
            foreach (var counter in FindObjectsOfType<Checkout>())
            {
                if (!counter.IsSpawned || counter.coinPouchPoint == null ||
                    (counter.coinPouchPoint.position - pouch.transform.position).sqrMagnitude > .0001f) continue;
                if (match != null) return null;
                match = counter;
            }
            return match;
        }

        private void Tick(PlayerInteraction player, Camera camera, bool interactionEnabled)
        {
            actionFrame = Time.frameCount;
            bool held = Keyboard.current != null && Keyboard.current.eKey.isPressed;
            bool allowed = interactionEnabled && player.isActiveAndEnabled && Application.isFocused &&
                Cursor.lockState == CursorLockMode.Locked && Time.timeScale > 0;
            var target = allowed ? LookAtCheckout(player, camera) : null;
            if (target != null && !target.IsSpawned) target = null;
            bool togglePressed = Keyboard.current != null && toggleKey.Value != Key.None &&
                Keyboard.current[toggleKey.Value].wasPressedThisFrame;
            session.Tick(held, togglePressed, target, Time.deltaTime, holdSeconds.Value);
            if (session.Target == null) ClearCheckout();
            else if (checkout == null)
            {
                checkout = target;
                nextAction = 0;
                confirmation.Reset();
            }
            hintPlayer = player;
            string shortcut = toggleKey.Value == Key.None ? "" : $" / {toggleKey.Value}：开关连续结账";
            hintMessage = target == null ? null : checkout == null ? "长按 E：连续装袋并收款" + shortcut :
                session.Toggled ? $"连续结账已开启 · {toggleKey.Value}：关闭" : "连续结账中… 松开 E 停止" + shortcut;
            if (checkout == null || Time.unscaledTime < nextAction) return;
            bool pending = (pendingItem != null && pendingItem.IsSpawned) ||
                (pendingPayment != null && pendingPayment.IsSpawned);
            if (confirmation.TimedOut(pending, Time.unscaledTime))
            {
                Logger.LogWarning("Checkout confirmation timed out; toggle again or release and hold E to restart.");
                Cancel();
                return;
            }
            if (pending) return;
            pendingItem = null;
            pendingPayment = null;
            nextAction = Time.unscaledTime + .08f;
            // Refresh the table after each server confirmation, including subsequent customers.
            var items = checkout.GetCheckoutItems();
            foreach (var item in items)
            {
                if (item == null || !item.IsSpawned || sentItems.Contains(item)) continue;
                if (item.GetComponentInParent<Checkout>() != checkout) { Cancel(); return; }
                pendingItem = item;
                item.Interact();
                confirmation.TimedOut(item != null && item.IsSpawned, Time.unscaledTime);
                return;
            }
            // Wait for server despawns before looking for the payment. Never call CheckProducts/SellServer directly.
            if (items.Count != 0)
            {
                // The native short press may have sent an item before this session started.
                pendingItem = items[0];
                confirmation.TimedOut(true, Time.unscaledTime);
                return;
            }
            CoinPouch payment = checkout.GetCoinPouch();
            if (payment == null)
                foreach (var pouch in FindObjectsOfType<CoinPouch>())
                {
                    if (!pouch.IsSpawned || checkout.coinPouchPoint == null ||
                        (pouch.transform.position - checkout.coinPouchPoint.position).sqrMagnitude > .0001f) continue;
                    if (payment != null) { Cancel(); return; }
                    payment = pouch;
                }
            if (payment == null || !payment.IsSpawned) return;
            if (MatchPouchToCheckout(payment) != checkout) { Cancel(); return; }
            pendingPayment = payment;
            if (!sentPouches.Contains(payment)) payment.Interact();
            confirmation.TimedOut(payment != null && payment.IsSpawned, Time.unscaledTime);
            // Keep the session alive while the next customer approaches. Empty tables have no timeout.
        }

        private void LateUpdate()
        {
            hint.Show(hintPlayer, actionFrame == Time.frameCount ? hintMessage : null);
            // Interaction callbacks stop during disconnect/despawn; don't carry a batch into a new scene.
            if (checkout != null && actionFrame != Time.frameCount) Cancel();
        }
        private void Cancel()
        {
            session.Cancel();
            ClearCheckout();
        }
        private void ClearCheckout()
        {
            checkout = null;
            pendingItem = null;
            pendingPayment = null;
            confirmation.Reset();
        }
        private void OnDestroy() { hint.Destroy(); harmony?.UnpatchSelf(); Cancel(); if (instance == this) instance = null; }
    }
}
