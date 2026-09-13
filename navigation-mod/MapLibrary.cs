using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OldMarket.Navigation
{
    [Serializable]
    public sealed class LocalMapManifest
    {
        public string Id, Name, SceneName, Region = "", Texture, OverlayTexture;
        public int MapId;
        public float MinX, MaxX, MinZ, MaxZ;
        public string North = "+Z";
        public long[] RequiredExpansions = new long[0], ExcludedExpansions = new long[0];
        public MapPoi[] Pois = new MapPoi[0];
    }

    // Local generated maps remain outside the public DLL and public art assets.
    public sealed class MapLibrary : IDisposable
    {
        private readonly List<(LocalMapManifest manifest, MapDefinition map)> maps = new List<(LocalMapManifest, MapDefinition)>();
        private readonly Dictionary<string,Texture2D> textures = new Dictionary<string,Texture2D>(StringComparer.OrdinalIgnoreCase);
        private Texture2D LoadTexture(string directory,string name)
        {
            if(string.IsNullOrWhiteSpace(name) || Path.GetFileName(name)!=name || Path.IsPathRooted(name)) throw new InvalidDataException("Map texture filename invalid");
            string path=Path.Combine(directory,name);
            if(textures.TryGetValue(path,out var cached)) return cached;
            if(new FileInfo(path).Length>32*1024*1024) throw new InvalidDataException("Map image too large");
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
            try
            {
                if(!ImageConversion.LoadImage(texture,File.ReadAllBytes(path),true)) throw new InvalidDataException("Map image invalid");
                texture.wrapMode=TextureWrapMode.Clamp;texture.filterMode=FilterMode.Bilinear;
                textures.Add(path,texture);
                return texture;
            }
            catch { UnityEngine.Object.Destroy(texture);throw; }
        }
        public MapLibrary(string directory, Action<string> log)
        {
            if (!Directory.Exists(directory)) return;
            foreach (string file in Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    if (new FileInfo(file).Length > 32768) throw new InvalidDataException("Map metadata too large");
                    var m = JsonUtility.FromJson<LocalMapManifest>(File.ReadAllText(file));
                    if (m == null || !NavMath.ValidBounds(m.MinX,m.MaxX,m.MinZ,m.MaxZ) || string.IsNullOrWhiteSpace(m.Id)
                        || string.IsNullOrWhiteSpace(m.Texture) || Path.GetFileName(m.Texture) != m.Texture || m.North != "+Z")
                        throw new InvalidDataException("Map metadata invalid");
                    var image=LoadTexture(directory,m.Texture);
                    var overlay=string.IsNullOrWhiteSpace(m.OverlayTexture)?null:LoadTexture(directory,m.OverlayTexture);
                    var pois=MapPoi.Validate(m.Pois,m.MinX,m.MaxX,m.MinZ,m.MaxZ);
                    maps.Add((m,new MapDefinition { Id=m.Id,Name=m.Name,Texture=image,OverlayTexture=overlay,MinX=m.MinX,MaxX=m.MaxX,MinZ=m.MinZ,MaxZ=m.MaxZ,Pois=pois }));
                    log("Local map loaded: " + m.Id);
                }
                catch (Exception error) { log("Local map rejected: " + Path.GetFileName(file) + ": " + error.Message); }
            }
        }
        public MapDefinition Find(int mapId,string scene,string region,IReadOnlyList<long> unlocked)
        {
            MapDefinition match=null;

            foreach(var entry in maps)
            {
                var m=entry.manifest;
                if(m.MapId!=mapId || m.SceneName!=scene || (m.Region??"")!=(region??"")) continue;
                bool matches=ExpansionRules.Matches(unlocked,m.RequiredExpansions,m.ExcludedExpansions);
                if(matches) { if(match!=null)return null;match=entry.map; }
            }
            return match;
        }
        public void Dispose() { foreach(var texture in textures.Values) if(texture!=null) UnityEngine.Object.Destroy(texture); textures.Clear();maps.Clear(); }
    }
}
