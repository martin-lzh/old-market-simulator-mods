using System;
using OldMarket.Navigation;

internal static class MapWheelZoomChecks
{
    internal static void Run(Action<bool,string> check)
    {
        void Near(float a,float b,string name)=>check(Math.Abs(a-b)<.0001f,name);
        Near(MapWheelZoom.Factor(6,6),1.25f,"UI-scaled wheel notch matches the button step");
        Near(MapWheelZoom.Factor(720,6*MapWheelZoom.PlatformScale(true,true)),1.25f,"Windows platform-range wheel normalizes 120 before zoom");
        Near(MapWheelZoom.Factor(6,6*MapWheelZoom.PlatformScale(false,true)),1.25f,"uniform Windows wheel is not divided by 120");
        Near(MapWheelZoom.PlatformScale(true,false),1,"other supported platform native units remain one");
        Near(MapWheelZoom.Factor(2.5f,2.5f),1.25f,"custom UI scroll scale preserves map sensitivity");
        Near(MapWheelZoom.Factor(1,1),1.25f,"legacy module notch uses unscaled wheel units");
        Near(MapWheelZoom.Factor(.6f,6), (float)Math.Pow(1.25,.1),"fractional wheel motion is not rounded");
        float pieces=1;for(int i=0;i<10;i++)pieces*=MapWheelZoom.Factor(.6f,6);
        Near(pieces,MapWheelZoom.Factor(6,6),"split high-resolution scroll equals one full notch");
        Near(MapWheelZoom.Factor(-6,6)*MapWheelZoom.Factor(6,6),1,"opposite wheel travel cancels");
        foreach(float bad in new[]{float.NaN,float.PositiveInfinity,float.NegativeInfinity})
        {Near(MapWheelZoom.Factor(bad,6),1,"invalid delta ignored");Near(MapWheelZoom.Factor(6,bad),1,"invalid module scale ignored");}
        Near(MapWheelZoom.Factor(6,0),1,"zero module scale cannot divide by zero");
        Near(MapWheelZoom.Factor(6,-1),1,"invalid negative module scale ignored");
        check(MapWheelZoom.Factor(float.MaxValue,6)<6f,"extreme input remains bounded");
        Near(MapWheelZoom.Target(2,3,.9f),1.8f,"reversing wheel immediately reverses pending zoom");
        Near(MapWheelZoom.Target(3,2,1.1f),3.3f,"reverse zoom-in discards pending zoom-out");
        Near(MapWheelZoom.Target(2,3,1.1f),3.3f,"same direction accumulates pending travel");
        Near(MapWheelZoom.Target(1,1,.9f),1,"zoom-out respects map coverage floor");
        Near(MapWheelZoom.Target(8,8,1.1f),8,"zoom-in respects maximum");
        foreach(var view in new[]{new MapPoint(900,500),new MapPoint(500,900),new MapPoint(800,800)})
        {
            Near(MapWheelZoom.Maximum(340,260,view.X,view.Y),8,"original town zoom range retained");
            float maximum=MapWheelZoom.Maximum(750,750,view.X,view.Y);
            var oldDetail=new MapViewportGeometry(view.X,view.Y,340f/260,8);
            var fullDetail=new MapViewportGeometry(view.X,view.Y,1,maximum);
            Near(oldDetail.Width/340,fullDetail.Width/750,"full map reaches original world-space detail");
            float fullFactor=MapWheelZoom.Factor(6,6,maximum);
            Near((float)(Math.Log(maximum)/Math.Log(fullFactor)),(float)(Math.Log(8)/Math.Log(1.25)),"same wheel travel spans larger map zoom range");
            Near(MapWheelZoom.Factor(-6,6,maximum)*fullFactor,1,"adaptive wheel zoom remains reversible");
            float partial=1;for(int i=0;i<10;i++)partial*=MapWheelZoom.Factor(.6f,6,maximum);
            Near(partial,fullFactor,"adaptive zoom preserves fractional scrolling");
            foreach(float direction in new[]{-1f,1f})
            {
                float button=MapWheelZoom.Factor(direction,1,maximum);
                Near(MapWheelZoom.Factor(direction*6,6,maximum),button,"uniform wheel notch equals one button press");
                Near(MapWheelZoom.Factor(direction*720,720,maximum),button,"Windows wheel notch equals one button press");
                foreach(var pending in new[]{new MapPoint(1,1),new MapPoint(2,3),new MapPoint(3,2),new MapPoint(maximum,maximum)})
                    Near(MapWheelZoom.Target(pending.X,pending.Y,button,maximum),MapWheelZoom.Target(pending.X,pending.Y,MapWheelZoom.Factor(direction*6,6,maximum),maximum),"wheel and button agree during animation, reversal and clamping");
            }
            Near(MapWheelZoom.Factor(1,1,maximum)*MapWheelZoom.Factor(-1,1,maximum),1,"shared zoom steps are reciprocal");
            Near(MapWheelZoom.Target(maximum,maximum,fullFactor,maximum),maximum,"adaptive maximum enforced");
            check(MapWheelZoom.Target(8,8,fullFactor,maximum)>8,"full map zoom can exceed previous cap");
        }
        Near(MapWheelZoom.Maximum(10,10,900,500),8,"small maps retain existing range");
        Near(MapWheelZoom.Maximum(float.MaxValue,float.MaxValue,900,500),64,"very large maps have bounded zoom");
        Near(MapWheelZoom.Maximum(750,750,0,500),8,"uninitialized viewport uses safe default");
        Near(MapWheelZoom.Maximum(float.NaN,750,900,500),8,"nonfinite bounds use safe default");
    }
}
