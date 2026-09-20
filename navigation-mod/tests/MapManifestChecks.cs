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
        check(legacy.Areas.Length==0,"legacy maps have no conditional areas");
        check(legacy.TextureUvMin.X==0&&legacy.TextureUvMin.Y==0&&legacy.TextureUvMax.X==1&&legacy.TextureUvMax.Y==1,"legacy maps use the full texture");
        const string textureBounds="\"TextureBounds\":{\"MinX\":-10,\"MaxX\":30,\"MinZ\":-20,\"MaxZ\":60}";
        var cropped=MapManifestReader.Read("{"+header+","+textureBounds+",\"Pois\":["+point+"]}");
        check(cropped.TextureUvMin.X==.25f&&cropped.TextureUvMin.Y==.25f&&cropped.TextureUvMax.X==.5f&&cropped.TextureUvMax.Y==.375f,"crop retains calibrated texture rectangle");
        foreach(var position in new[]{new MapPoint(0,0),new MapPoint(10,10),new MapPoint(2.5f,7.25f)})
        {
            var uv=NavMath.WorldToUv(position.X,position.Y,cropped.MinX,cropped.MaxX,cropped.MinZ,cropped.MaxZ);
            var original=cropped.TextureBounds.Project(position.X,position.Y);
            float u=cropped.TextureUvMin.X+uv.X*(cropped.TextureUvMax.X-cropped.TextureUvMin.X);
            float v=cropped.TextureUvMin.Y+uv.Y*(cropped.TextureUvMax.Y-cropped.TextureUvMin.Y);
            check(Math.Abs(u-original.X)<.00001f&&Math.Abs(v-original.Y)<.00001f,"cropped corners and POI sample the original artwork coordinates");
        }
        reject(()=>MapManifestReader.Read("{"+header+",\"TextureBounds\":{}}"),"incomplete texture bounds rejected");
        reject(()=>MapManifestReader.Read("{"+header+","+textureBounds.Replace("\"MinX\":-10","\"MinX\":1")+"}"),"crop outside texture rejected");
        reject(()=>MapManifestReader.Read("{"+header+","+textureBounds.Replace("\"MaxZ\":60","\"MaxZ\":-30")+"}"),"inverted texture bounds rejected");
        const string area="{\"Id\":\"street\",\"MinX\":1,\"MaxX\":4,\"MinZ\":2,\"MaxZ\":6,\"RequiredExpansions\":[9007199254740993]}";
        var staged=MapManifestReader.Read("{"+header+",\"Areas\":["+area+"]}");
        check(staged.Areas[0].IsLocked(new long[0]),"new save masks locked district");
        check(!staged.Areas[0].IsLocked(new[]{9007199254740993L}),"network unlock reveals district with full ID precision");
        check(staged.Areas[0].IsLocked(null),"unavailable state never reveals district");
        check(staged.Areas[0].IsLocked(new[]{9007199254740992L}),"different expansion does not reveal district");
        var portal=MapManifestReader.Read("{"+header+",\"Pois\":["+point.Replace("\"X\":2.5","\"Icon\":\"portal\",\"RequiredExpansions\":[1],\"ExcludedExpansions\":[2],\"X\":2.5")+"]}").Pois[0];
        check(!portal.IsVisible(new long[0])&&portal.IsVisible(new[]{1L})&&!portal.IsVisible(new[]{1L,2L}),"POIs follow both enable and disable ancestry");
        check(!portal.IsVisible(null),"POI visibility waits for state");
        reject(()=>MapManifestReader.Read("{"+header+",\"Areas\":["+area+","+area+"]}"),"duplicate area IDs rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"Areas\":["+area.Replace("\"MaxX\":4","\"MaxX\":40")+"]}"),"foreign region area rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"Areas\":["+area.Replace("[9007199254740993]","[]")+"]}"),"unconditional lock rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"DetailTexture\":\"detail.png\",\"DetailMinX\":0,\"DetailMaxX\":20,\"DetailMinZ\":0,\"DetailMaxZ\":10}"),"detail outside map rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"Pois\":["+point.Replace("\"X\":2.5","\"Icon\":\"../private\",\"X\":2.5")+"]}"),"unknown icon rejected");
        var generic=MapManifestReader.Read("{"+header+",\"Pois\":["+point.Replace(",\"Category\":\"shop\"","")+"]}");
        check(generic.Pois[0].Category=="other","omitted category retains generic icon default");
        reject(()=>MapManifestReader.Read("{"+header+",\"Pois\":["+point+","+point+"]}"),"duplicate parsed POIs rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"Pois\":["+point.Replace("2.5","25")+"]}"),"out-of-bounds parsed POIs rejected");
        reject(()=>MapManifestReader.Read("{"+header+",\"Pois\":[}"),"malformed POI array rejected");
        reject(()=>MapManifestReader.Read(new string(' ',32769)),"oversized metadata rejected");
    }
}
