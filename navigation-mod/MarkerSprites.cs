using System;
using System.Collections.Generic;
using UnityEngine;

namespace OldMarket.Navigation
{
    // Personal marker and editor glyphs do not depend on native font coverage.
    public sealed class MarkerSprites : IDisposable
    {
        private readonly Dictionary<string,Sprite> cache=new Dictionary<string,Sprite>();
        private readonly List<UnityEngine.Object> owned=new List<UnityEngine.Object>();
        public Sprite Get(int icon,int color)=>Create("marker"+icon+":"+color,icon,MapWindow.MarkerColors[Mathf.Clamp(color,0,3)]);
        public Sprite Action(string action)=>Create(action,action=="delete"?3:action=="color"?4:1,Color.white);
        private Sprite Create(string key,int shape,Color color)
        {
            if(cache.TryGetValue(key,out var cached))return cached;
            const int size=32;var mask=new bool[size*size];var pixels=new Color32[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                float dx=Math.Abs(x-15.5f),dy=Math.Abs(y-15.5f);
                bool ink=shape==0?y>=5&&y<=26&&dx<=(y-4)*.48f:
                    shape==1?dx+dy<=11:shape==2||shape==4?dx*dx+dy*dy<=115:
                    (y>=23&&y<=25&&x>=5&&x<=26)||(y>=26&&y<=28&&x>=12&&x<=19)||
                    (y>=5&&y<=22&&x>=8&&x<=23&&!(y>=8&&y<=20&&(x==12||x==13||x==18||x==19)));
                mask[y*size+x]=ink;
            }
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                int index=y*size+x;
                if(mask[index]){pixels[index]=shape==4?(Color32)MapWindow.MarkerColors[(x<16?0:1)+(y<16?0:2)]:(Color32)color;continue;}
                bool edge=false;for(int dy=-3;dy<=3;dy++)for(int dx=-3;dx<=3;dx++)
                {int px=x+dx,py=y+dy;if(px>=0&&py>=0&&px<size&&py<size&&mask[py*size+px])edge=true;}
                pixels[index]=new Color32(15,12,9,(byte)(edge?255:0));
            }
            var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);texture.SetPixels32(pixels);texture.Apply(false,true);owned.Add(texture);
            var sprite=Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f));owned.Add(sprite);cache.Add(key,sprite);return sprite;
        }
        public void Dispose(){foreach(var item in owned)if(item!=null)UnityEngine.Object.Destroy(item);owned.Clear();cache.Clear();}
    }
}
