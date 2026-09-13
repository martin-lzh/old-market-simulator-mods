using System;
using System.Collections.Generic;
using UnityEngine;

namespace OldMarket.Navigation
{
    /// <summary>Original semantic sprites shared by both map presentations; no font glyph dependencies.</summary>
    public sealed class PoiIconSet : IDisposable
    {
        private readonly List<UnityEngine.Object> owned=new List<UnityEngine.Object>();
        private readonly Dictionary<string,Sprite> icons=new Dictionary<string,Sprite>();
        public Sprite Get(string category)
        {
            category=category??"other";
            if(!icons.TryGetValue(category,out var sprite)){sprite=CreateIcon(category);icons.Add(category,sprite);}
            return sprite;
        }
        private static Color32 FillColor(string category)
        {
            if(category=="shop")return new Color32(245,170,65,255);
            if(category=="home")return new Color32(125,216,125,255);
            if(category=="dock")return new Color32(90,199,238,255);
            if(category=="player")return new Color32(255,231,152,255);
            return new Color32(205,151,236,255);
        }
        private Sprite CreateIcon(string category)
        {
            const int size=32;var pixels=new Color32[size*size];var mask=new bool[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                bool ink;
                if(category=="player")
                {
                    float dx=Math.Abs(x-15.5f);
                    ink=y>=3&&y<=29&&dx<=(29-y)*.32f&&y>=9-dx*.65f;
                }
                else if(category=="home")
                {
                    bool roof=y>=17&&y<=28&&Math.Abs(x-15.5f)<=29-y;
                    bool walls=x>=7&&x<=24&&y>=5&&y<=18;
                    bool door=x>=13&&x<=18&&y<=14;
                    ink=roof||(walls&&!door);
                }
                else if(category=="shop")
                {
                    bool roof=y>=20&&y<=27&&x>=5+(y-20)/3&&x<=26-(y-20)/3;
                    bool awning=y>=17&&y<=20&&x>=5&&x<=26&&(x/4)%2==0;
                    bool walls=y>=5&&y<=17&&((x>=7&&x<=9)||(x>=23&&x<=25));
                    bool counter=y>=5&&y<=8&&x>=7&&x<=25;
                    ink=roof||awning||walls||counter;
                }
                else if(category=="dock")
                {
                    // Mast, hull and water make a dock/boat sign, with no dependency on a font glyph.
                    ink=(x>=15&&x<=17&&y>=12&&y<=28)||(y>=16&&y<=25&&x>=6&&x<15&&x>=25-y)||
                        (y>=7&&y<=12&&x>=12-y/2&&x<=20+y/2)||(y>=3&&y<=4&&x>=5&&x<=26);
                }
                else
                {
                    float dx=x-15.5f,dy=y-19;
                    ink=(dx*dx+dy*dy<=74&&dx*dx+dy*dy>=16)||(y>=4&&y<14&&Math.Abs(dx)<(y-2)*.55f);
                }
                mask[y*size+x]=ink;
            }
            var fill=FillColor(category);
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                int index=y*size+x;
                if(mask[index]){pixels[index]=fill;continue;}
                bool edge=false;
                for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++)
                {int px=x+dx,py=y+dy;if(px>=0&&py>=0&&px<size&&py<size&&mask[py*size+px])edge=true;}
                pixels[index]=new Color32(15,12,9,(byte)(edge?255:0));
            }
            var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);texture.SetPixels32(pixels);texture.Apply(false,true);owned.Add(texture);
            var sprite=Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f));owned.Add(sprite);return sprite;
        }

        public void Dispose(){foreach(var item in owned)if(item!=null)UnityEngine.Object.Destroy(item);owned.Clear();icons.Clear();}
    }
}