using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OldMarket.Navigation
{
    /// <summary>Read-only companion places, drawn at fixed UI size while the map beneath them zooms.</summary>
    public sealed class MapPoiLayer : IDisposable
    {
        private sealed class Node
        {
            public MapPoi Poi;
            public RectTransform Root, Plate;
            public Image Face, Icon;
            public TextMeshProUGUI Label;
            public Vector2 Position;
            public bool LabelVisible;
            public int Side;
            public float Width=140;
        }
        private readonly NavigationState state;
        private readonly RectTransform mapRect, viewport;
        private readonly Action<string> selectTarget;
        private readonly List<Node> nodes=new List<Node>();
        private readonly List<HudBox> occupied=new List<HudBox>();
        private readonly PoiIconSet icons=new PoiIconSet();
        public int NodeCount => nodes.Count;
        public int InViewCount { get; private set; }
        private IReadOnlyList<HudBox> externalOccupied;
        private MapDefinition definition;
        private static readonly Color Gold=new Color(.96f,.78f,.42f,1), Cream=new Color(.96f,.90f,.73f,1);

        public MapPoiLayer(NavigationState state,RectTransform mapRect,RectTransform viewport,Action<string> selectTarget)
        {
            this.state=state;this.mapRect=mapRect;this.viewport=viewport;this.selectTarget=selectTarget;
        }

        // Rectangles use viewport-local coordinates, centered on the viewport. The caller may provide
        // actual user marker labels so collapsed labels do not reserve unnecessary map space.
        public void SetOccupied(IReadOnlyList<HudBox> rectangles){externalOccupied=rectangles;}

        private static RectTransform Rect(string name,Transform parent,Vector2 size)
        {
            var r=(RectTransform)new GameObject(name,typeof(RectTransform)).transform;r.SetParent(parent,false);
            r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.sizeDelta=size;return r;
        }
        private Vector2 Project(float x,float z)
        {
            var uv=state.Map.Project(x,z);return new Vector2((uv.X-.5f)*mapRect.sizeDelta.x,(uv.Y-.5f)*mapRect.sizeDelta.y);
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
                var node=new Node{Poi=poi};node.Root=Rect("POI_"+poi.Id,mapRect,new Vector2(32,32));
                node.Face=node.Root.gameObject.AddComponent<Image>();
                var icon=Rect("PlaceIcon",node.Root,new Vector2(25,25));node.Icon=icon.gameObject.AddComponent<Image>();
                node.Icon.sprite=icons.Get(poi.Category);
                node.Icon.raycastTarget=false;
                var button=node.Root.gameObject.AddComponent<Button>();string id="poi:"+poi.Id;button.onClick.AddListener(()=>selectTarget?.Invoke(id));
                node.Plate=Rect("PlaceLabel",node.Root,new Vector2(140,30));node.Plate.gameObject.AddComponent<Image>().color=new Color(.105f,.088f,.065f,.96f);
                var labelRect=Rect("Text",node.Plate,new Vector2(126,30));node.Label=labelRect.gameObject.AddComponent<TextMeshProUGUI>();
                node.Label.fontSize=16;node.Label.color=Cream;node.Label.alignment=TextAlignmentOptions.Center;
                node.Label.enableAutoSizing=true;node.Label.fontSizeMin=12;node.Label.fontSizeMax=16;
                node.Label.textWrappingMode=TextWrappingModes.NoWrap;node.Label.overflowMode=TextOverflowModes.Ellipsis;
                node.Label.richText=false;node.Label.raycastTarget=false;
                var labelButton=node.Plate.gameObject.AddComponent<Button>();labelButton.onClick.AddListener(()=>selectTarget?.Invoke(id));
                node.Plate.gameObject.SetActive(false);nodes.Add(node);
            }
        }

        public void Refresh(float zoom)
        {
            if(definition!=state.Map)Rebuild();
            if(definition==null||!definition.Valid)return;
            occupied.Clear();InViewCount=0;
            float width=viewport.rect.width,height=viewport.rect.height;
            var bounds=new HudBox(-width/2,-height/2,width,height);
            // Keep north and zoom/center controls readable even when a place moves underneath them.
            occupied.Add(new HudBox(bounds.X+10,bounds.Top-106,76,96));
            occupied.Add(new HudBox(bounds.X+10,bounds.Y+10,176,44));
            occupied.Add(new HudBox(bounds.Right-112,bounds.Y+10,104,44));
            if(state.Available)
            {
                var player=Project(state.PlayerPosition.x,state.PlayerPosition.z)+mapRect.anchoredPosition;
                occupied.Add(new HudBox(player.x-21,player.y-21,42,42));
            }
            if(externalOccupied!=null)for(int i=0;i<externalOccupied.Count;i++)occupied.Add(externalOccupied[i]);
            else foreach(var marker in state.Markers)
            {
                var position=Project(marker.X,marker.Z)+mapRect.anchoredPosition;
                occupied.Add(new HudBox(position.x-18,position.y-18,36,36));
                if(zoom>=1.5f||state.TargetId==marker.Id)occupied.Add(new HudBox(position.x+22,position.y-16,144,32));
            }
            foreach(var node in nodes)
            {
                node.Root.anchoredPosition=Project(node.Poi.X,node.Poi.Z);
                node.Position=node.Root.anchoredPosition+mapRect.anchoredPosition;
                occupied.Add(new HudBox(node.Position.x-16,node.Position.y-16,32,32));
                string name=node.Poi.DisplayName(state.Locale);
                if(node.Label.text!=name)
                {
                    node.Label.text=name;float measured=18;
                    foreach(char c in name)measured+=c>255?16:9;
                    node.Width=Mathf.Clamp(measured,64,200);
                    node.Plate.sizeDelta=new Vector2(node.Width,30);node.Label.rectTransform.sizeDelta=new Vector2(node.Width-12,30);
                }
                if(state.Font!=null&&node.Label.font!=state.Font)node.Label.font=state.Font;
                bool target=state.TargetId=="poi:"+node.Poi.Id;
                node.Face.color=Color.clear;
                node.Icon.color=Color.white;node.Label.color=target?Gold:Cream;
            }
            // A target gets the first free label slot; remaining places keep deterministic ID order.
            nodes.Sort((a,b)=>
            {
                bool at=state.TargetId=="poi:"+a.Poi.Id,bt=state.TargetId=="poi:"+b.Poi.Id;
                return at!=bt?(at?-1:1):string.CompareOrdinal(a.Poi.Id,b.Poi.Id);
            });
            foreach(var node in nodes)
            {
                bool target=state.TargetId=="poi:"+node.Poi.Id;
                bool onScreen=node.Position.x>=bounds.X&&node.Position.x<=bounds.Right&&node.Position.y>=bounds.Y&&node.Position.y<=bounds.Top;
                if(onScreen)InViewCount++;
                bool show=onScreen&&!string.IsNullOrEmpty(node.Label.text)&&PoiLabelLayout.WantsLabel(zoom,node.LabelVisible,target);
                if(show&&PoiLabelLayout.TryPlace(node.Position.x,node.Position.y,node.Width,bounds,occupied,node.LabelVisible,ref node.Side,out var placed))
                {
                    node.Plate.anchoredPosition=new Vector2(placed.X+placed.Width/2-node.Position.x,placed.Y+placed.Height/2-node.Position.y);
                    occupied.Add(placed);
                }
                else show=false;
                if(show!=node.LabelVisible){node.Plate.gameObject.SetActive(show);node.LabelVisible=show;}
            }
        }

        public void Dispose(){Clear();icons.Dispose();}
    }
}
