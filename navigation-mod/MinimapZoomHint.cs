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
                host=UnityEngine.Object.Instantiate(ui.prefabControlHintDesc,parent,false);
                host.name="NavigationZoomHint";
                caption=host.GetComponentInChildren<TextMeshProUGUI>();
                minus=CreateKey(ui.prefabControlHintSlot,"-");
                equals=CreateKey(ui.prefabControlHintSlot,"=");
                if(caption==null || minus==null || equals==null) {Dispose();return;}
                foreach(var graphic in host.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
                var rect=Rect;rect.anchorMin=rect.anchorMax=new Vector2(.5f,1);rect.pivot=new Vector2(.5f,0);
                rect.anchoredPosition=new Vector2(0,14);
            }
            if(lastLocale==state.Locale && lastFont==state.Font && lastWidth==width)return;
            lastLocale=state.Locale;lastFont=state.Font;lastWidth=width;
            caption.text=Texts.Get(state.Locale,"ZoomHint");caption.richText=false;
            if(state.Font!=null) {caption.font=state.Font;minus.textBinding.font=state.Font;equals.textBinding.font=state.Font;}
            // Preserve the native arrangement and keycap/label proportions while fitting the map width.
            Rect.localScale=Vector3.one;
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
            float preferred=Rect.rect.width;
            if(preferred>width && preferred>0)Rect.localScale=Vector3.one*(width/preferred);
        }

        private ControlSlot CreateKey(GameObject prefab,string label)
        {
            var key=UnityEngine.Object.Instantiate(prefab,host.transform,false).GetComponent<ControlSlot>();
            if(key==null || key.textBinding==null || key.imageBinding==null || key.imageBackground==null)return null;
            key.textBinding.text=label;key.textBinding.alignment=TextAlignmentOptions.Center;
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
