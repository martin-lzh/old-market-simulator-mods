using System;
using System.Collections.Generic;
using OldMarket.Navigation;

static class PoiLabelChecks
{
    public static void Check(Action<bool,string> check)
    {
        check(!PoiLabelLayout.WantsLabel(1.49f,false,false),"POI stays icon below opening zoom");
        check(PoiLabelLayout.WantsLabel(1.5f,false,false),"POI opens label at detail zoom");
        check(PoiLabelLayout.WantsLabel(1.4f,true,false),"POI keeps label inside zoom hysteresis");
        check(!PoiLabelLayout.WantsLabel(1.34f,true,false),"POI collapses below retention zoom");
        check(PoiLabelLayout.WantsLabel(1,false,true),"POI target bypasses zoom threshold");
        var viewport=new HudBox(-200,-150,400,300);var blocked=new List<HudBox>();int side=0;
        blocked.Add(new HudBox(-16,-16,32,32));
        check(PoiLabelLayout.TryPlace(0,0,120,viewport,blocked,false,ref side,out var result)&&side==0,"POI prefers right label");
        blocked.Add(result);
        check(PoiLabelLayout.TryPlace(0,0,120,viewport,blocked,false,ref side,out result)&&side==1,"POI moves label away from neighbor");
        blocked.Add(result);blocked.Add(PoiLabelLayout.Candidate(0,0,120,2));blocked.Add(PoiLabelLayout.Candidate(0,0,120,3));
        check(!PoiLabelLayout.TryPlace(0,0,120,viewport,blocked,true,ref side,out result),"POI collapses when all label positions occupied");
        blocked.Clear();side=0;
        check(PoiLabelLayout.TryPlace(174,0,120,viewport,blocked,false,ref side,out result)&&side==1,"POI right viewport edge uses left label");
        check(!PoiLabelLayout.Fits(new HudBox(100,140,60,30),viewport,blocked,3),"POI top clipping rejected");
        blocked.Add(new HudBox(145,-15,20,30));
        var near=new HudBox(22,-15,120,30);
        check(PoiLabelLayout.Fits(near,viewport,blocked,3)&&!PoiLabelLayout.Fits(near,viewport,blocked,8),"POI spatial hysteresis keeps but does not reopen near collision");
        for(int x=-190;x<=190;x+=19)for(int y=-140;y<=140;y+=20)
        {
            side=0;
            if(PoiLabelLayout.TryPlace(x,y,100,viewport,blocked,false,ref side,out result))
                check(PoiLabelLayout.Fits(result,viewport,blocked,8),"POI accepted label inside viewport and clear of symbol");
        }
    }
}
