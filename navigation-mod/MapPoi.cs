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
        [OnDeserializing]
        private void SetDefaults(StreamingContext context){Category="other";}
        public string DisplayName(string locale)
        {
            string translated=string.IsNullOrEmpty(NameKey)?"":Texts.Get(locale,NameKey);
            return translated!=""&&translated!=NameKey?translated:Name;
        }
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
                    ||(point.Category!="home"&&point.Category!="shop"&&point.Category!="dock"&&point.Category!="other"))
                    throw new ArgumentException("Invalid map POI");
                result.Add(point);
            }
            return result;
        }
    }
}
