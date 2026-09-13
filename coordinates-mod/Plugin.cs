using System.Globalization;
using BepInEx;
using BepInEx.Configuration;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OldMarket.Coordinates
{
    [BepInPlugin("local.oldmarket.coordinates", "Old Market Coordinates", "0.1.4")]
    [BepInProcess("Old Market Simulator.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        private bool visible = true;
        private ConfigEntry<Key> toggleKey;
        private NetworkObject trackedPlayer;
        private Transform positionSource;
        private TextMeshProUGUI label, coins;

        private void Awake()
        {
            // Always show on launch, even when an older installation saved Visible=false.
            var legacyVisible = Config.Bind("Display", "Visible", true, "Always reset to true on launch; F8 only hides for this session.");
            legacyVisible.Value = true;
            toggleKey = Config.Bind("Display", "ToggleKey", Key.F8, "Unity Input System key; None disables the hotkey.");
            gameObject.hideFlags |= HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
            Logger.LogInfo("Coordinates 0.1.4 loaded; visible on launch, position sampled every LateUpdate.");
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
                foreach (var control in keyboard.allKeys)
                    if (control.keyCode == toggleKey.Value && control.wasPressedThisFrame)
                    {
                        visible = !visible;
                        Logger.LogInfo("Coordinate toggle: " + toggleKey.Value + "; visible=" + visible);
                        break;
                    }
        }

        private void LateUpdate()
        {
            var current = UIManager.Instance != null ? UIManager.Instance.textCoins : null;
            if (current != coins || (current != null && label == null))
            {
                Clear();
                coins = current;
                if (coins != null) CreateLabel();
            }
            if (label == null) return;
            var manager = NetworkManager.Singleton;
            var player = manager != null && manager.IsClient ? manager.LocalClient?.PlayerObject : null;
            bool show = visible && player != null && player.IsSpawned && player.IsLocalPlayer;
            label.gameObject.SetActive(show);
            if (!show) return;
            // Use the actual HUD's font asset, material and typography, including locale changes.
            label.font = coins.font;
            label.fontSharedMaterial = coins.fontSharedMaterial;
            label.fontSize = coins.fontSize;
            label.fontStyle = coins.fontStyle;
            label.fontWeight = coins.fontWeight;
            label.color = coins.color;
            label.characterSpacing = coins.characterSpacing;
            if (trackedPlayer != player || positionSource == null)
            {
                trackedPlayer = player;
                var setup = player.GetComponent<ExampleCharacterSetup>();
                positionSource = setup != null && setup.customCharacterController != null
                    ? setup.customCharacterController.transform : player.transform;
            }
            Vector3 position = positionSource.position;
            string value = string.Format(CultureInfo.CurrentCulture, "X {0:F1}  Y {1:F1}  Z {2:F1}", position.x, position.y, position.z);
            if (label.text != value)
            {
                label.text = value;
                Vector2 size = label.GetPreferredValues(value);
                label.rectTransform.sizeDelta = new Vector2(size.x + 8, size.y + 8);
            }
        }

        private void CreateLabel()
        {
            var host = new GameObject("OldMarketCoordinates", typeof(RectTransform), typeof(Canvas), typeof(TextMeshProUGUI), typeof(LayoutElement));
            host.transform.SetParent(coins.transform, false);
            host.GetComponent<LayoutElement>().ignoreLayout = true;
            // A child keeps the native HUD position and scale; its own canvas draws above other HUD panels.
            var canvas = host.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 32760;
            label = host.GetComponent<TextMeshProUGUI>();
            label.raycastTarget = false;
            label.maskable = false;
            label.enableAutoSizing = false;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Overflow;
            label.alignment = TextAlignmentOptions.TopLeft;
            var rect = label.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(0, -6);
            Logger.LogInfo("Coordinates attached below money HUD.");
        }

        private void Clear()
        {
            if (label != null) { label.gameObject.SetActive(false); Destroy(label.gameObject); }
            label = null; coins = null; trackedPlayer = null; positionSource = null;
        }
        private void OnDestroy() => Clear();
    }
}
