using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OldMarket.Navigation
{
    /// <summary>One static texture and lightweight uGUI transforms; never renders the scene again.</summary>
    public sealed class NavigationHud : IDisposable
    {
        private readonly NavigationState state;
        private readonly RectTransform root, mini, disk, content, compass, targetRoot, worldTargetRoot;
        private readonly RawImage map, mapOverlay;
        private readonly TextMeshProUGUI playerArrow, north, mode, location, targetText, compassTarget, center, noMap, worldTargetText, worldTargetDiamond;
        private RectTransform ringArt;
        private readonly List<TextMeshProUGUI> cardinals = new List<TextMeshProUGUI>();
        private readonly List<TextMeshProUGUI> marks = new List<TextMeshProUGUI>();
        private readonly List<TextMeshProUGUI> labels = new List<TextMeshProUGUI>();
        private readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();
        private NavigationLayout layout = new NavigationLayout();
        private readonly Vector3[] boundsCorners = new Vector3[4];
        public bool MinimapVisible = true, CompassVisible = true, GuidanceVisible = true, CoordinatesVisible;
        public float MinimapRange = 100;
        private static readonly Color Gold = new Color(.92f, .79f, .53f, 1);
        private static readonly string[] Directions={"North","NE","East","SE","South","SW","West","NW"};

        public NavigationHud(NavigationState state, RectTransform parent, string directory)
        {
            this.state = state;
            root = Rect("NavigationHud", parent, Vector2.one, Vector2.zero);
            root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one; root.offsetMin = root.offsetMax = Vector2.zero;
            mini = Rect("Minimap", root, Vector2.one, new Vector2(240,240));
            mini.pivot = new Vector2(1,1);
            var ring = mini.gameObject.AddComponent<Image>(); ring.color = Gold; ring.raycastTarget = false;
            var circle = CircleSprite(); ring.sprite = circle;
            disk = Rect("MapMask", mini, new Vector2(.5f,.5f), new Vector2(228,228));
            var maskImage = disk.gameObject.AddComponent<Image>(); maskImage.sprite = circle; maskImage.color = new Color(.14f,.18f,.16f); maskImage.raycastTarget = false;
            disk.gameObject.AddComponent<Mask>().showMaskGraphic = true;
            content = Rect("MapContent", disk, new Vector2(.5f,.5f), Vector2.zero);
            var mapRect = Rect("Terrain", content, new Vector2(.5f,.5f), Vector2.one);
            map = mapRect.gameObject.AddComponent<RawImage>(); map.raycastTarget = false;
            var overlayRect=Rect("Obstacles",mapRect,new Vector2(.5f,.5f),Vector2.zero);
            overlayRect.anchorMin=Vector2.zero;overlayRect.anchorMax=Vector2.one;overlayRect.offsetMin=overlayRect.offsetMax=Vector2.zero;
            mapOverlay=overlayRect.gameObject.AddComponent<RawImage>();mapOverlay.raycastTarget=false;mapOverlay.gameObject.SetActive(false);
            noMap = Text("NoTerrain", disk, 14); noMap.rectTransform.sizeDelta=new Vector2(175,54); noMap.rectTransform.anchoredPosition=new Vector2(0,-52); noMap.textWrappingMode=TextWrappingModes.Normal;
            playerArrow = Text("Player", disk, 27); playerArrow.text = "▲"; playerArrow.color = Color.white;
            north = Text("North", mini, 19);
            mode = Text("Mode", mini, 17); mode.rectTransform.anchoredPosition = new Vector2(0,-138); mode.rectTransform.sizeDelta = new Vector2(330,28);
            var decoration = LoadSprite(Path.Combine(directory ?? "", "assets", "minimap-ring.png"));
            if (decoration != null)
            {
                var art = Rect("MinimapFrame", mini, new Vector2(.5f,.5f), new Vector2(252,252));
                ringArt = art;
                var image = art.gameObject.AddComponent<Image>(); image.sprite=decoration; image.raycastTarget=false;
            }
            compass = Rect("Compass", root, new Vector2(.5f,1), new Vector2(520,70)); compass.pivot=new Vector2(.5f,1);
            var shade = compass.gameObject.AddComponent<Image>(); shade.color = new Color(.07f,.1f,.12f,.62f); shade.raycastTarget=false;
            for (int i=0; i<8; i++) cardinals.Add(Text("Bearing"+i,compass,21));
            center = Text("Heading",compass,16); center.rectTransform.anchoredPosition = new Vector2(0,-24);
            compassTarget = Text("TargetBearing",compass,22); compassTarget.text="◆";
            location = Text("Coordinates",compass,18); location.rectTransform.anchoredPosition=new Vector2(0,-62); location.rectTransform.sizeDelta=new Vector2(550,28);
            targetRoot = Rect("TargetGuidance",root,new Vector2(.5f,1),new Vector2(520,64));
            targetRoot.pivot=new Vector2(.5f,1);
            targetText=Text("Target",targetRoot,22); targetText.rectTransform.sizeDelta=new Vector2(520,64);
            targetText.enableAutoSizing=true; targetText.fontSizeMin=14;
            worldTargetRoot=Rect("WorldTarget",root,new Vector2(.5f,.5f),new Vector2(300,80));
            worldTargetDiamond=Text("WorldTargetDiamond",worldTargetRoot,28);worldTargetDiamond.text="◆";
            worldTargetText=Text("WorldTargetLabel",worldTargetRoot,24);
            worldTargetText.rectTransform.sizeDelta=new Vector2(300,40);worldTargetText.rectTransform.anchoredPosition=new Vector2(0,-32);
            worldTargetText.enableAutoSizing=true;worldTargetText.fontSizeMin=14;
            worldTargetRoot.gameObject.SetActive(false);
            ApplyLayout(layout);
        }

        public void ApplyLayout(NavigationLayout value)
        {
            layout=value ?? new NavigationLayout();
            MinimapRange=layout.MinimapRange;
            mini.sizeDelta=Vector2.one*layout.MinimapSize;
            mini.anchorMin=mini.anchorMax=mini.pivot=layout.MinimapBottomLeft?Vector2.zero:Vector2.one;
            mini.anchoredPosition=layout.MinimapBottomLeft?new Vector2(layout.MinimapLeft,layout.MinimapBottom):new Vector2(-layout.MinimapRight,-layout.MinimapTop);
            disk.sizeDelta=Vector2.one*(layout.MinimapSize-12);
            if(ringArt!=null)ringArt.sizeDelta=Vector2.one*(layout.MinimapSize+12);
            compass.sizeDelta=new Vector2(layout.CompassWidth,70);
            compass.anchoredPosition=new Vector2(0,-layout.CompassTop);
            mode.rectTransform.anchoredPosition=new Vector2(0,-layout.MinimapSize/2-18);
            mode.rectTransform.sizeDelta=new Vector2(layout.MinimapSize+12,28);
            mode.enableAutoSizing=true;mode.fontSizeMin=10;mode.fontSizeMax=17;
            noMap.rectTransform.sizeDelta=new Vector2(layout.MinimapSize*.72f,layout.MinimapSize*.25f);
            noMap.rectTransform.anchoredPosition=new Vector2(0,-layout.MinimapSize*.22f);
            noMap.fontSize=Mathf.Clamp(layout.MinimapSize/17,10,16);
            targetText.fontSize=layout.WorldMarkerSize;
            targetText.fontSizeMax=layout.WorldMarkerSize;
            worldTargetText.fontSize=layout.WorldMarkerSize;worldTargetText.fontSizeMax=layout.WorldMarkerSize;
            worldTargetDiamond.fontSize=layout.WorldMarkerSize;
            var group=root.GetComponent<CanvasGroup>() ?? root.gameObject.AddComponent<CanvasGroup>();
            group.alpha=layout.Opacity; group.blocksRaycasts=false;
        }

        public void Refresh()
        {
            root.gameObject.SetActive(state.Available);
            if (!state.Available) return;
            foreach(var label in labels) if(label.font!=state.Font) label.font=state.Font;
            mini.gameObject.SetActive(MinimapVisible);
            compass.gameObject.SetActive(CompassVisible);
            location.gameObject.SetActive(CoordinatesVisible);
            location.text=string.Format(CultureInfo.CurrentCulture,"X {0:F1}   Y {1:F1}   Z {2:F1}",state.PlayerPosition.x,state.PlayerPosition.y,state.PlayerPosition.z);
            for(int i=0;i<8;i++)
            {
                float delta=Mathf.DeltaAngle(state.CameraYaw,i*45);
                cardinals[i].gameObject.SetActive(Mathf.Abs(delta)<85);
                cardinals[i].text=Texts.Get(state.Locale,Directions[i]);
                cardinals[i].rectTransform.anchoredPosition=new Vector2(delta/170*layout.CompassWidth,9);
            }
            center.text="▾  "+Mathf.RoundToInt(Mathf.Repeat(state.CameraYaw,360))+"°";
            float rotation=state.RotateWithCamera?state.CameraYaw:0;
            content.localRotation=Quaternion.Euler(0,0,rotation);
            playerArrow.rectTransform.localRotation=Quaternion.Euler(0,0,state.RotateWithCamera?0:-state.CameraYaw);
            float r=layout.MinimapSize*.43f;
            north.rectTransform.anchoredPosition=new Vector2(-Mathf.Sin(rotation*Mathf.Deg2Rad)*r,Mathf.Cos(rotation*Mathf.Deg2Rad)*r);
            north.text=Texts.Get(state.Locale,"North");
            mode.text=Texts.Get(state.Locale,state.RotateWithCamera?"CameraUp":"NorthUp");
            float scale=(layout.MinimapSize-12)/(2*MinimapRange);
            map.gameObject.SetActive(state.Map!=null && state.Map.Valid);
            noMap.gameObject.SetActive(!map.gameObject.activeSelf);
            mapOverlay.texture=state.Map?.OverlayTexture;mapOverlay.uvRect=map.uvRect;
            mapOverlay.gameObject.SetActive(map.gameObject.activeSelf && mapOverlay.texture!=null);
            noMap.text=Texts.Get(state.Locale,"NoMap");
            if(state.Map!=null && state.Map.Valid)
            {
                map.texture=state.Map.Texture;
                map.rectTransform.sizeDelta=new Vector2((state.Map.MaxX-state.Map.MinX)*scale,(state.Map.MaxZ-state.Map.MinZ)*scale);
                map.rectTransform.anchoredPosition=new Vector2(((state.Map.MinX+state.Map.MaxX)/2-state.PlayerPosition.x)*scale,((state.Map.MinZ+state.Map.MaxZ)/2-state.PlayerPosition.z)*scale);
            }
            NavMarker target=null;
            for(int i=0;i<state.Markers.Count;i++)
            {
                var marker=state.Markers[i];
                if(marker.Id==state.TargetId)target=marker;
                if(i>=marks.Count) marks.Add(Text("Marker"+i,content,18));
                var mark=marks[i]; mark.gameObject.SetActive(true); mark.text=MapWindow.MarkerSymbol(marker.Icon);
                mark.color=MapWindow.MarkerColors[Mathf.Clamp(marker.Color,0,MapWindow.MarkerColors.Length-1)];
                mark.rectTransform.anchoredPosition=new Vector2((marker.X-state.PlayerPosition.x)*scale,(marker.Z-state.PlayerPosition.z)*scale);
                mark.rectTransform.localRotation=Quaternion.Euler(0,0,-rotation);
            }
            for(int i=state.Markers.Count;i<marks.Count;i++)marks[i].gameObject.SetActive(false);
            bool targetVisible=GuidanceVisible && target!=null;
            targetRoot.gameObject.SetActive(targetVisible); compassTarget.gameObject.SetActive(targetVisible);
            worldTargetRoot.gameObject.SetActive(false);
            if(!targetVisible)return;
            float dx=target.X-state.PlayerPosition.x,dz=target.Z-state.PlayerPosition.z;
            float bearing=Mathf.Atan2(dx,dz)*Mathf.Rad2Deg;
            float difference=Mathf.DeltaAngle(state.CameraYaw,bearing);
            compassTarget.rectTransform.anchoredPosition=new Vector2(Mathf.Clamp(difference,-83,83)/170*layout.CompassWidth,-7);
            targetText.text=(difference < -5 ? "◀ " : difference > 5 ? "▶ " : "◆ ")+Texts.Get(state.Locale,"Target")+": "+target.Name+"\n"+Mathf.Sqrt(dx*dx+dz*dz).ToString("F0",CultureInfo.CurrentCulture)+" m  ·  "+Mathf.Abs(difference).ToString("F0",CultureInfo.CurrentCulture)+"°";
            // Fixed bearing panel: markers have X/Z only, so never imply an invented world height.
            targetRoot.anchoredPosition=new Vector2(0,-layout.CompassTop-(CompassVisible?CoordinatesVisible?126:92:0));
            if(state.WorldTargetVisible && RectTransformUtility.ScreenPointToLocalPointInRectangle(root,state.WorldTargetScreen,null,out var local)
                && Mathf.Abs(local.x)<root.rect.width/2-160 && Mathf.Abs(local.y)<root.rect.height/2-60)
            {
                worldTargetRoot.anchoredPosition=local;
                worldTargetText.text=target.Name+"  "+Mathf.Sqrt(dx*dx+dz*dz).ToString("F0",CultureInfo.CurrentCulture)+" m";
                worldTargetRoot.gameObject.SetActive(true);
            }
        }

        public void GetReservedBounds(out Rect? minimapBounds,out Rect? topBounds)
        {
            minimapBounds=null;topBounds=null;
            if(root==null || root.rect.width<=0 || root.rect.height<=0)return;
            AddBounds(ref minimapBounds,mini);
            AddBounds(ref minimapBounds,ringArt);
            AddBounds(ref minimapBounds,mode.rectTransform);
            AddBounds(ref topBounds,compass);
            AddBounds(ref topBounds,location.rectTransform);
            AddBounds(ref topBounds,targetRoot);
        }

        private void AddBounds(ref Rect? bounds,RectTransform rect)
        {
            if(rect==null || !rect.gameObject.activeInHierarchy)return;
            rect.GetWorldCorners(boundsCorners);
            for(int i=0;i<4;i++)
            {
                var point=RectTransformUtility.WorldToScreenPoint(null,boundsCorners[i]);
                bounds=bounds.HasValue?UnityEngine.Rect.MinMaxRect(Mathf.Min(bounds.Value.xMin,point.x),Mathf.Min(bounds.Value.yMin,point.y),Mathf.Max(bounds.Value.xMax,point.x),Mathf.Max(bounds.Value.yMax,point.y)):new Rect(point.x,point.y,0,0);
            }
        }

        private TextMeshProUGUI Text(string name,RectTransform parent,float size)
        {
            var rect=Rect(name,parent,new Vector2(.5f,.5f),new Vector2(100,36));
            var label=rect.gameObject.AddComponent<TextMeshProUGUI>(); label.font=state.Font; label.fontSize=size;
            label.richText=false;
            label.color=Gold; label.alignment=TextAlignmentOptions.Center; label.raycastTarget=false;
            label.textWrappingMode=TextWrappingModes.NoWrap; label.overflowMode=TextOverflowModes.Overflow;
            labels.Add(label); return label;
        }
        private static RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 size)
        {
            var rect=(RectTransform)new GameObject(name,typeof(RectTransform)).transform; rect.SetParent(parent,false);
            rect.anchorMin=rect.anchorMax=anchor; rect.sizeDelta=size;return rect;
        }
        private Sprite CircleSprite()
        {
            const int size=128;var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);var pixels=new Color32[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++) { float d=Vector2.Distance(new Vector2(x+.5f,y+.5f),new Vector2(size/2f,size/2f)); pixels[y*size+x]=new Color(1,1,1,Mathf.Clamp01(size/2f-d)); }
            texture.SetPixels32(pixels);texture.Apply(false,true);owned.Add(texture);
            var sprite=Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f));owned.Add(sprite);return sprite;
        }
        private Sprite LoadSprite(string path)
        {
            try {var bytes=UiAssets.Read(path); if(bytes==null)return null; var texture=new Texture2D(2,2); if(!ImageConversion.LoadImage(texture,bytes)) {UnityEngine.Object.Destroy(texture);return null;} owned.Add(texture);var sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f));owned.Add(sprite);return sprite;}
            catch(IOException){return null;}
        }
        public void Dispose() { if(root!=null)UnityEngine.Object.Destroy(root.gameObject);foreach(var item in owned)UnityEngine.Object.Destroy(item); }
    }
}
