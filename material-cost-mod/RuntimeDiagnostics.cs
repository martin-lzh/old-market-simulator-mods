using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OldMarket.MaterialCost
{
    // Bounded, allocation-free frame counting; one aggregate log per 10 seconds.
    // No changes to rendering, GC, game settings, or saves.
    public sealed class RuntimeDiagnostics : MonoBehaviour
    {
        private int frames, slow, uiSlow, gcStart, focusedFrames, focusedSlow;
        private float nextLog, worst;
        private long lastWallTick;
        private double worstWall;
        private void Start()
        {
            gcStart = System.GC.CollectionCount(0);
            nextLog = Time.realtimeSinceStartup + 10;
            lastWallTick = System.Diagnostics.Stopwatch.GetTimestamp();
            Plugin.Trace("Diagnostics started; plugin survived initial scene load.");
            SceneManager.sceneLoaded += SceneLoaded;
        }
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
            => Plugin.Trace($"Scene loaded: {scene.name}; recipe callbacks={Plugin.RecipeCalls}");
        private void Update()
        {
            var now = System.Diagnostics.Stopwatch.GetTimestamp();
            var wallSeconds = (double)(now - lastWallTick) / System.Diagnostics.Stopwatch.Frequency;
            lastWallTick = now;
            if (wallSeconds > worstWall) worstWall = wallSeconds;
            frames++;
            if (Application.isFocused) focusedFrames++;
            var seconds = Time.unscaledDeltaTime;
            if (seconds > worst) worst = seconds;
            if (seconds >= .1f)
            {
                slow++;
                if (Application.isFocused) focusedSlow++;
                if (Time.frameCount - Plugin.LastUiFrame <= 1) uiSlow++;
            }
            if (Time.realtimeSinceStartup < nextLog) return;
            Flush();
        }
        internal void Flush()
        {
            if (frames == 0) return;
            var ui = UIManager.Instance;
            Plugin.Trace($"Frame sample: utc={System.DateTime.UtcNow:O}, frames={frames}, over100ms={slow}, nearModUI={uiSlow}, worstMs={worst * 1000:F1}, worstWallMs={worstWall * 1000:F1}, GC0={System.GC.CollectionCount(0) - gcStart}, focusedFrames={focusedFrames}, focusedSlow={focusedSlow}, recipes={Plugin.RecipeCalls}, recipePanel={(ui != null && ui.panelRecipes != null && ui.panelRecipes.activeInHierarchy)}");
            frames = slow = uiSlow = focusedFrames = focusedSlow = 0; worst = 0;
            worstWall = 0;
            gcStart = System.GC.CollectionCount(0);
            nextLog = Time.realtimeSinceStartup + 10;
        }
        private void OnApplicationFocus(bool focused)
            => Plugin.Trace($"Focus changed: focused={focused}, utc={System.DateTime.UtcNow:O}, frame={Time.frameCount}");
        internal static IEnumerator DescribeNextFrame(GameObject panel, string name)
        {
            yield return null;
            if (panel == null) yield break;
            foreach (var rect in panel.GetComponentsInChildren<RectTransform>(true))
                if (rect.name == name)
                    Plugin.Trace($"UI {name}: active={rect.gameObject.activeInHierarchy}, rect={rect.rect}, world={rect.position}, scale={rect.lossyScale}, parent={rect.parent.name}");
        }
        private void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;
    }
}
