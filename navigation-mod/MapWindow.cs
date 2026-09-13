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
        private readonly RectTransform root, viewport, mapRect, player;
        private readonly RawImage mapImage, mapOverlay;
        private readonly RectTransform sidebar;
        private readonly TMP_Text title, help, status;
        private readonly List<Tuple<NavMarker,RectTransform>> markerNodes=new List<Tuple<NavMarker,RectTransform>>();
        private NavigationLayout appliedLayout=new NavigationLayout();
        private Vector2 lastParentSize;
        private readonly List<Tuple<TMP_Text,string>> labels=new List<Tuple<TMP_Text,string>>();
        private string selected="", lastLocale="", lastScope="";
        private float zoom=1;
        private Vector2 pan;
        private TMP_InputField nameInput;
        private Texture2D frameTexture, paperTexture; private Sprite frameSprite; private TMP_Text rotationLabel;
        private readonly List<TMP_Text> allText=new List<TMP_Text>();
        public bool IsOpen => root!=null && root.gameObject.activeSelf;
        private static readonly Color Paper=new Color(.77f,.66f,.47f,.97f), Ink=new Color(.22f,.16f,.1f,1);
        public static readonly Color[] MarkerColors={new Color(1,.77f,.25f),new Color(.35f,.8f,.95f),new Color(.95f,.4f,.35f),new Color(.45f,.9f,.55f)};
        public static string MarkerSymbol(int icon) => icon==1?"◆":icon==2?"●":"▼";
        public MapWindow(NavigationState state,RectTransform parent)
        {
            this.state=state;
            root=Rect("NavigationMapWindow",parent); root.anchorMin=new Vector2(.08f,.10f);root.anchorMax=new Vector2(.92f,.9f);root.offsetMin=root.offsetMax=Vector2.zero;
            var background=root.gameObject.AddComponent<Image>(); background.color=Paper;
            var paper=Rect("Parchment",root);paper.anchorMin=Vector2.zero;paper.anchorMax=Vector2.one;paper.offsetMin=new Vector2(10,10);paper.offsetMax=new Vector2(-10,-10);
            var paperImage=paper.gameObject.AddComponent<RawImage>();paperImage.color=Paper;paperImage.raycastTarget=false;
            var paperBytes=UiAssets.Read("parchment.png");
            if(paperBytes!=null) { paperTexture=new Texture2D(2,2,TextureFormat.RGBA32,false);if(ImageConversion.LoadImage(paperTexture,paperBytes)){paperImage.texture=paperTexture;paperImage.color=Color.white;} }
            var bytes=UiAssets.Read("map-frame.png");
            if(bytes!=null)
            {
                frameTexture=new Texture2D(2,2,TextureFormat.RGBA32,false);
                if(ImageConversion.LoadImage(frameTexture,bytes))
                {
                    frameSprite=Sprite.Create(frameTexture,new Rect(0,0,frameTexture.width,frameTexture.height),new Vector2(.5f,.5f),400,0,SpriteMeshType.FullRect,new Vector4(90,90,90,90));
                    var frame=Rect("MapFrame",root);frame.anchorMin=Vector2.zero;frame.anchorMax=Vector2.one;frame.offsetMin=frame.offsetMax=Vector2.zero;
                    var frameImage=frame.gameObject.AddComponent<Image>();frameImage.sprite=frameSprite;frameImage.type=Image.Type.Sliced;frameImage.color=Color.white;frameImage.raycastTarget=false;
                }
            }
            title=Label(root,"Map",new Vector2(24,-16),new Vector2(360,42),28);
            Button(root,"close",new Vector2(-160,-14),new Vector2(140,40),Close,true);
            viewport=Rect("MapViewport",root); viewport.anchorMin=new Vector2(0,0);viewport.anchorMax=new Vector2(1,1);viewport.offsetMin=new Vector2(20,100);viewport.offsetMax=new Vector2(-280,-70);
            viewport.gameObject.AddComponent<Image>().color=new Color(.18f,.23f,.2f);viewport.gameObject.AddComponent<RectMask2D>();
            mapRect=Rect("MapTexture",viewport);mapRect.anchorMin=mapRect.anchorMax=new Vector2(.5f,.5f);
            mapImage=mapRect.gameObject.AddComponent<RawImage>();mapImage.raycastTarget=false;
            var overlayRect=Rect("MapObstacles",mapRect);overlayRect.anchorMin=Vector2.zero;overlayRect.anchorMax=Vector2.one;overlayRect.offsetMin=overlayRect.offsetMax=Vector2.zero;
            mapOverlay=overlayRect.gameObject.AddComponent<RawImage>();mapOverlay.raycastTarget=false;mapOverlay.gameObject.SetActive(false);
            var gesture=viewport.gameObject.AddComponent<MapGesture>();gesture.Click=ClickMap;gesture.Drag=e=>{pan+=e.delta/CanvasScale();ApplyMapGeometry();};gesture.Scroll=e=>{zoom=Mathf.Clamp(zoom*Mathf.Pow(1.15f,e.scrollDelta.y),1,8);ApplyMapGeometry();};
            player=Rect("Player",mapRect);player.sizeDelta=new Vector2(28,28);var arrow=player.gameObject.AddComponent<TextMeshProUGUI>();arrow.text="▲";arrow.color=Color.white;arrow.fontSize=28;arrow.alignment=TextAlignmentOptions.Center;arrow.raycastTarget=false;allText.Add(arrow);if(state.Font!=null)arrow.font=state.Font;
            sidebar=Rect("MarkerControls",root);sidebar.anchorMin=new Vector2(1,0);sidebar.anchorMax=new Vector2(1,1);sidebar.pivot=new Vector2(1,.5f);sidebar.offsetMin=new Vector2(-260,100);sidebar.offsetMax=new Vector2(-20,-70);
            help=Label(root,"help",new Vector2(20,82),new Vector2(1100,25),16,false);
            status=Label(root,"",new Vector2(20,32),new Vector2(1100,24),15,false);
            Button(root,"center",new Vector2(-260,28),new Vector2(240,38),Center,true,false);
                        Button(root,"NorthUp",new Vector2(-400,-14),new Vector2(220,40),()=>{state.RotateWithCamera=!state.RotateWithCamera;state.RotationChanged?.Invoke(state.RotateWithCamera);},true);
            rotationLabel=labels[labels.Count-1].Item1;
            ApplyLayout(new NavigationLayout());
            root.gameObject.SetActive(false);
        }
        private float CanvasScale() { var canvas=root.GetComponentInParent<Canvas>();return canvas==null?1:Mathf.Max(.1f,canvas.scaleFactor*root.localScale.x); }
        private static RectTransform Rect(string name,Transform parent)
        { var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);return (RectTransform)go.transform; }
        private TMP_Text Label(RectTransform parent,string key,Vector2 pos,Vector2 size,float font,bool top=true)
        {
            var rect=Rect(key,parent);rect.anchorMin=rect.anchorMax=new Vector2(0,top?1:0);rect.pivot=new Vector2(0,1);rect.anchoredPosition=pos;rect.sizeDelta=size;
            var label=rect.gameObject.AddComponent<TextMeshProUGUI>();label.fontSize=font;label.color=Ink;label.raycastTarget=false;label.textWrappingMode=TextWrappingModes.Normal;label.richText=false;if(state.Font!=null)label.font=state.Font;
            allText.Add(label);
            if(key!="") { labels.Add(Tuple.Create((TMP_Text)label,key));label.text=Texts.Get(state.Locale,key); }return label;
        }
        private void Button(RectTransform parent,string key,Vector2 pos,Vector2 size,Action action,bool right=false,bool top=true)
        {
            var rect=Rect(key,parent);rect.anchorMin=rect.anchorMax=new Vector2(right?1:0,top?1:0);rect.pivot=new Vector2(0,top?1:0);rect.anchoredPosition=pos;rect.sizeDelta=size;
            rect.gameObject.AddComponent<Image>().color=new Color(.55f,.41f,.25f,.3f);var button=rect.gameObject.AddComponent<UnityEngine.UI.Button>();button.onClick.AddListener(()=>action());
            var label=Label(rect,key,new Vector2(4,-3),size-new Vector2(8,6),18);label.alignment=TextAlignmentOptions.Center;label.enableAutoSizing=true;label.fontSizeMin=12;label.fontSizeMax=18;
        }
        public void Toggle() { if(IsOpen) Close(); else {root.gameObject.SetActive(true);RebuildSidebar();Refresh();Center();} }
        public void Close() { if(root!=null)root.gameObject.SetActive(false); }
        public void Refresh()
        {
            if(!IsOpen)return;
            if(((RectTransform)root.parent).rect.size!=lastParentSize)ApplyLayout(appliedLayout);
            if(state.Font!=null)foreach(var text in allText)if(text!=null)text.font=state.Font;
            if(lastScope!=state.ScopeId) { lastScope=state.ScopeId;selected="";zoom=1;pan=Vector2.zero;RebuildSidebar(); }
            if(lastLocale!=state.Locale) { lastLocale=state.Locale;RebuildSidebar();foreach(var item in labels)if(item.Item1!=null)item.Item1.text=Texts.Get(state.Locale,item.Item2); }
            rotationLabel.text=Texts.Get(state.Locale,state.RotateWithCamera?"CameraUp":"NorthUp");
            title.text=Texts.Get(state.Locale,"Map")+(string.IsNullOrWhiteSpace(state.Map?.Name)?"":" · "+state.Map.Name);
            mapImage.texture=state.Map?.Texture;bool valid=state.Map!=null&&state.Map.Valid;
            mapRect.gameObject.SetActive(valid);
            mapOverlay.texture=state.Map?.OverlayTexture;mapOverlay.uvRect=mapImage.uvRect;
            mapOverlay.gameObject.SetActive(valid && mapOverlay.texture!=null);
            status.text=state.ErrorKey!=""?Texts.Get(state.Locale,state.ErrorKey):!valid?Texts.Get(state.Locale,"NoMap"):(string.IsNullOrEmpty(state.ScopeId)||state.Store==null)?Texts.Get(state.Locale,"temporary"):"";
            ApplyMapGeometry();
            if(valid)
            {
                var uv=state.Map.Project(state.PlayerPosition.x,state.PlayerPosition.z);player.anchoredPosition=new Vector2((uv.X-.5f)*mapRect.sizeDelta.x,(uv.Y-.5f)*mapRect.sizeDelta.y);player.localEulerAngles=new Vector3(0,0,-state.CameraYaw);player.gameObject.SetActive(state.Available);player.SetAsLastSibling();
            }
        }
        private void ApplyMapGeometry()
        {
            if(state.Map==null||!state.Map.Valid)return;
            float aspect=(state.Map.MaxX-state.Map.MinX)/(state.Map.MaxZ-state.Map.MinZ);
            float w=viewport.rect.width,h=w/aspect;if(h>viewport.rect.height){h=viewport.rect.height;w=h*aspect;}
            mapRect.sizeDelta=new Vector2(w,h)*zoom;
            pan.x=Mathf.Clamp(pan.x,-mapRect.sizeDelta.x*.5f,mapRect.sizeDelta.x*.5f);pan.y=Mathf.Clamp(pan.y,-mapRect.sizeDelta.y*.5f,mapRect.sizeDelta.y*.5f);mapRect.anchoredPosition=pan;
            foreach(var node in markerNodes)if(node.Item2!=null)
            {var marker=node.Item1;var uv=state.Map.Project(marker.X,marker.Z);node.Item2.anchoredPosition=new Vector2((uv.X-.5f)*mapRect.sizeDelta.x,(uv.Y-.5f)*mapRect.sizeDelta.y);}
        }
        private void Center()
        {if(state.Map==null||!state.Map.Valid)return;ApplyMapGeometry();var uv=state.Map.Project(state.PlayerPosition.x,state.PlayerPosition.z);pan=-new Vector2((uv.X-.5f)*mapRect.sizeDelta.x,(uv.Y-.5f)*mapRect.sizeDelta.y);ApplyMapGeometry();}
        private void ClickMap(PointerEventData e)
        {
            if(e.button!=PointerEventData.InputButton.Right||state.Map==null||!state.Map.Valid||state.Markers.Count>=512)return;
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect,e.position,e.pressEventCamera,out var point))return;
            float u=point.x/mapRect.rect.width+.5f,v=point.y/mapRect.rect.height+.5f;if(u<0||u>1||v<0||v>1)return;
            var world=NavMath.UvToWorld(u,v,state.Map.MinX,state.Map.MaxX,state.Map.MinZ,state.Map.MaxZ);
            var marker=new NavMarker {Name=Texts.Get(state.Locale,"marker")+" "+(state.Markers.Count+1),X=world.X,Z=world.Y};state.Markers.Add(marker);selected=marker.Id;state.SaveMarkers();RebuildSidebar();
        }
        private void RebuildSidebar()
        {
            for(int i=sidebar.childCount-1;i>=0;i--)UnityEngine.Object.Destroy(sidebar.GetChild(i).gameObject);
            foreach(var node in markerNodes)if(node.Item2!=null)UnityEngine.Object.Destroy(node.Item2.gameObject);markerNodes.Clear();labels.RemoveAll(x=>x.Item1==null);allText.RemoveAll(x=>x==null);
            foreach(var marker in state.Markers)
            {
                var rect=Rect(marker.Id,mapRect);rect.sizeDelta=new Vector2(32,32);var img=rect.gameObject.AddComponent<Image>();img.color=new Color(0,0,0,.25f);var button=rect.gameObject.AddComponent<UnityEngine.UI.Button>();string id=marker.Id;button.onClick.AddListener(()=>{selected=id;RebuildSidebar();});
                var text=Label(rect,"",new Vector2(0,0),new Vector2(32,32),24);text.alignment=TextAlignmentOptions.Center;text.text=MarkerSymbol(marker.Icon);text.color=MarkerColors[marker.Color];markerNodes.Add(Tuple.Create(marker,rect));
            }
            var chosen=state.Markers.Find(m=>m.Id==selected);
            if(chosen==null){Label(sidebar,"help",Vector2.zero,new Vector2(235,180),19);return;}
            var inputRect=Rect("MarkerName",sidebar);inputRect.anchorMin=inputRect.anchorMax=new Vector2(0,1);inputRect.pivot=new Vector2(0,1);inputRect.sizeDelta=new Vector2(235,48);inputRect.gameObject.AddComponent<Image>().color=new Color(1,1,1,.25f);
            var textViewport=Rect("TextViewport",inputRect);textViewport.anchorMin=Vector2.zero;textViewport.anchorMax=Vector2.one;textViewport.offsetMin=new Vector2(8,6);textViewport.offsetMax=new Vector2(-8,-6);textViewport.gameObject.AddComponent<RectMask2D>();
            var inputText=Label(textViewport,"",Vector2.zero,new Vector2(219,36),19);inputText.textWrappingMode=TextWrappingModes.NoWrap;
            nameInput=inputRect.gameObject.AddComponent<TMP_InputField>();nameInput.textViewport=textViewport;nameInput.textComponent=(TextMeshProUGUI)inputText;nameInput.characterLimit=80;nameInput.richText=false;nameInput.text=chosen.Name;
            string editScope=state.ScopeId;
            nameInput.onEndEdit.AddListener(value=>{ if(state.ScopeId==editScope && state.Markers.Contains(chosen) && !string.IsNullOrWhiteSpace(value)){chosen.Name=value.Trim();state.SaveMarkers();} });
            Label(sidebar,"Coordinates",new Vector2(0,-60),new Vector2(235,24),16);var coords=Label(sidebar,"",new Vector2(0,-88),new Vector2(235,30),17);coords.text=$"X {chosen.X:F1}   Z {chosen.Z:F1}";
            Button(sidebar,"Target",new Vector2(0,-130),new Vector2(235,40),()=>{state.TargetId=chosen.Id;});
            Button(sidebar,"clear_target",new Vector2(0,-180),new Vector2(235,40),()=>{state.TargetId="";});
            Button(sidebar,"color",new Vector2(0,-230),new Vector2(112,40),()=>{chosen.Color=(chosen.Color+1)%4;state.SaveMarkers();RebuildSidebar();});
            Button(sidebar,"icon",new Vector2(122,-230),new Vector2(112,40),()=>{chosen.Icon=(chosen.Icon+1)%3;state.SaveMarkers();RebuildSidebar();});
            Button(sidebar,"delete",new Vector2(0,-290),new Vector2(235,40),()=>{state.Markers.Remove(chosen);if(state.TargetId==chosen.Id)state.TargetId="";selected="";state.SaveMarkers();RebuildSidebar();});
        }
        public void ApplyLayout(NavigationLayout layout)
        {
            if(layout==null||!layout.IsValid())return;
            appliedLayout=layout;lastParentSize=((RectTransform)root.parent).rect.size;
            root.anchorMin=root.anchorMax=new Vector2(.5f,.5f);root.pivot=new Vector2(.5f,.5f);root.anchoredPosition=Vector2.zero;
            var parent=(RectTransform)root.parent;root.sizeDelta=new Vector2(Mathf.Max(860,layout.MapWidth),Mathf.Max(540,layout.MapHeight));
            float availableWidth=parent.rect.width>0?parent.rect.width:1920,availableHeight=parent.rect.height>0?parent.rect.height:1080;
            float fit=Mathf.Min(1,Mathf.Min((availableWidth-32)/root.sizeDelta.x,(availableHeight-32)/root.sizeDelta.y));root.localScale=Vector3.one*Mathf.Max(.2f,fit);
            title.rectTransform.sizeDelta=new Vector2(root.sizeDelta.x-450,42);
            var group=root.GetComponent<CanvasGroup>()??root.gameObject.AddComponent<CanvasGroup>();group.alpha=layout.Opacity;
            help.rectTransform.sizeDelta=new Vector2(Mathf.Max(200,root.sizeDelta.x-320),48);help.fontSize=14;
            status.rectTransform.sizeDelta=new Vector2(Mathf.Max(200,root.sizeDelta.x-320),24);
            ApplyMapGeometry();
        }
        public void Dispose() { if(root!=null)UnityEngine.Object.Destroy(root.gameObject);if(frameSprite!=null)UnityEngine.Object.Destroy(frameSprite);if(frameTexture!=null)UnityEngine.Object.Destroy(frameTexture);if(paperTexture!=null)UnityEngine.Object.Destroy(paperTexture); }
    }
}
