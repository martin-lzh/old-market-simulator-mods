using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OldMarket.Navigation
{
    // Clone the native keycaps and description style; never modify the source prefabs or native actions.
    public sealed class MinimapZoomHint : IDisposable
    {
        private readonly RectTransform parent;
        private UIManager owner;
        private GameObject host;
        private TextMeshProUGUI caption;
        private ControlSlot minus, equals;
        private string lastLocale;
        private TMP_FontAsset lastFont;
        private float lastWidth=-1;
        public RectTransform Rect => host==null?null:host.transform as RectTransform;

        public MinimapZoomHint(RectTransform parent) {this.parent=parent;}

        public void Refresh(NavigationState state,float width)
        {
            var ui=UIManager.Instance;
            if(ui==null || ui.prefabControlHintDesc==null || ui.prefabControlHintSlot==null) {Dispose();return;}
            if(owner!=ui || host==null)
            {
                Dispose();owner=ui;
                var source=ui.prefabControlHintDesc.GetComponentInChildren<TextMeshProUGUI>(true);
                if(source==null)return;
                host=new GameObject("NavigationZoomHint",typeof(RectTransform));host.transform.SetParent(parent,false);
                // Only borrow the native visual elements. Its description container expects a different parent layout.
                caption=UnityEngine.Object.Instantiate(source.gameObject,host.transform,false).GetComponent<TextMeshProUGUI>();
                caption.gameObject.SetActive(true);
                minus=CreateKey(ui.prefabControlHintSlot,"-");
                equals=CreateKey(ui.prefabControlHintSlot,"=");
                if(caption==null || minus==null || equals==null) {Dispose();return;}
                foreach(var group in host.GetComponentsInChildren<LayoutGroup>(true))group.enabled=false;
                foreach(var fitter in host.GetComponentsInChildren<ContentSizeFitter>(true))fitter.enabled=false;
                foreach(var graphic in host.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
                var rect=Rect;rect.anchorMin=rect.anchorMax=new Vector2(.5f,1);rect.pivot=new Vector2(.5f,0);
                rect.anchoredPosition=new Vector2(0,14);
            }
            if(lastLocale==state.Locale && lastFont==state.Font && lastWidth==width)return;
            lastLocale=state.Locale;lastFont=state.Font;lastWidth=width;
            caption.text=Texts.Get(state.Locale,"ZoomHint");caption.richText=false;
            if(state.Font!=null) {caption.font=state.Font;minus.textBinding.font=state.Font;equals.textBinding.font=state.Font;}
            // One explicit horizontal row: caption, minus, equals. No layout pass may shrink Chinese to one character.
            float rowWidth=Mathf.Max(240,width);
            Rect.sizeDelta=new Vector2(rowWidth,32);Rect.localScale=Vector3.one;
            Place(caption.rectTransform,new Vector2(-40,0),new Vector2(rowWidth-80,32));
            caption.textWrappingMode=TextWrappingModes.NoWrap;caption.overflowMode=TextOverflowModes.Ellipsis;
            caption.alignment=TextAlignmentOptions.Center;caption.enableAutoSizing=true;caption.fontSizeMin=10;caption.fontSizeMax=18;
            Place(minus.transform as RectTransform,new Vector2(rowWidth/2-58,0),new Vector2(32,32));
            Place(equals.transform as RectTransform,new Vector2(rowWidth/2-18,0),new Vector2(32,32));
        }

        private static void Place(RectTransform rect,Vector2 position,Vector2 size)
        {
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);rect.localScale=Vector3.one;
            rect.anchoredPosition=position;rect.sizeDelta=size;
        }

        private ControlSlot CreateKey(GameObject prefab,string label)
        {
            var key=UnityEngine.Object.Instantiate(prefab,host.transform,false).GetComponent<ControlSlot>();
            if(key==null || key.textBinding==null || key.imageBinding==null || key.imageBackground==null)return null;
            key.textBinding.text=label;key.textBinding.alignment=TextAlignmentOptions.Center;
            key.textBinding.textWrappingMode=TextWrappingModes.NoWrap;
            key.textBinding.enableAutoSizing=true;key.textBinding.fontSizeMin=12;key.textBinding.fontSizeMax=18;
            if(key.textBinding.transform!=key.transform)Place(key.textBinding.rectTransform,Vector2.zero,new Vector2(32,32));
            if(key.imageBackground.transform!=key.transform)Place(key.imageBackground.rectTransform,Vector2.zero,new Vector2(32,32));
            key.textBinding.gameObject.SetActive(true);key.imageBinding.gameObject.SetActive(false);key.imageBackground.enabled=true;
            return key;
        }

        public void Dispose()
        {
            if(host!=null) {host.SetActive(false);UnityEngine.Object.Destroy(host);}
            host=null;owner=null;caption=null;minus=equals=null;lastLocale=null;lastFont=null;lastWidth=-1;
        }
    }
}
