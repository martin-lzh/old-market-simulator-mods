using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace OldMarket.Navigation
{
    [DataContract]
    public sealed class MapTextureBounds
    {
        [DataMember(IsRequired=true)] public float MinX;
        [DataMember(IsRequired=true)] public float MaxX;
        [DataMember(IsRequired=true)] public float MinZ;
        [DataMember(IsRequired=true)] public float MaxZ;
        public MapPoint Project(float x,float z)=>NavMath.WorldToUv(x,z,MinX,MaxX,MinZ,MaxZ);
    }

    [DataContract]
    public sealed class LocalMapManifest
    {
        [DataMember] public string Id;
        [DataMember] public string Name;
        [DataMember] public string NameTextKey;
        [DataMember] public string SceneName;
        [DataMember] public string Region="";
        [DataMember] public string Texture;
        [DataMember] public string OverlayTexture;
        // Base/overlay artwork may cover more ground than the displayed map crop.
        [DataMember] public MapTextureBounds TextureBounds;
        [DataMember] public int MapId;
        [DataMember] public float MinX;
        [DataMember] public float MaxX;
        [DataMember] public float MinZ;
        [DataMember] public float MaxZ;
        [DataMember] public string North="+Z";
        [DataMember] public long[] RequiredExpansions=new long[0];
        [DataMember] public long[] ExcludedExpansions=new long[0];
        [DataMember] public MapPoi[] Pois=new MapPoi[0];
        [DataMember] public MapArea[] Areas=new MapArea[0];
        [DataMember] public string DetailTexture;
        [DataMember] public float DetailMinX,DetailMaxX,DetailMinZ,DetailMaxZ;
        public MapPoint TextureUvMin=>TextureBounds==null?new MapPoint(0,0):TextureBounds.Project(MinX,MinZ);
        public MapPoint TextureUvMax=>TextureBounds==null?new MapPoint(1,1):TextureBounds.Project(MaxX,MaxZ);
        [OnDeserializing]
        private void SetDefaults(StreamingContext context){North="+Z";}
    }

    public static class MapManifestReader
    {
        // The same metadata reader runs in the plugin and in tests, without Unity native serialization.
        public static LocalMapManifest Read(string json)
        {
            if(json==null||Encoding.UTF8.GetByteCount(json)>32768)throw new InvalidDataException("Map metadata too large");
            var serializer=new DataContractJsonSerializer(typeof(LocalMapManifest));
            using(var stream=new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var map=(LocalMapManifest)serializer.ReadObject(stream);
                if(map==null)throw new InvalidDataException("Map metadata missing");
                map.Pois=map.Pois??new MapPoi[0];map.Region=map.Region??"";
                map.RequiredExpansions=map.RequiredExpansions??new long[0];map.ExcludedExpansions=map.ExcludedExpansions??new long[0];
                map.Areas=map.Areas??new MapArea[0];
                var bounds=map.TextureBounds;
                if(bounds!=null&&(!NavMath.ValidBounds(bounds.MinX,bounds.MaxX,bounds.MinZ,bounds.MaxZ)
                    ||!NavMath.ValidBounds(map.MinX,map.MaxX,map.MinZ,map.MaxZ)
                    ||map.MinX<bounds.MinX||map.MaxX>bounds.MaxX||map.MinZ<bounds.MinZ||map.MaxZ>bounds.MaxZ))
                    throw new InvalidDataException("Map texture bounds invalid");
                MapArea.Validate(map.Areas,map.MinX,map.MaxX,map.MinZ,map.MaxZ);
                if(!string.IsNullOrEmpty(map.DetailTexture)&&(!NavMath.ValidBounds(map.DetailMinX,map.DetailMaxX,map.DetailMinZ,map.DetailMaxZ)
                    ||map.DetailMinX<map.MinX||map.DetailMaxX>map.MaxX||map.DetailMinZ<map.MinZ||map.DetailMaxZ>map.MaxZ))
                    throw new InvalidDataException("Map detail bounds invalid");
                MapPoi.Validate(map.Pois,map.MinX,map.MaxX,map.MinZ,map.MaxZ);
                return map;
            }
        }
    }
}
