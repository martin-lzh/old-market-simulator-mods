using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace OldMarket.Navigation
{
    [BepInPlugin("local.oldmarket.navigation", "Old Market Navigation", "0.1.0")]
    [BepInProcess("Old Market Simulator.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        private readonly NavigationState state = new NavigationState();
        private ConfigEntry<bool> minimap, compass, guidance, coordinates, cameraUp;
        private ConfigEntry<Key> mapKey;
        private ConfigEntry<string> remoteProfile;
        private MapLibrary maps;
        private LayoutHotReload layout;
        private readonly ExpansionState expansions = new ExpansionState();
        private readonly WorldTargetProjection worldTarget = new WorldTargetProjection();
        private readonly Dictionary<string,List<NavMarker>> sessionMarkers = new Dictionary<string,List<NavMarker>>();
        private string contextKey = "", sessionId = Guid.NewGuid().ToString("N");
        private bool hadPlayer;
        private float nextContext;
        private static readonly FieldInfo SlotField = typeof(SaveManager).GetField("currentSlot", BindingFlags.Instance|BindingFlags.NonPublic);
        private static readonly MethodInfo RegionMethod = typeof(RegionManager).GetMethod("GetLocalCurrentRegionSceneName", BindingFlags.Instance|BindingFlags.NonPublic);
        private GameObject canvas;
        private NavigationHud hud;
        private MapWindow window;
        private ExampleCharacterSetup player;
        private bool ownsInput, previousPlayerEnabled, previousCursorVisible;
        private ExampleCharacterSetup inputPlayer;
        private InputManager inputManager;
        private CursorLockMode previousCursorLock;
        private InputManager escapeInputManager;
        private InputAction suppressedSettings, suppressedClose;
        private bool settingsWasEnabled, closeWasEnabled;
        private int restoreEscapeAfterFrame = -1;

        private void Awake()
        {
            minimap = Config.Bind("Display", "Minimap", true, "Show the minimap.");
            compass = Config.Bind("Display", "Compass", true, "Show the compass.");
            guidance = Config.Bind("Display", "TargetGuidance", true, "Show target direction and distance.");
            coordinates = Config.Bind("Display", "Coordinates", false, "Show coordinates below compass; disable when using Coordinates mod.");
            cameraUp = Config.Bind("Display", "CameraUp", false, "Rotate minimap with camera instead of keeping north up.");
            mapKey = Config.Bind("Input", "MapKey", Key.M, "Unity Input System key for map; None disables the hotkey.");
            remoteProfile = Config.Bind("Markers", "RemoteProfile", "", "Optional unique save profile for a remote host. Blank keeps remote markers only for this connection.");
            string modDirectory = Path.GetDirectoryName(Info.Location);
            maps = new MapLibrary(Path.Combine(modDirectory,"maps"), message=>Logger.LogInfo(message));
            string settings = Path.Combine(Paths.ConfigPath,"OldMarket.Navigation");
            state.Store = new MarkerStore(Path.Combine(settings,"markers"));
            state.RotationChanged = value=>cameraUp.Value=value;
            layout = new LayoutHotReload(Path.Combine(settings,"layout.json"), message=>Logger.LogWarning(message));
            gameObject.hideFlags |= HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
            Logger.LogInfo("Navigation 0.1.0 loaded. Map texture is optional; no scene cameras or additional regions are created.");
        }

        private void CreateUi()
        {
            canvas = new GameObject("OldMarketNavigation", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            DontDestroyOnLoad(canvas);
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.GetComponent<Canvas>().sortingOrder = 32000;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = .5f;
            hud = new NavigationHud(state, (RectTransform)canvas.transform, System.IO.Path.GetDirectoryName(Info.Location));
            window = new MapWindow(state, (RectTransform)canvas.transform);
            hud.ApplyLayout(layout.Current);
            window.ApplyLayout(layout.Current);
        }

        private void Update()
        {
            RestoreEscapeActions(false);
            if (window == null) return;
            if (layout.Poll(Time.unscaledTime)) { hud.ApplyLayout(layout.Current); window.ApplyLayout(layout.Current); }
            if (window.IsOpen && (!state.Available || NativePanelOpen())) CloseMap();
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (window.IsOpen && keyboard.escapeKey.wasPressedThisFrame) { CloseMap(); return; }
            bool pressed = false;
            foreach (var key in keyboard.allKeys)
                if (key.keyCode == mapKey.Value && key.wasPressedThisFrame) { pressed = true; break; }
            if (!pressed || Typing()) return;
            if (window.IsOpen) CloseMap();
            else if (state.Available && player != null && !NativePanelOpen() && InputManager.Instance != null && InputManager.Instance.inputMaster.Player.enabled)
            {
                previousPlayerEnabled = InputManager.Instance.inputMaster.Player.enabled;
                inputPlayer=player;
                inputManager=InputManager.Instance;
                previousCursorLock = Cursor.lockState;
                previousCursorVisible = Cursor.visible;
                ownsInput = true;
                SuppressEscapeActions();
                player.DisablePlayerControl(true);
                window.Toggle();
            }
        }

        private bool NativePanelOpen() => (UIManager.Instance != null && UIManager.Instance.IsAnyPanelActive())
            || (SceneSettings.Instance!=null && ((SceneSettings.Instance.panelLoading!=null && SceneSettings.Instance.panelLoading.activeInHierarchy)
                || (SceneSettings.Instance.panelMultiplayerLoading!=null && SceneSettings.Instance.panelMultiplayerLoading.activeInHierarchy)))
            || (player != null && player.overlayRegionLoading != null && player.overlayRegionLoading.activeInHierarchy);
        private static bool Typing()
        {
            var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            return selected != null && (selected.GetComponentInParent<TMP_InputField>() != null || selected.GetComponentInParent<InputField>() != null);
        }

        private void CloseMap()
        {
            window?.Close();
            if (!ownsInput) return;
            ownsInput = false;
            // Enabling Settings in this frame would let the same Escape press open Pause.
            if (suppressedSettings != null || suppressedClose != null) restoreEscapeAfterFrame = Time.frameCount;
            // Do not steal input from a native window opened while the map was visible.
            if (NativePanelOpen() || !state.Available || inputPlayer==null || !inputPlayer.IsSpawned || !inputPlayer.IsLocalPlayer || InputManager.Instance!=inputManager) return;
            if (inputManager != null && previousPlayerEnabled) inputManager.inputMaster.Player.Enable();
            Cursor.lockState = previousCursorLock;
            Cursor.visible = previousCursorVisible;
        }

        private void SuppressEscapeActions()
        {
            if (escapeInputManager == inputManager && suppressedSettings != null)
            {
                // Reopened before a pending restore: retain the original enabled states.
                restoreEscapeAfterFrame = -1;
                return;
            }
            RestoreEscapeActions(true);
            escapeInputManager = inputManager;
            suppressedSettings = inputManager.inputMaster.UI.Settings;
            suppressedClose = inputManager.inputMaster.UI.Close;
            settingsWasEnabled = suppressedSettings.enabled;
            closeWasEnabled = suppressedClose.enabled;
            suppressedSettings.Disable();
            suppressedClose.Disable();
            restoreEscapeAfterFrame = -1;
        }

        private void RestoreEscapeActions(bool immediate)
        {
            if (suppressedSettings == null && suppressedClose == null) return;
            if (!immediate && (restoreEscapeAfterFrame < 0 || Time.frameCount <= restoreEscapeAfterFrame
                || (Keyboard.current != null && Keyboard.current.escapeKey.isPressed))) return;
            // A replacement scene's InputManager owns its own actions. Never enable those by accident.
            if (escapeInputManager != null && InputManager.Instance == escapeInputManager)
            {
                if (settingsWasEnabled && suppressedSettings != null) suppressedSettings.Enable();
                if (closeWasEnabled && suppressedClose != null) suppressedClose.Enable();
            }
            suppressedSettings = suppressedClose = null;
            escapeInputManager = null;
            restoreEscapeAfterFrame = -1;
        }

        private void LateUpdate()
        {
            var manager = NetworkManager.Singleton;
            var local = manager != null && manager.IsClient ? manager.LocalClient?.PlayerObject : null;
            state.Available = local != null && local.IsSpawned && local.IsLocalPlayer && UIManager.Instance != null && UIManager.Instance.textCoins != null;
            if (!state.Available)
            {
                CloseMap();
                if (canvas != null) canvas.SetActive(false);
                if(hadPlayer) { hadPlayer=false; sessionId=Guid.NewGuid().ToString("N"); contextKey=""; state.Markers.Clear(); state.TargetId=""; state.Map=null; sessionMarkers.Clear(); }
                return;
            }
            hadPlayer=true;
            player = local.GetComponent<ExampleCharacterSetup>();
            if (player == null) { state.Available = false; CloseMap(); if(canvas!=null)canvas.SetActive(false); return; }
            state.PlayerPosition = player.customCharacterController != null ? player.customCharacterController.transform.position : local.transform.position;
            var camera = GameManager.Instance != null && GameManager.Instance.exampleCharacterCamera != null ? GameManager.Instance.exampleCharacterCamera.Camera : Camera.main;
            state.CameraYaw = camera != null ? camera.transform.eulerAngles.y : local.transform.eulerAngles.y;
            state.Font = UIManager.Instance.textCoins.font;
            var locale = LocalizationSettings.SelectedLocaleAsync;
            state.Locale = locale.IsDone && locale.Result != null ? locale.Result.Identifier.Code : "en";
            state.RotateWithCamera = cameraUp.Value;
            if(expansions.Poll(Time.unscaledTime)) nextContext=0;
            RefreshContext(manager);
            if (canvas == null) CreateUi();
            canvas.SetActive(true);
            worldTarget.Refresh(state,camera,Time.unscaledTime,window.IsOpen,guidance.Value);
            hud.MinimapVisible = minimap.Value;
            hud.CompassVisible = compass.Value;
            hud.GuidanceVisible = guidance.Value;
            hud.CoordinatesVisible = coordinates.Value;
            hud.Refresh();
            window.Refresh();
            // A close button inside MapWindow follows the same input restoration path.
            if (ownsInput && !window.IsOpen) CloseMap();
        }

        private void RefreshContext(NetworkManager manager)
        {
            if(Time.unscaledTime<nextContext) return;
            nextContext=Time.unscaledTime+.5f;
            var game=GameManager.Instance;
            if(game==null || game.mapSO==null) return;
            string region=RegionManager.Instance!=null && RegionMethod!=null ? (string)RegionMethod.Invoke(RegionManager.Instance,null)??"" : "";
            string mapKey=game.mapSO.id+"|"+game.mapSO.sceneName+"|"+region;
            string profile="";
            if(manager.IsHost && SaveManager.Instance!=null && SlotField!=null)
            {
                string slot=SlotField.GetValue(SaveManager.Instance) as string;
                if(!string.IsNullOrWhiteSpace(slot) && Path.GetFileName(slot)==slot)
                {
                    string dir=Path.Combine(Application.persistentDataPath,slot);
                    // Read directory identity only; never deserialize or edit game saves.
                    profile="local|"+slot+"|"+(Directory.Exists(dir)?Directory.GetCreationTimeUtc(dir).Ticks.ToString():"new");
                }
            }
            else if(!string.IsNullOrWhiteSpace(remoteProfile.Value)) profile="remote|"+remoteProfile.Value.Trim();
            bool persistent=profile.Length>0;
            string next=(persistent?profile:"session|"+sessionId)+"|"+mapKey;
            state.Map=maps.Find(game.mapSO.id,game.mapSO.sceneName,region,expansions.IsReady?expansions.UnlockedIds:null);
            if(next==contextKey) return;
            CloseMap();
            if(contextKey!="") sessionMarkers[contextKey]=state.Markers;
            contextKey=next;
            state.ScopeId=next;
            state.TargetId="";
            state.ErrorKey="";
            state.Store=persistent?state.Store??new MarkerStore(Path.Combine(Paths.ConfigPath,"OldMarket.Navigation","markers")):null;
            try { state.Markers=persistent?state.Store.Load(next):sessionMarkers.TryGetValue(next,out var memory)?memory:new List<NavMarker>(); }
            catch(Exception error)
            {
                state.Markers=new List<NavMarker>();
                state.Store=null; // Keep a corrupt file untouched; this scope is read-only for this connection.
                state.ErrorKey="load_error";
                Logger.LogWarning("Markers could not be loaded; disk writes disabled for this scope: "+error.Message);
            }
        }

        private void OnDisable()
        {
            CloseMap();
            RestoreEscapeActions(true); // No future Update is guaranteed after disable.
            if (canvas != null) canvas.SetActive(false);
        }
        private void OnDestroy()
        {
            CloseMap();
            RestoreEscapeActions(true);
            window?.Dispose();
            hud?.Dispose();
            maps?.Dispose();
            expansions.Dispose();
            worldTarget.Dispose();
            if (canvas != null) Destroy(canvas);
        }
    }
}
