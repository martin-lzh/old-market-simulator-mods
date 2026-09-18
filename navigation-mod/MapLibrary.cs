using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OldMarket.Navigation
{
    // Map files ship alongside the plugin DLL in its maps directory.
    public sealed class MapLibrary : IDisposable
    {
        private readonly List<(LocalMapManifest manifest, MapDefinition map)> maps = new List<(LocalMapManifest, MapDefinition)>();
        private readonly Dictionary<string,Texture2D> textures = new Dictionary<string,Texture2D>(StringComparer.OrdinalIgnoreCase);
        private string selectionKey;
        private MapDefinition selected;
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
                    var m = MapManifestReader.Read(File.ReadAllText(file));
                    if (m == null || !NavMath.ValidBounds(m.MinX,m.MaxX,m.MinZ,m.MaxZ) || string.IsNullOrWhiteSpace(m.Id)
                        || string.IsNullOrWhiteSpace(m.Texture) || Path.GetFileName(m.Texture) != m.Texture || m.North != "+Z")
                        throw new InvalidDataException("Map metadata invalid");
                    var image=LoadTexture(directory,m.Texture);
                    var overlay=string.IsNullOrWhiteSpace(m.OverlayTexture)?null:LoadTexture(directory,m.OverlayTexture);
                    var detail=string.IsNullOrWhiteSpace(m.DetailTexture)?null:LoadTexture(directory,m.DetailTexture);
                    var pois=MapPoi.Validate(m.Pois,m.MinX,m.MaxX,m.MinZ,m.MaxZ);
                    maps.Add((m,new MapDefinition { Id=m.Id,Name=m.Name,NameTextKey=m.NameTextKey,Texture=image,OverlayTexture=overlay,DetailTexture=detail,
                        DetailMinX=m.DetailMinX,DetailMaxX=m.DetailMaxX,DetailMinZ=m.DetailMinZ,DetailMaxZ=m.DetailMaxZ,
                        MinX=m.MinX,MaxX=m.MaxX,MinZ=m.MinZ,MaxZ=m.MaxZ,Pois=pois }));
                    log("Local map loaded: " + m.Id + "; POIs=" + pois.Count);
                }
                catch (Exception error) { log("Local map rejected: " + Path.GetFileName(file) + ": " + error.Message); }
            }
        }
        public MapDefinition Find(int mapId,string scene,string region,IReadOnlyList<long> unlocked)
        {
            if(unlocked==null){selectionKey=null;selected=null;return null;}
            var sorted=new List<long>(unlocked);sorted.Sort();
            string key=mapId+"|"+scene+"|"+region+"|"+string.Join(",",sorted);
            if(key==selectionKey)return selected;
            selectionKey=key;selected=null;
            MapDefinition match=null;

            foreach(var entry in maps)
            {
                var m=entry.manifest;
                if(m.MapId!=mapId || m.SceneName!=scene || (m.Region??"")!=(region??"")) continue;
                bool matches=ExpansionRules.Matches(unlocked,m.RequiredExpansions,m.ExcludedExpansions);
                if(matches)
                {
                    if(match!=null)return null;
                    var source=entry.map;
                    match=new MapDefinition{Id=source.Id,Name=source.Name,Texture=source.Texture,OverlayTexture=source.OverlayTexture,
                        DetailTexture=source.DetailTexture,DetailMinX=source.DetailMinX,DetailMaxX=source.DetailMaxX,DetailMinZ=source.DetailMinZ,DetailMaxZ=source.DetailMaxZ,
                        MinX=source.MinX,MaxX=source.MaxX,MinZ=source.MinZ,MaxZ=source.MaxZ};
                    foreach(var poi in source.Pois)if(poi.IsVisible(unlocked))match.Pois.Add(poi);
                    foreach(var area in m.Areas)if(area.IsLocked(unlocked))match.LockedAreas.Add(area);
                }
            }
            selected=match;return selected;
        }
        public void Dispose() { foreach(var texture in textures.Values) if(texture!=null) UnityEngine.Object.Destroy(texture); textures.Clear();maps.Clear(); }
    }
}
