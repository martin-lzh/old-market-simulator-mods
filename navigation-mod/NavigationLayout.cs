using System;

namespace OldMarket.Navigation
{
    [Serializable]
    public sealed class NavigationLayout
    {
        public float MinimapSize = 240;
        public bool MinimapBottomLeft = true;
        public float MinimapLeft = 24;
        public float MinimapBottom = 48;
        public float MinimapRight = 24;
        public float MinimapTop = 150;
        public float MinimapRange = 75;
        public float CompassWidth = 520;
        public float CompassTop = 22;
        public float WorldMarkerSize = 28;
        public float MapWidth = 1080;
        public float MapHeight = 720;
        public float Opacity = .92f;

        public bool IsValid()
        {
            return In(MinimapSize, 120, 480) && In(MinimapLeft, 0, 1200) && In(MinimapBottom, 0, 800)
                && In(MinimapRight, 0, 1200) && In(MinimapTop, 0, 800)
                && In(MinimapRange, 20, 1000) && In(CompassWidth, 240, 1000) && In(CompassTop, 0, 800)
                && In(WorldMarkerSize, 12, 80) && In(MapWidth, 600, 1800) && In(MapHeight, 400, 1000)
                && In(Opacity, .2f, 1);
        }
        private static bool In(float value, float min, float max) => !float.IsNaN(value) && !float.IsInfinity(value) && value >= min && value <= max;
    }
}
