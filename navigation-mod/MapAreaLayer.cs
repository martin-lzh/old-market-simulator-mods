using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OldMarket.Navigation
{
    /// <summary>Unlock shading shared by both map views; never samples or modifies the game scene.</summary>
    public sealed class MapAreaLayer : IDisposable
    {
        private readonly RectTransform root;
        private readonly List<GameObject> nodes=new List<GameObject>();
        private readonly Texture2D hatch;
        private MapDefinition definition;

        public MapAreaLayer(RectTransform parent)
        {
            root=(RectTransform)new GameObject("ExpansionAreas",typeof(RectTransform)).transform;
            root.SetParent(parent,false);root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;
            root.offsetMin=root.offsetMax=Vector2.zero;
            hatch=new Texture2D(32,32,TextureFormat.RGBA32,false);
            var pixels=new Color32[32*32];
            for(int y=0;y<32;y++)for(int x=0;x<32;x++)
                pixels[y*32+x]=((x+y)%12<3)?new Color32(161,132,89,230):new Color32(51,47,39,225);
            hatch.SetPixels32(pixels);hatch.Apply(false,true);
        }

        public void Refresh(MapDefinition map)
        {
            if(ReferenceEquals(definition,map))return;
            definition=map;
            foreach(var node in nodes){node.SetActive(false);UnityEngine.Object.Destroy(node);}nodes.Clear();
            if(map==null)return;
            if(map.DetailTexture!=null)Add(map,"TownDetail",map.DetailMinX,map.DetailMaxX,map.DetailMinZ,map.DetailMaxZ,map.DetailTexture);
            foreach(var area in map.LockedAreas)
                Add(map,"Locked_"+area.Id,area.MinX,area.MaxX,area.MinZ,area.MaxZ,hatch);
        }

        private void Add(MapDefinition map,string name,float minX,float maxX,float minZ,float maxZ,Texture2D texture)
        {
            var r=(RectTransform)new GameObject(name,typeof(RectTransform)).transform;
            r.SetParent(root,false);
            var low=map.Project(minX,minZ);var high=map.Project(maxX,maxZ);
            r.anchorMin=new Vector2(low.X,low.Y);r.anchorMax=new Vector2(high.X,high.Y);
            r.offsetMin=r.offsetMax=Vector2.zero;
            var image=r.gameObject.AddComponent<RawImage>();image.texture=texture;image.raycastTarget=false;
            nodes.Add(r.gameObject);
        }

        public void Dispose()
        {
            if(root!=null)UnityEngine.Object.Destroy(root.gameObject);
            if(hatch!=null)UnityEngine.Object.Destroy(hatch);
        }
    }
}
