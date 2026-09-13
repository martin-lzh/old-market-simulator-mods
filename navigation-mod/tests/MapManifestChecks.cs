using System;
using OldMarket.Navigation;

static class MapManifestChecks
{
    public static void Run(Action<bool,string> check,Action<Action,string> reject)
    {
        const string header="\"MinX\":0,\"MaxX\":10,\"MinZ\":0,\"MaxZ\":10";
        const string point="{\"Id\":\"store\",\"Name\":\"商店\",\"Category\":\"shop\",\"X\":2.5,\"Z\":7.25}";
        var map=MapManifestReader.Read("{"+header+",\"Pois\":["+point+"],\"RequiredExpansions\":[9007199254740993]}");
        check(map.Pois.Length==1&&map.Pois[0].Name=="商店"&&map.Pois[0].Category=="shop","nested POI fields and Unicode survive metadata parsing");
        check(map.Pois[0].X==2.5f&&map.Pois[0].Z==7.25f,"POI coordinates survive metadata parsing");
        check(map.RequiredExpansions[0]==9007199254740993L,"expansion identifiers preserve integer precision");
        var legacy=MapManifestReader.Read("{"+header+"}");
        check(legacy.Pois.Length==0&&legacy.ExcludedExpansions.Length==0,"omitted legacy arrays normalize to empty");
        check(legacy.North=="+Z","omitted north keeps original default");
        var generic=MapManifestReader.Read("{"+header+",\"Pois\":["+point.Replace(",\"Category\":\"shop\"","")+"]}");
        check(generic.Pois[0].Category=="other","omitted category retains generic icon default");
        reject(()=>MapManifestReader.Read("{"+header+",\"Pois\":["+point+","+point+"]}"),"duplicate parsed POIs rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"Pois\":["+point.Replace("2.5","25")+"]}"),"out-of-bounds parsed POIs rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"Pois\":[}"),"malformed POI array rejected");
        reject(()=>MapManifestReader.Read(new string(' ',32769)),"oversized metadata rejected");
    }
}
