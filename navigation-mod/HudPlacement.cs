using System;
using System.Collections.Generic;

namespace OldMarket.Navigation
{
    // Screen-space rectangles, with Y increasing upwards. Kept independent of Unity for boundary tests.
    public readonly struct HudBox
    {
        public readonly float X, Y, Width, Height;
        public float Right => X + Width;
        public float Top => Y + Height;
        public HudBox(float x, float y, float width, float height) { X=x; Y=y; Width=width; Height=height; }
        public bool Overlaps(HudBox b, float gap=0) => X < b.Right+gap && Right+gap > b.X && Y < b.Top+gap && Top+gap > b.Y;
    }

    public static class HudPlacement
    {
        // Prefer the requested slot, then the closest free edge of another occupied rectangle.
        // If no complete slot exists, return false instead of placing content on top of another HUD.
        public static bool TryPlace(HudBox desired, HudBox screen, IReadOnlyList<HudBox> occupied, float gap, out HudBox result)
        {
            result=desired;
            if (desired.Width<=0 || desired.Height<=0 || desired.Width>screen.Width || desired.Height>screen.Height) return false;
            var xs=new List<float>{desired.X,screen.X,screen.Right-desired.Width};
            var ys=new List<float>{desired.Y,screen.Y,screen.Top-desired.Height};
            foreach(var box in occupied)
            {
                xs.Add(box.X-gap-desired.Width); xs.Add(box.Right+gap);
                ys.Add(box.Y-gap-desired.Height); ys.Add(box.Top+gap);
            }
            float best=float.PositiveInfinity;
            foreach(float x in xs) foreach(float y in ys)
            {
                var candidate=new HudBox(Math.Max(screen.X,Math.Min(screen.Right-desired.Width,x)), Math.Max(screen.Y,Math.Min(screen.Top-desired.Height,y)),desired.Width,desired.Height);
                bool blocked=false;
                foreach(var box in occupied) if(candidate.Overlaps(box,gap)) {blocked=true;break;}
                if(blocked)continue;
                float dx=candidate.X-desired.X,dy=candidate.Y-desired.Y,score=dx*dx+dy*dy;
                if(score<best) {best=score;result=candidate;}
            }
            return !float.IsPositiveInfinity(best);
        }
    }
}
