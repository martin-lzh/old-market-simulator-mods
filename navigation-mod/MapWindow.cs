using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace OldMarket.Navigation
{
    public sealed class MapGesture : MonoBehaviour, IPointerClickHandler, IDragHandler, IScrollHandler
    {
        public Action<PointerEventData> Click, Drag, Scroll;
        public void OnPointerClick(PointerEventData e) => Click?.Invoke(e);
        public void OnDrag(PointerEventData e) { if(e.button==PointerEventData.InputButton.Left) Drag?.Invoke(e); }
        public void OnScroll(PointerEventData e) => Scroll?.Invoke(e);
    }
    public sealed class MapWindow : IDisposable
    {
        private readonly NavigationState state;
        private readonly RectTransform root, viewport, mapRect, player, playerNamePlate, sidebar, sidebarContent;
        private readonly RawImage mapImage, mapOverlay;
        private readonly TMP_Text title, help, status, north;
        private TMP_Text rotationLabel, targetInfo;
        private readonly List<Tuple<NavMarker,RectTransform>> markerNodes=new List<Tuple<NavMarker,RectTransform>>();
        private readonly List<Tuple<NavMarker,TMP_Text,string>> markerNames=new List<Tuple<NavMarker,TMP_Text,string>>();
        private readonly List<Tuple<TMP_Text,string>> labels=new List<Tuple<TMP_Text,string>>();
        private readonly List<TMP_Text> allText=new List<TMP_Text>();
        private NavigationLayout appliedLayout=new NavigationLayout();
        private Vector2 lastParentSize, pan;
        private string selected="", lastLocale="", lastScope="";
        private float zoom=1, targetZoom=1, lastZoomTime;
        private Vector2 zoomAnchor;
        private int listOffset, lastCount=-1;
        private TMP_InputField nameInput;
        private Texture2D frameTexture, paperTexture;
        private Sprite frameSprite;
        public bool IsOpen => root!=null && root.gameObject.activeSelf;
        private static readonly Color Gold=new Color(.86f,.71f,.43f,1), Cream=new Color(.94f,.88f,.72f,1);
        private static readonly Color Dark=new Color(.115f,.092f,.071f,.98f), Panel=new Color(.17f,.135f,.095f,.98f);
        public static readonly Color[] MarkerColors={new Color(1,.77f,.25f),new Color(.35f,.8f,.95f),new Color(.95f,.4f,.35f),new Color(.45f,.9f,.55f)};
        public static string MarkerSymbol(int icon) => icon==1?"◆":icon==2?"●":"▼";
        public MapWindow(NavigationState state,RectTransform parent)
        {
            this.state=state;
            root=Rect("NavigationMapWindow",parent);
            root.gameObject.AddComponent<Image>().color=Gold;
            var backing=Fill("DarkBacking",root,2);backing.gameObject.AddComponent<Image>().color=Dark;
            var paperBytes=UiAssets.Read("parchment.png");
            if(paperBytes!=null){paperTexture=new Texture2D(2,2,TextureFormat.RGBA32,false);ImageConversion.LoadImage(paperTexture,paperBytes);}
            var bytes=UiAssets.Read("map-frame.png");
            if(bytes!=null)
            {
                frameTexture=new Texture2D(2,2,TextureFormat.RGBA32,false);
                if(ImageConversion.LoadImage(frameTexture,bytes))
                {
                    frameSprite=Sprite.Create(frameTexture,new Rect(0,0,frameTexture.width,frameTexture.height),new Vector2(.5f,.5f),400,0,SpriteMeshType.FullRect,new Vector4(90,90,90,90));
                    var frame=Fill("GoldFrame",root,0);var image=frame.gameObject.AddComponent<Image>();image.sprite=frameSprite;image.type=Image.Type.Sliced;image.raycastTarget=false;
                    image.color=new Color(.75f,.61f,.39f,1);
                }
            }
            title=Label(root,"Map",new Vector2(140,-17),new Vector2(700,40),27);title.alignment=TextAlignmentOptions.Center;
            title.textWrappingMode=TextWrappingModes.NoWrap;
            var close=Button(root,"close",new Vector2(-112,-20),new Vector2(86,34),Close,true);
            Line("HeaderRule",root,new Vector2(22,-68),new Vector2(1010,1));
            viewport=Rect("MapViewport",root);viewport.anchorMin=Vector2.zero;viewport.anchorMax=Vector2.one;
            viewport.offsetMin=new Vector2(22,82);viewport.offsetMax=new Vector2(-282,-84);
            viewport.gameObject.AddComponent<Image>().color=new Color(.29f,.26f,.18f,1);viewport.gameObject.AddComponent<RectMask2D>();
            var paper=Fill("MapParchment",viewport,0);var paperImage=paper.gameObject.AddComponent<RawImage>();paperImage.texture=paperTexture;paperImage.color=new Color(.63f,.58f,.42f,1);paperImage.raycastTarget=false;
            mapRect=Rect("MapTexture",viewport);mapRect.anchorMin=mapRect.anchorMax=new Vector2(.5f,.5f);
            mapImage=mapRect.gameObject.AddComponent<RawImage>();mapImage.raycastTarget=false;
            var overlay=Fill("MapObstacles",mapRect,0);mapOverlay=overlay.gameObject.AddComponent<RawImage>();mapOverlay.raycastTarget=false;mapOverlay.gameObject.SetActive(false);
            var gesture=viewport.gameObject.AddComponent<MapGesture>();gesture.Click=ClickMap;
            gesture.Drag=e=>{targetZoom=zoom;pan+=e.delta/CanvasScale();ApplyMapGeometry();};
            gesture.Scroll=e=>{if(RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport,e.position,e.pressEventCamera,out var anchor))Zoom(Mathf.Pow(1.15f,e.scrollDelta.y),anchor);};
            player=Rect("Player",mapRect);player.sizeDelta=new Vector2(34,34);
            var arrow=player.gameObject.AddComponent<TextMeshProUGUI>();arrow.text="▲";arrow.color=Color.white;arrow.fontSize=32;arrow.alignment=TextAlignmentOptions.Center;arrow.raycastTarget=false;allText.Add(arrow);if(state.Font!=null)arrow.font=state.Font;
            playerNamePlate=Rect("PlayerName",mapRect);playerNamePlate.sizeDelta=new Vector2(132,27);playerNamePlate.gameObject.AddComponent<Image>().color=new Color(.08f,.085f,.07f,.88f);
            playerNamePlate.GetComponent<Image>().raycastTarget=false;
            var playerName=Label(playerNamePlate,"YourPosition",new Vector2(4,-3),new Vector2(124,22),15);playerName.alignment=TextAlignmentOptions.Center;
            var northPlate=Box("NorthPlate",viewport,new Vector2(14,-14),new Vector2(64,88),Dark);
            // Native CJK fonts can have line metrics taller than the glyph: do not ellipsize the direction away.
            north=Label(northPlate,"North",new Vector2(4,-4),new Vector2(56,44),22);north.alignment=TextAlignmentOptions.Center;
            north.textWrappingMode=TextWrappingModes.NoWrap;north.overflowMode=TextOverflowModes.Overflow;north.color=Gold;
            var northArrow=Label(northPlate,"",new Vector2(4,-45),new Vector2(56,38),27);northArrow.text="▲";northArrow.alignment=TextAlignmentOptions.Center;northArrow.color=Gold;
            var plus=Button(viewport,"",new Vector2(-104,16),new Vector2(38,36),()=>Zoom(1.25f,Vector2.zero),true,false);SetButtonSymbol(plus,"+");
            var minus=Button(viewport,"",new Vector2(-56,16),new Vector2(38,36),()=>Zoom(.8f,Vector2.zero),true,false);SetButtonSymbol(minus,"-");
            Button(viewport,"center",new Vector2(16,16),new Vector2(164,36),Center,false,false);
            sidebar=Rect("MarkerSidebar",root);sidebar.anchorMin=new Vector2(1,0);sidebar.anchorMax=Vector2.one;sidebar.pivot=new Vector2(1,.5f);sidebar.offsetMin=new Vector2(-264,82);sidebar.offsetMax=new Vector2(-22,-84);
            sidebar.gameObject.AddComponent<Image>().color=Panel;
            sidebarContent=Fill("SidebarContent",sidebar,12);
            help=Label(root,"help",new Vector2(24,66),new Vector2(800,28),14,false);help.color=new Color(.79f,.73f,.61f,1);
            status=Label(root,"",new Vector2(24,31),new Vector2(800,23),14,false);status.color=new Color(1,.77f,.5f,1);
            Button(root,"NorthUp",new Vector2(-264,23),new Vector2(242,35),()=>{state.RotateWithCamera=!state.RotateWithCamera;state.RotationChanged?.Invoke(state.RotateWithCamera);},true,false);
            rotationLabel=labels[labels.Count-1].Item1;
            ApplyLayout(new NavigationLayout());root.gameObject.SetActive(false);
        }
        private float CanvasScale(){var canvas=root.GetComponentInParent<Canvas>();return canvas==null?1:Mathf.Max(.1f,canvas.scaleFactor*root.localScale.x);}
        private static RectTransform Rect(string name,Transform parent){var rect=(RectTransform)new GameObject(name,typeof(RectTransform)).transform;rect.SetParent(parent,false);return rect;}
        private static RectTransform Fill(string name,RectTransform parent,float inset){var r=Rect(name,parent);r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=Vector2.one*inset;r.offsetMax=Vector2.one*-inset;return r;}
        private static RectTransform Box(string name,RectTransform parent,Vector2 position,Vector2 size,Color color)
        {var r=Rect(name,parent);r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=position;r.sizeDelta=size;r.gameObject.AddComponent<Image>().color=color;return r;}
        private static void Line(string name,RectTransform parent,Vector2 position,Vector2 size){var r=Box(name,parent,position,size,new Color(.62f,.48f,.27f,.65f));r.GetComponent<Image>().raycastTarget=false;}
        private TMP_Text Label(RectTransform parent,string key,Vector2 position,Vector2 size,float font,bool top=true)
        {
            var r=Rect(key,parent);r.anchorMin=r.anchorMax=new Vector2(0,top?1:0);r.pivot=new Vector2(0,1);r.anchoredPosition=position;r.sizeDelta=size;
            var label=r.gameObject.AddComponent<TextMeshProUGUI>();label.fontSize=font;label.color=Cream;label.raycastTarget=false;label.textWrappingMode=TextWrappingModes.Normal;label.richText=false;label.overflowMode=TextOverflowModes.Ellipsis;
            if(state.Font!=null)label.font=state.Font;allText.Add(label);
            if(key!=""){labels.Add(Tuple.Create((TMP_Text)label,key));label.text=Texts.Get(state.Locale,key);}return label;
        }
        private RectTransform Button(RectTransform parent,string key,Vector2 position,Vector2 size,Action action,bool right=false,bool top=true)
        {
            var r=Rect(key,parent);r.anchorMin=r.anchorMax=new Vector2(right?1:0,top?1:0);r.pivot=new Vector2(0,top?1:0);r.anchoredPosition=position;r.sizeDelta=size;
            r.gameObject.AddComponent<Image>().color=new Color(.58f,.45f,.26f,1);var inset=Fill("ButtonFace",r,1);inset.gameObject.AddComponent<Image>().color=new Color(.20f,.16f,.11f,.98f);inset.GetComponent<Image>().raycastTarget=false;var b=r.gameObject.AddComponent<UnityEngine.UI.Button>();b.onClick.AddListener(()=>action());
            var label=Label(r,key,new Vector2(5,-3),size-new Vector2(10,6),17);label.alignment=TextAlignmentOptions.Center;label.enableAutoSizing=true;label.fontSizeMin=12;label.fontSizeMax=17;label.color=Gold;
            return r;
        }
        private static void SetButtonSymbol(RectTransform button,string symbol){var label=button.GetComponentInChildren<TMP_Text>();label.text=symbol;label.fontSizeMax=25;}
        public void Toggle(){if(IsOpen)Close();else{root.gameObject.SetActive(true);lastZoomTime=Time.unscaledTime;Refresh();RebuildSidebar();Center();}}
        public void Close(){if(root!=null)root.gameObject.SetActive(false);}
        public void Refresh()
        {
            if(!IsOpen)return;
            if(((RectTransform)root.parent).rect.size!=lastParentSize)ApplyLayout(appliedLayout);
            if(state.Font!=null)foreach(var text in allText)if(text!=null)text.font=state.Font;
            if(lastScope!=state.ScopeId){lastScope=state.ScopeId;selected="";zoom=targetZoom=1;pan=Vector2.zero;listOffset=0;RebuildSidebar();}
            if(lastLocale!=state.Locale){lastLocale=state.Locale;RebuildSidebar();foreach(var label in labels)if(label.Item1!=null)label.Item1.text=Texts.Get(state.Locale,label.Item2);}
            if(lastCount!=state.Markers.Count)RebuildSidebar();
            rotationLabel.text=Texts.Get(state.Locale,state.RotateWithCamera?"CameraUp":"NorthUp");
            title.text=string.IsNullOrWhiteSpace(state.Map?.Name)?Texts.Get(state.Locale,"Map"):state.Map.Name;
            bool valid=state.Map!=null&&state.Map.Valid;mapImage.texture=state.Map?.Texture;mapRect.gameObject.SetActive(valid);
            mapOverlay.texture=state.Map?.OverlayTexture;mapOverlay.uvRect=mapImage.uvRect;mapOverlay.gameObject.SetActive(valid&&mapOverlay.texture!=null);
            status.text=state.ErrorKey!=""?Texts.Get(state.Locale,state.ErrorKey):!valid?Texts.Get(state.Locale,"NoMap"):(string.IsNullOrEmpty(state.ScopeId)||state.Store==null)?Texts.Get(state.Locale,"temporary"):"";
            AnimateZoom();ApplyMapGeometry();
            if(valid){var uv=state.Map.Project(state.PlayerPosition.x,state.PlayerPosition.z);player.anchoredPosition=new Vector2((uv.X-.5f)*mapRect.sizeDelta.x,(uv.Y-.5f)*mapRect.sizeDelta.y);player.localEulerAngles=new Vector3(0,0,-state.CameraYaw);player.gameObject.SetActive(state.Available);playerNamePlate.gameObject.SetActive(state.Available);playerNamePlate.anchoredPosition=player.anchoredPosition+new Vector2(0,-32);player.SetAsLastSibling();playerNamePlate.SetAsLastSibling();}
            foreach(var entry in markerNames)if(entry.Item2!=null)entry.Item2.text=entry.Item3+entry.Item1.Name;
            if(targetInfo!=null){var target=state.Target;if(target==null)targetInfo.text=Texts.Get(state.Locale,"NoTarget");else{float dx=target.X-state.PlayerPosition.x,dz=target.Z-state.PlayerPosition.z;targetInfo.text=MarkerSymbol(target.Icon)+" "+target.Name+"\n"+Mathf.Sqrt(dx*dx+dz*dz).ToString("F0")+" m";}}
        }
        private MapViewportGeometry Geometry(float scale)=>new MapViewportGeometry(viewport.rect.width,viewport.rect.height,(state.Map.MaxX-state.Map.MinX)/(state.Map.MaxZ-state.Map.MinZ),scale);
        private void ApplyMapGeometry()
        {
            if(state.Map==null||!state.Map.Valid||viewport.rect.width<=0||viewport.rect.height<=0)return;
            var geometry=Geometry(zoom);mapRect.sizeDelta=new Vector2(geometry.Width,geometry.Height);var bounded=geometry.ClampPan(new MapPoint(pan.x,pan.y));pan=new Vector2(bounded.X,bounded.Y);mapRect.anchoredPosition=pan;
            foreach(var node in markerNodes)if(node.Item2!=null){var marker=node.Item1;var uv=state.Map.Project(marker.X,marker.Z);node.Item2.anchoredPosition=new Vector2((uv.X-.5f)*geometry.Width,(uv.Y-.5f)*geometry.Height);}
        }
        private void Zoom(float factor,Vector2 anchor)
        {
            if(state.Map==null||!state.Map.Valid||viewport.rect.width<=0||viewport.rect.height<=0)return;
            targetZoom=Mathf.Clamp(targetZoom*factor,1,8);zoomAnchor=anchor;
        }
        private void AnimateZoom()
        {
            float now=Time.unscaledTime,elapsed=Mathf.Clamp(now-lastZoomTime,0,.1f);lastZoomTime=now;
            if(state.Map==null||!state.Map.Valid||viewport.rect.width<=0||viewport.rect.height<=0||Mathf.Abs(targetZoom-zoom)<.0001f)return;
            var previous=Geometry(zoom);float next=zoom+(targetZoom-zoom)*(float)(1-Math.Exp(-14*elapsed));
            if(Mathf.Abs(targetZoom-next)<.001f)next=targetZoom;
            var current=Geometry(next);var position=current.ZoomAt(previous,new MapPoint(pan.x,pan.y),new MapPoint(zoomAnchor.x,zoomAnchor.y));
            zoom=next;pan=new Vector2(position.X,position.Y);
        }
        private void Center()
        {if(state.Map==null||!state.Map.Valid||viewport.rect.width<=0||viewport.rect.height<=0)return;targetZoom=zoom;ApplyMapGeometry();var uv=state.Map.Project(state.PlayerPosition.x,state.PlayerPosition.z);var center=Geometry(zoom).Center(uv);pan=new Vector2(center.X,center.Y);ApplyMapGeometry();}
        private void ClickMap(PointerEventData e)
        {
            if(e.button!=PointerEventData.InputButton.Right||state.Map==null||!state.Map.Valid||state.Markers.Count>=512)return;
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect,e.position,e.pressEventCamera,out var point))return;
            float u=point.x/mapRect.rect.width+.5f,v=point.y/mapRect.rect.height+.5f;if(u<0||u>1||v<0||v>1)return;
            var world=NavMath.UvToWorld(u,v,state.Map.MinX,state.Map.MaxX,state.Map.MinZ,state.Map.MaxZ);
            var marker=new NavMarker{Name=Texts.Get(state.Locale,"marker")+" "+(state.Markers.Count+1),X=world.X,Z=world.Y};state.Markers.Add(marker);Select(marker.Id);state.SaveMarkers();
        }
        private void Select(string id){selected=id;int index=state.Markers.FindIndex(m=>m.Id==id);if(index>=0)listOffset=index;RebuildSidebar();}
        private void RebuildSidebar()
        {
            // Commit an active edit before replacing its controls; its callback also checks scope identity.
            if(nameInput!=null&&nameInput.isFocused)nameInput.DeactivateInputField();
            for(int i=sidebarContent.childCount-1;i>=0;i--){var child=sidebarContent.GetChild(i).gameObject;child.SetActive(false);UnityEngine.Object.Destroy(child);}
            foreach(var node in markerNodes)if(node.Item2!=null){node.Item2.gameObject.SetActive(false);UnityEngine.Object.Destroy(node.Item2.gameObject);}markerNodes.Clear();markerNames.Clear();
            labels.RemoveAll(x=>x.Item1==null);allText.RemoveAll(x=>x==null);lastCount=state.Markers.Count;nameInput=null;targetInfo=null;
            foreach(var marker in state.Markers)
            {
                var node=Rect(marker.Id,mapRect);node.sizeDelta=new Vector2(36,36);var icon=Label(node,"",new Vector2(0,0),new Vector2(36,36),30);icon.text=MarkerSymbol(marker.Icon);icon.alignment=TextAlignmentOptions.Center;icon.color=MarkerColors[marker.Color];
                bool emphasized=marker.Id==selected||marker.Id==state.TargetId;
                var click=node.gameObject.AddComponent<Image>();click.color=emphasized?new Color(.25f,.19f,.09f,.94f):new Color(.07f,.075f,.065f,.8f);
                var button=node.gameObject.AddComponent<UnityEngine.UI.Button>();string id=marker.Id;button.onClick.AddListener(()=>Select(id));
                var plate=Box("NamePlate",node,new Vector2(40,-2),new Vector2(144,32),emphasized?new Color(.4f,.30f,.15f,.98f):new Color(.1f,.11f,.09f,.94f));plate.GetComponent<Image>().raycastTarget=false;
                var text=Label(plate,"",new Vector2(7,-4),new Vector2(130,24),16);text.text=marker.Name;text.textWrappingMode=TextWrappingModes.NoWrap;text.color=emphasized?Gold:Cream;markerNames.Add(Tuple.Create(marker,text,""));markerNodes.Add(Tuple.Create(marker,node));
            }
            float width=sidebarContent.rect.width>0?sidebarContent.rect.width:218;
            float height=sidebarContent.rect.height>0?sidebarContent.rect.height:500;
            Label(sidebarContent,"Markers",Vector2.zero,new Vector2(width-40,28),21).color=Gold;
            var countLabel=Label(sidebarContent,"",new Vector2(width-36,-3),new Vector2(36,24),16);countLabel.text=state.Markers.Count.ToString();countLabel.alignment=TextAlignmentOptions.Center;countLabel.color=Gold;
            float listHeight=Mathf.Clamp(height-410,72,240);int rows=Math.Max(2,(int)(listHeight/36));
            listOffset=Math.Max(0,Math.Min(listOffset,Math.Max(0,state.Markers.Count-rows)));
            var list=Box("MarkerList",sidebarContent,new Vector2(0,-38),new Vector2(width,listHeight),new Color(.085f,.073f,.058f,.8f));
            var scroll=list.gameObject.AddComponent<MapGesture>();scroll.Scroll=e=>{listOffset=Math.Max(0,Math.Min(listOffset+(e.scrollDelta.y>0?-1:1),Math.Max(0,state.Markers.Count-rows)));RebuildSidebar();};
            if(state.Markers.Count==0)Label(list,"NoMarkers",new Vector2(9,-12),new Vector2(width-18,60),16);
            else for(int i=0;i<rows&&listOffset+i<state.Markers.Count;i++)
            {
                var marker=state.Markers[listOffset+i];string id=marker.Id;var row=Button(list,"",new Vector2(3,-3-i*36),new Vector2(width-6,32),()=>Select(id));
                row.GetComponent<Image>().color=id==selected?Gold:new Color(.4f,.31f,.19f,1);
                row.GetChild(0).GetComponent<Image>().color=id==selected?new Color(.33f,.25f,.14f,1):new Color(.15f,.13f,.09f,1);
                var text=row.GetComponentInChildren<TMP_Text>();text.text=MarkerSymbol(marker.Icon)+"  "+marker.Name;text.color=id==selected?Gold:Cream;text.alignment=TextAlignmentOptions.Left;text.textWrappingMode=TextWrappingModes.NoWrap;
                markerNames.Add(Tuple.Create(marker,text,MarkerSymbol(marker.Icon)+"  "));
            }
            if(state.Markers.Count>rows)
            {
                float thumb=Mathf.Max(18,listHeight*rows/state.Markers.Count);
                float travel=(listHeight-thumb)*listOffset/(state.Markers.Count-rows);
                var rail=Box("ScrollTrack",list,new Vector2(width-3,0),new Vector2(3,listHeight),new Color(.1f,.08f,.05f,1));rail.GetComponent<Image>().raycastTarget=false;
                var handle=Box("ScrollPosition",list,new Vector2(width-3,-travel),new Vector2(3,thumb),Gold);handle.GetComponent<Image>().raycastTarget=false;
            }
            float y=50+listHeight;Line("ListDivider",sidebarContent,new Vector2(0,-y),new Vector2(width,1));y+=12;
            var chosen=state.Markers.Find(m=>m.Id==selected);
            if(chosen!=null)
            {
                var inputRect=Box("MarkerName",sidebarContent,new Vector2(0,-y),new Vector2(width,36),new Color(.29f,.24f,.165f,1));
                var textViewport=Fill("NameViewport",inputRect,5);textViewport.gameObject.AddComponent<RectMask2D>();var inputText=Label(textViewport,"",Vector2.zero,new Vector2(width-10,28),17);inputText.textWrappingMode=TextWrappingModes.NoWrap;
                nameInput=inputRect.gameObject.AddComponent<TMP_InputField>();nameInput.textViewport=textViewport;nameInput.textComponent=(TextMeshProUGUI)inputText;nameInput.characterLimit=80;nameInput.richText=false;nameInput.text=chosen.Name;
                string scope=state.ScopeId;nameInput.onEndEdit.AddListener(value=>{if(state.ScopeId==scope&&state.Markers.Contains(chosen)&&!string.IsNullOrWhiteSpace(value)){chosen.Name=value.Trim();state.SaveMarkers();}});
                y+=43;var coords=Label(sidebarContent,"",new Vector2(0,-y),new Vector2(width,23),14);coords.text=$"X {chosen.X:F1}  Z {chosen.Z:F1}";coords.color=new Color(.71f,.65f,.53f,1);y+=30;
                Button(sidebarContent,"SetTarget",new Vector2(0,-y),new Vector2(width,34),()=>{state.TargetId=chosen.Id;RebuildSidebar();});y+=43;
                Button(sidebarContent,"color",new Vector2(0,-y),new Vector2((width-8)/2,32),()=>{chosen.Color=(chosen.Color+1)%4;state.SaveMarkers();RebuildSidebar();});
                Button(sidebarContent,"icon",new Vector2((width+8)/2,-y),new Vector2((width-8)/2,32),()=>{chosen.Icon=(chosen.Icon+1)%3;state.SaveMarkers();RebuildSidebar();});y+=41;
                Button(sidebarContent,"delete",new Vector2(0,-y),new Vector2(width,30),()=>{state.Markers.Remove(chosen);if(state.TargetId==chosen.Id)state.TargetId="";selected="";state.SaveMarkers();RebuildSidebar();});y+=43;
            }
            else{Label(sidebarContent,"SelectMarker",new Vector2(0,-y),new Vector2(width,52),16);y+=66;}
            Line("TargetDivider",sidebarContent,new Vector2(0,-y),new Vector2(width,1));y+=12;
            Label(sidebarContent,"CurrentTarget",new Vector2(0,-y),new Vector2(width,25),17).color=Gold;y+=29;
            targetInfo=Label(sidebarContent,"",new Vector2(0,-y),new Vector2(width,47),16);y+=51;
            if(state.Target!=null)Button(sidebarContent,"clear_target",new Vector2(0,-y),new Vector2(width,30),()=>{state.TargetId="";RebuildSidebar();});
            ApplyMapGeometry();
        }
        public void ApplyLayout(NavigationLayout layout)
        {
            if(layout==null||!layout.IsValid())return;targetZoom=zoom;appliedLayout=layout;lastParentSize=((RectTransform)root.parent).rect.size;
            root.anchorMin=root.anchorMax=new Vector2(.5f,.5f);root.pivot=new Vector2(.5f,.5f);root.anchoredPosition=Vector2.zero;
            var parent=(RectTransform)root.parent;root.sizeDelta=new Vector2(Mathf.Max(1000,layout.MapWidth),Mathf.Max(740,layout.MapHeight));
            float w=parent.rect.width>0?parent.rect.width:1920,h=parent.rect.height>0?parent.rect.height:1080;float fit=Mathf.Min(1,Mathf.Min((w-32)/root.sizeDelta.x,(h-32)/root.sizeDelta.y));root.localScale=Vector3.one*Mathf.Max(.2f,fit);
            title.rectTransform.sizeDelta=new Vector2(root.sizeDelta.x-280,40);
            var divider=root.Find("HeaderRule") as RectTransform;if(divider!=null)divider.sizeDelta=new Vector2(root.sizeDelta.x-44,1);
            var group=root.GetComponent<CanvasGroup>()??root.gameObject.AddComponent<CanvasGroup>();group.alpha=layout.Opacity;
            help.rectTransform.sizeDelta=new Vector2(root.sizeDelta.x-310,32);status.rectTransform.sizeDelta=new Vector2(root.sizeDelta.x-310,28);
            if(IsOpen)RebuildSidebar();ApplyMapGeometry();
        }
        public void Dispose(){if(root!=null)UnityEngine.Object.Destroy(root.gameObject);if(frameSprite!=null)UnityEngine.Object.Destroy(frameSprite);if(frameTexture!=null)UnityEngine.Object.Destroy(frameTexture);if(paperTexture!=null)UnityEngine.Object.Destroy(paperTexture);}
    }
}
