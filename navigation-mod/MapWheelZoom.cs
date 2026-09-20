using System;

namespace OldMarket.Navigation
{
    internal static class MapWheelZoom
    {
        // Preserve the original town crop's closest world-space detail at any
        // viewport aspect. Larger maps traverse the expanded range in the same
        // number of wheel notches/button presses (uniform logarithmic steps).
        public static float Maximum(float worldWidth,float worldHeight,float viewWidth,float viewHeight)
        {
            if(!Positive(worldWidth)||!Positive(worldHeight)||!Positive(viewWidth)||!Positive(viewHeight))return 8;
            double scale=Math.Min((double)worldWidth/viewWidth,(double)worldHeight/viewHeight)
                /Math.Min(340.0/viewWidth,260.0/viewHeight);
            return (float)Math.Max(8,Math.Min(64,8*scale));
        }
        private static bool Positive(float value)=>value>0&&!float.IsNaN(value)&&!float.IsInfinity(value);
        public static float ScaleFactor(float factor,float maximum)=>(float)Math.Pow(factor,Math.Log(maximum)/Math.Log(8));
        public static float PlatformScale(bool platformRange,bool windows)=>platformRange&&windows?120:1;
        // UI modules scale wheel events for scrolling lists. Undo that scale before
        // interpreting wheel travel as a map zoom; retain fractional trackpad input.
        public static float Factor(float delta, float uiScale,float maximum=8)
        {
            if(float.IsNaN(delta)||float.IsInfinity(delta)||float.IsNaN(uiScale)||float.IsInfinity(uiScale)||uiScale<=0)return 1;
            double ticks=Math.Max(-8,Math.Min(8,delta/uiScale));
            return ScaleFactor((float)Math.Pow(1.25,ticks),maximum);
        }

        public static float Target(float current,float pending,float factor,float maximum=8)
        {
            if((factor>1&&pending<current)||(factor<1&&pending>current))pending=current;
            return Math.Max(1,Math.Min(maximum,pending*factor));
        }
    }
}
