using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OldMarket.Navigation
{
    /// <summary>Upright place symbols in the circular minimap, independent of terrain rotation.</summary>
    public sealed class MiniMapPoiLayer : IDisposable
    {
        private sealed class Node
        {
            public MapPoi Poi;
            public RectTransform Root, Plate;
            public Image Icon;
            public TextMeshProUGUI Label;
            public Vector2 Position;
            public float Width;
            public bool Visible, LabelVisible;
            public int Side;
        }
        private readonly NavigationState state;
        private readonly RectTransform root;
        private readonly PoiIconSet icons=new PoiIconSet();
        private readonly List<Node> nodes=new List<Node>();
        private readonly List<HudBox> occupied=new List<HudBox>();
        private MapDefinition definition;
        public int NodeCount => nodes.Count;
        public int InViewCount { get; private set; }
        private static readonly Color Gold=new Color(1,.78f,.27f,1), Cream=new Color(1,.95f,.80f,1);

        public MiniMapPoiLayer(NavigationState state,RectTransform parent)
        {
            this.state=state;root=Rect("MinimapPlaces",parent,Vector2.zero);
        }
        private static RectTransform Rect(string name,Transform parent,Vector2 size)
        {
            var r=(RectTransform)new GameObject(name,typeof(RectTransform)).transform;r.SetParent(parent,false);
            r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.sizeDelta=size;return r;
        }
        private void Clear()
        {
            foreach(var node in nodes){node.Root.gameObject.SetActive(false);UnityEngine.Object.Destroy(node.Root.gameObject);}
            nodes.Clear();
        }
        private void Rebuild()
        {
            Clear();definition=state.Map;if(definition==null)return;
            foreach(var poi in definition.Pois)
            {
                var node=new Node{Poi=poi};node.Root=Rect("MiniPOI_"+poi.Id,root,new Vector2(25,25));
                node.Icon=node.Root.gameObject.AddComponent<Image>();node.Icon.sprite=icons.Get(poi);node.Icon.raycastTarget=true;
                BindPointer(node.Root,"poi:"+poi.Id);
                node.Plate=Rect("Name",node.Root,new Vector2(90,24));
                var plate=node.Plate.gameObject.AddComponent<Image>();plate.color=new Color(.10f,.075f,.04f,.85f);plate.raycastTarget=true;BindPointer(node.Plate,"poi:"+poi.Id);
                var labelRect=Rect("Text",node.Plate,new Vector2(80,24));node.Label=labelRect.gameObject.AddComponent<TextMeshProUGUI>();
                node.Label.fontSize=14;node.Label.fontSizeMax=14;node.Label.fontSizeMin=10;node.Label.enableAutoSizing=true;
                node.Label.alignment=TextAlignmentOptions.Center;node.Label.textWrappingMode=TextWrappingModes.NoWrap;
                node.Label.overflowMode=TextOverflowModes.Ellipsis;node.Label.richText=false;node.Label.raycastTarget=false;
                node.Plate.gameObject.SetActive(false);nodes.Add(node);
            }
        }
        private void BindPointer(RectTransform node,string id)
        {
            var gesture=node.gameObject.AddComponent<MapGesture>();
            gesture.Click=e=>{if(e.button==PointerEventData.InputButton.Left||e.button==PointerEventData.InputButton.Right)state.TargetId=id;};
        }
        private Vector2 Project(float x,float z,float scale,float cos,float sin)
        {
            float dx=(x-state.PlayerPosition.x)*scale,dz=(z-state.PlayerPosition.z)*scale;
            return new Vector2(dx*cos-dz*sin,dx*sin+dz*cos);
        }
        private static bool InsideCircle(HudBox box,float radius)
        {
            float x=Mathf.Max(Mathf.Abs(box.X),Mathf.Abs(box.Right));
            float y=Mathf.Max(Mathf.Abs(box.Y),Mathf.Abs(box.Top));
            return x*x+y*y<=radius*radius;
        }
        public void Refresh(float size,float range,float rotation)
        {
            if(definition!=state.Map)Rebuild();
            root.gameObject.SetActive(definition!=null&&definition.Valid);
            if(definition==null||!definition.Valid)return;
            float radius=(size-12)/2,scale=radius/range;
            float sin=Mathf.Sin(rotation*Mathf.Deg2Rad),cos=Mathf.Cos(rotation*Mathf.Deg2Rad);
            InViewCount=0;occupied.Clear();occupied.Add(new HudBox(-17,-17,34,34));
            foreach(var marker in state.Markers)
            {
                var p=Project(marker.X,marker.Z,scale,cos,sin);occupied.Add(new HudBox(p.x-13,p.y-13,26,26));
            }
            foreach(var node in nodes)
            {
                node.Position=Project(node.Poi.X,node.Poi.Z,scale,cos,sin);
                node.Root.anchoredPosition=node.Position;
                node.Visible=node.Position.x*node.Position.x+node.Position.y*node.Position.y<=(radius-18)*(radius-18);
                node.Root.gameObject.SetActive(node.Visible);
                if(!node.Visible){node.LabelVisible=false;node.Plate.gameObject.SetActive(false);continue;}
                InViewCount++;
                occupied.Add(new HudBox(node.Position.x-12.5f,node.Position.y-12.5f,25,25));
                bool target=state.TargetId=="poi:"+node.Poi.Id;node.Icon.color=Color.white;node.Label.color=target?Gold:Cream;
                string name=node.Poi.DisplayName(state.Locale);
                if(node.Label.text!=name)
                {
                    node.Label.text=name;float width=14;foreach(char c in name)width+=c>255?14:8;
                    node.Width=Mathf.Clamp(width,42,132);node.Plate.sizeDelta=new Vector2(node.Width,24);node.Label.rectTransform.sizeDelta=new Vector2(node.Width-8,24);
                }
                if(state.Font!=null&&node.Label.font!=state.Font)node.Label.font=state.Font;
            }
            nodes.Sort((a,b)=>
            {
                bool at=state.TargetId=="poi:"+a.Poi.Id,bt=state.TargetId=="poi:"+b.Poi.Id;
                return at!=bt?(at?-1:1):string.CompareOrdinal(a.Poi.Id,b.Poi.Id);
            });
            var bounds=new HudBox(-radius,-radius,radius*2,radius*2);
            foreach(var node in nodes)
            {
                if(!node.Visible)continue;
                bool show=false,target=state.TargetId=="poi:"+node.Poi.Id;
                if(!string.IsNullOrEmpty(node.Label.text)&&PoiLabelLayout.WantsLabel(100/range,node.LabelVisible,target))
                {
                    for(int attempt=0;attempt<4;attempt++)
                    {
                        int side=(node.Side+attempt)%4;
                        var candidate=PoiLabelLayout.Candidate(node.Position.x,node.Position.y,node.Width,side);
                        if(!InsideCircle(candidate,radius-3)||!PoiLabelLayout.Fits(candidate,bounds,occupied,node.LabelVisible?2:5))continue;
                        node.Plate.anchoredPosition=new Vector2(candidate.X+candidate.Width/2-node.Position.x,candidate.Y+candidate.Height/2-node.Position.y);
                        occupied.Add(candidate);node.Side=side;show=true;break;
                    }
                }
                if(show!=node.LabelVisible){node.Plate.gameObject.SetActive(show);node.LabelVisible=show;}
            }
        }
        public void Dispose(){Clear();icons.Dispose();if(root!=null)UnityEngine.Object.Destroy(root.gameObject);}
    }
}
