using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OldMarket.Navigation
{
    [Serializable, DataContract]
    public sealed class MapPoi
    {
        [DataMember] public string Id="";
        [DataMember] public string Name="";
        [DataMember] public string NameKey="";
        [DataMember] public string Category="other";
        [DataMember] public float X;
        [DataMember] public float Z;
        [DataMember] public string Icon;
        [DataMember] public long[] RequiredExpansions;
        [DataMember] public long[] ExcludedExpansions;
        public bool IsVisible(IReadOnlyList<long> unlocked) => ExpansionRules.Matches(unlocked,RequiredExpansions,ExcludedExpansions);
        [OnDeserializing]
        private void SetDefaults(StreamingContext context){Category="other";}
        public string DisplayName(string locale)
        {
            if (string.IsNullOrEmpty(NameKey)) return Name;
            string key=NativeKey(NameKey);
            // Known game POIs stay icon-only until their native text is available.
            // Do not substitute an action or a made-up place name for an unnamed location.
            return key.Length==0 ? "" : NativeNameResolver?.Invoke(locale,key) ?? "";
        }
        // The runtime supplies an asynchronous game-table reader; metadata stays Unity-independent.
        public static Func<string,string,string> NativeNameResolver;
        private static readonly Dictionary<string,string> NativeAliases=new Dictionary<string,string> {
            ["poi_engineer"]="engineer", ["poi_decorations"]="decoration_store",
            ["poi_carpenter"]="lumberjack", ["poi_animals"]="animal_market",
            ["poi_garden"]="gardener", ["poi_clothing"]="clothing_store",
            ["poi_orders"]="orders", ["poi_workshop"]="workshop",
            ["poi_farm"]="farm", ["poi_museum"]="museum",
            ["poi_rest"]="", ["poi_market"]="",
            ["poi_water"]="", ["poi_calendar"]="", ["poi_return"]=""
        };
        public static string NativeKey(string key)
        { return NativeAliases.TryGetValue(key??"",out var native)?native:key??""; }
        public static List<MapPoi> Validate(MapPoi[] points,float minX,float maxX,float minZ,float maxZ)
        {
            var result=new List<MapPoi>();var ids=new HashSet<string>();
            if(points==null)return result;
            if(points.Length>256)throw new ArgumentException("Too many map POIs");
            foreach(var point in points)
            {
                if(point==null||string.IsNullOrWhiteSpace(point.Id)||point.Id.Length>60||!ids.Add(point.Id)
                    ||string.IsNullOrWhiteSpace(point.Name)||point.Name.Length>80||(point.NameKey??"").Length>80
                    ||!NavMath.Finite(point.X)||!NavMath.Finite(point.Z)||point.X<minX||point.X>maxX||point.Z<minZ||point.Z>maxZ
                    ||(!string.IsNullOrEmpty(point.Icon)&&point.Icon!="portal"&&point.Icon!="locked"&&point.Icon!="return"&&point.Icon!="mail")
                    ||(point.RequiredExpansions?.Length??0)>16||(point.ExcludedExpansions?.Length??0)>16
                    ||(point.Category!="home"&&point.Category!="shop"&&point.Category!="dock"&&point.Category!="other"))
                    throw new ArgumentException("Invalid map POI");
                result.Add(point);
            }
            return result;
        }
    }
}
