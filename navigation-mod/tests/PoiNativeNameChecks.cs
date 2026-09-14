using System;
using OldMarket.Navigation;

internal static class PoiNativeNameChecks
{
    internal static void Run()
    {
        var previous=MapPoi.NativeNameResolver;
        try
        {
            string[] aliases={"engineer","decorations","carpenter","animals","garden","clothing","orders","workshop","farm","museum"};
            string[] native={"engineer","decoration_store","lumberjack","animal_market","gardener","clothing_store","orders","workshop","farm","museum"};
            for(int i=0;i<aliases.Length;i++)
                Equal(MapPoi.NativeKey("poi_"+aliases[i]),native[i],"verified native sign key");
            MapPoi.NativeNameResolver=null;
            var shop=new MapPoi {Name="Invented fallback",NameKey="poi_garden"};
            Equal(shop.DisplayName("zh"),"","unavailable native reader does not invent a label");
            MapPoi.NativeNameResolver=(locale,key)=>locale+":"+key;
            Equal(shop.DisplayName("zh"),"zh:gardener","selected locale and native key reach reader");
            Equal(shop.DisplayName("de"),"de:gardener","language switch reads again without stale MapPoi cache");
            string[] localAliases={"poi_rest","poi_market","poi_water","poi_calendar","poi_return"};
            string[] localLabels={"Rest area","Market","Water refill","Calendar","Return to Rome"};
            for(int i=0;i<localAliases.Length;i++)
                Equal(new MapPoi {Name="Unused fallback",NameKey=localAliases[i]}.DisplayName("en"),localLabels[i],"original functional label bypasses native reader");
            Equal(new MapPoi {Name="Personal name"}.DisplayName("en"),"Personal name","explicit custom name preserved");
            Equal(new MapPoi {NameKey="museum"}.DisplayName("en"),"en:museum","native keys supported directly");
            MapPoi.NativeNameResolver=(locale,key)=>"";
            Equal(shop.DisplayName("zh"),"","pending table does not leak fallback");
            MapPoi.NativeNameResolver=(locale,key)=>"Ready";
            Equal(shop.DisplayName("zh"),"Ready","ready table updates without language change");
            Console.WriteLine("POI native-name checks passed (22).");
        }
        finally { MapPoi.NativeNameResolver=previous; }
    }
    private static void Equal(string actual,string expected,string message)
    { if(actual!=expected)throw new Exception(message+": "+actual); }
}
