using System.Collections.Generic;

namespace OldMarket.Navigation
{
    /// <summary>Keep place labels readable without letting them cover nearby map symbols.</summary>
    public static class PoiLabelLayout
    {
        public static bool WantsLabel(float zoom, bool visible, bool target) => target || zoom >= (visible ? 1.35f : 1.5f);

        public static bool Fits(HudBox candidate, HudBox viewport, IReadOnlyList<HudBox> occupied, float gap)
        {
            if(candidate.X < viewport.X+gap || candidate.Y < viewport.Y+gap ||
               candidate.Right > viewport.Right-gap || candidate.Top > viewport.Top-gap)return false;
            for(int i=0;i<occupied.Count;i++)if(candidate.Overlaps(occupied[i],gap))return false;
            return true;
        }

        public static HudBox Candidate(float x,float y,float width,int side)
        {
            const float height=30, offset=26;
            if(side==1)return new HudBox(x-offset-width,y-height/2,width,height);
            if(side==2)return new HudBox(x-width/2,y+offset,width,height);
            if(side==3)return new HudBox(x-width/2,y-offset-height,width,height);
            return new HudBox(x+offset,y-height/2,width,height);
        }

        public static bool TryPlace(float x,float y,float width,HudBox viewport,IReadOnlyList<HudBox> occupied,
            bool wasVisible,ref int side,out HudBox result)
        {
            // Reopening needs more clearance than retaining an existing label, preventing edge chatter.
            float gap=wasVisible?3:8;
            result=Candidate(x,y,width,side);
            if(Fits(result,viewport,occupied,gap))return true;
            for(int next=0;next<4;next++)
            {
                if(next==side)continue;
                var candidate=Candidate(x,y,width,next);
                if(!Fits(candidate,viewport,occupied,gap))continue;
                result=candidate;side=next;return true;
            }
            return false;
        }
    }
}
