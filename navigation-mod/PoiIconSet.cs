using System;
using System.Collections.Generic;
using UnityEngine;

namespace OldMarket.Navigation
{
    /// <summary>Phosphor filled semantic place sprites and the original player arrow; no font glyph dependencies.</summary>
    public sealed class PoiIconSet : IDisposable
    {
        private readonly List<UnityEngine.Object> owned=new List<UnityEngine.Object>();
        private readonly Dictionary<string,Sprite> icons=new Dictionary<string,Sprite>();
        public Sprite Get(string category) => GetIcon(category=="home"?"house":category=="shop"?"storefront":category=="dock"?"boat":"map-pin",category);
        public Sprite Get(MapPoi poi)
        {
            if(poi==null)return Get("other");
            string key=poi.NameKey??"", native=MapPoi.NativeKey(key), name;
            if(key=="poi_rest"||native=="rest")name="bed";
            else if(key=="poi_market"||native=="market")name="storefront";
            else switch(native)
            {
                case "farm":name="barn";break;
                case "museum":name="bank";break;
                case "workshop":name="hammer";break;
                case "engineer":name="wrench";break;
                case "decoration_store":name="armchair";break;
                case "lumberjack":name="axe";break;
                case "animal_market":name="cow";break;
                case "gardener":name="plant";break;
                case "clothing_store":name="t-shirt";break;
                case "orders":name="boat";break;
                case "licenses":name="identification-card";break;
                case "employees":name="users-three";break;
                case "expansions":name="ruler";break;
                case "junkman":name="recycle";break;
                default:return Get(poi.Category);
            }
            return GetIcon(name,poi.Category);
        }
        private Sprite GetIcon(string name,string category)
        {
            category=category??"other";
            if(category!="shop"&&category!="home"&&category!="dock"&&category!="player")category="other";
            string key=category=="player"?"player":name+"-"+category;
            if(icons.TryGetValue(key,out var sprite))return sprite;
            if(category=="player")sprite=CreatePlayerIcon();
            else
            {
                var bytes=UiAssets.Read("poi-"+key+".png");
                if(bytes==null)throw new InvalidOperationException("Missing embedded POI icon: "+key);
                var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
                if(!ImageConversion.LoadImage(texture,bytes,false)){UnityEngine.Object.Destroy(texture);throw new InvalidOperationException("Invalid embedded POI icon: "+key);}
                owned.Add(texture);sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f));owned.Add(sprite);
            }
            icons.Add(key,sprite);return sprite;
        }
        private Sprite CreatePlayerIcon()
        {
            const int size=32;var pixels=new Color32[size*size];var mask=new bool[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                float dx=Math.Abs(x-15.5f);
                bool ink=y>=3&&y<=29&&dx<=(29-y)*.32f&&y>=9-dx*.65f;
                mask[y*size+x]=ink;
            }
            var fill=new Color32(193,204,208,255);
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                int index=y*size+x;
                if(mask[index]){pixels[index]=fill;continue;}
                bool edge=false;
                for(int dy=-3;dy<=3;dy++)for(int dx=-3;dx<=3;dx++)
                {int px=x+dx,py=y+dy;if(px>=0&&py>=0&&px<size&&py<size&&mask[py*size+px])edge=true;}
                pixels[index]=new Color32(15,12,9,(byte)(edge?255:0));
            }
            var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);texture.SetPixels32(pixels);texture.Apply(false,true);owned.Add(texture);
            var sprite=Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f));owned.Add(sprite);return sprite;
        }

        public void Dispose(){foreach(var item in owned)if(item!=null)UnityEngine.Object.Destroy(item);owned.Clear();icons.Clear();}
    }
}
