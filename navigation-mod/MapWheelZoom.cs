using System;

namespace OldMarket.Navigation
{
    internal static class MapWheelZoom
    {
        public static float PlatformScale(bool platformRange,bool windows)=>platformRange&&windows?120:1;
        // UI modules scale wheel events for scrolling lists. Undo that scale before
        // interpreting wheel travel as a map zoom; retain fractional trackpad input.
        public static float Factor(float delta, float uiScale)
        {
            if(float.IsNaN(delta)||float.IsInfinity(delta)||float.IsNaN(uiScale)||float.IsInfinity(uiScale)||uiScale<=0)return 1;
            double ticks=Math.Max(-8,Math.Min(8,delta/uiScale));
            return (float)Math.Pow(1.12,ticks);
        }

        public static float Target(float current,float pending,float factor)
        {
            if((factor>1&&pending<current)||(factor<1&&pending>current))pending=current;
            return Math.Max(1,Math.Min(8,pending*factor));
        }
    }
}
