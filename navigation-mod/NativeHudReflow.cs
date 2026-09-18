using System;
using System.Collections.Generic;
using LivingTheDeal.UI;
using UnityEngine;

namespace OldMarket.Navigation
{
    /// <summary>Repositions the audited native HUD only; native text, layout and tweens retain ownership.</summary>
    public sealed class NativeHudReflow : IDisposable
    {
        private sealed class Entry
        {
            public RectTransform Rect;
            public Vector2 Applied;
            public bool Conflicted;
            private bool ownsAnchors;
            private Vector2 beforeMin,beforeMax,writtenMin,writtenMax;
            public Entry(RectTransform rect) { Rect=rect; }
            public void CheckOwnership()
            {
                if(ownsAnchors && Rect!=null && (Rect.anchorMin!=writtenMin || Rect.anchorMax!=writtenMax))
                {
                    // Another writer replaced the anchors. Relinquish this instance rather than undo its work.
                    ownsAnchors=false;Applied=Vector2.zero;Conflicted=true;
                }
            }
            public void Apply(Vector2 next)
            {
                CheckOwnership();if(Conflicted || Rect==null)return;
                if(!ownsAnchors) {beforeMin=Rect.anchorMin;beforeMax=Rect.anchorMax;}
                var delta=next-Applied;
                if(Mathf.Abs(delta.x)<=0.000001f && Mathf.Abs(delta.y)<=0.000001f)return;
                Rect.anchorMin+=delta;Rect.anchorMax+=delta;Applied=next;
                writtenMin=Rect.anchorMin;writtenMax=Rect.anchorMax;ownsAnchors=true;
            }
            public void Restore()
            {
                CheckOwnership();
                if(Rect!=null && ownsAnchors) {Rect.anchorMin=beforeMin;Rect.anchorMax=beforeMax;}
                Applied=Vector2.zero;ownsAnchors=false;
            }
        }
        private UIManager owner;
        private Entry notifications, tutorial, toast;
        private readonly List<HudBox> occupied=new List<HudBox>();
        private readonly Vector3[] corners=new Vector3[4];
        private float nextPoll;
        private Rect? lastMini,lastTop;
        private int lastWidth,lastHeight;

        public void Tick(UIManager ui, Rect? minimapBounds, Rect? topBounds)
        {
            if(ui==null || (!minimapBounds.HasValue && !topBounds.HasValue)) {Restore();return;}
            if(owner!=ui) {Restore();owner=ui;}
            bool changed=!Nullable.Equals(lastMini,minimapBounds) || !Nullable.Equals(lastTop,topBounds)
                || lastWidth!=Screen.width || lastHeight!=Screen.height;
            if(!changed && Time.unscaledTime<nextPoll)return;
            nextPoll=Time.unscaledTime+.1f;lastMini=minimapBounds;lastTop=topBounds;lastWidth=Screen.width;lastHeight=Screen.height;
            Bind(ref notifications,ui.panelNotifications);
            Bind(ref tutorial,ui.panelTutorial);
            Bind(ref toast,ui.toastHint);
            occupied.Clear();
            float gap=Mathf.Max(6,Screen.height/1080f*12);
            var screen=new HudBox(gap,gap,Screen.width-gap*2,Screen.height-gap*2);
            if(minimapBounds.HasValue)occupied.Add(Box(minimapBounds.Value));
            if(topBounds.HasValue)occupied.Add(Box(topBounds.Value));
            // Native stats and operation hints remain in their original positions.
            if(ui.panelStats!=null && ui.panelStats.activeInHierarchy && TryMeasure(ui.panelStats.transform as RectTransform,false,out var stats))occupied.Add(Box(stats));
            if(ui.hints!=null && ui.hints.gameObject.activeInHierarchy && TryMeasure(ui.hints as RectTransform,true,out var hints))occupied.Add(Box(hints));

            // Notifications are lower-left by default. Only occupied child rows are moved above the minimap.
            if(minimapBounds.HasValue && TryOriginal(notifications,true,false,out var note))
            {
                var mini=minimapBounds.Value;
                Place(notifications,note,HudPlacement.AboveMap(Box(mini),note.width,note.height,Screen.width,gap),screen,gap);
            }
            else {notifications?.Restore();Reserve(notifications,true,false);}

            // The dynamic tutorial card keeps its original position unless it collides.
            if(TryOriginal(tutorial,false,false,out var card))
            {
                var desired=Box(card);
                if(Blocked(desired,gap))
                {
                    var obstruction=minimapBounds ?? topBounds.Value;
                    desired=new HudBox(obstruction.xMax-card.width,obstruction.yMin-gap-card.height,card.width,card.height);
                }
                Place(tutorial,card,desired,screen,gap);
            }
            else {tutorial?.Restore();Reserve(tutorial,false,false);}

            // ToastHint animates anchoredPosition from above the screen. Shift its anchor, never tween endpoints.
            if(topBounds.HasValue && TryOriginal(toast,false,true,out var hint))
            {
                var top=topBounds.Value;
                var below=new HudBox(screen.X,screen.Y,screen.Width,Mathf.Min(screen.Top,top.yMin-gap)-screen.Y);
                Place(toast,hint,new HudBox(hint.x,top.yMin-gap-hint.height,hint.width,hint.height),below,gap);
            }
            else toast?.Restore();
        }

        private static void Bind(ref Entry entry, GameObject go)
        {
            var rect=go==null?null:go.transform as RectTransform;
            if(entry!=null && entry.Rect==rect)return;
            entry?.Restore();entry=rect==null?null:new Entry(rect);
        }
        private bool Blocked(HudBox box,float gap) {foreach(var item in occupied)if(box.Overlaps(item,gap))return true;return false;}
        private void Reserve(Entry entry,bool children,bool rest)
        {if(TryOriginal(entry,children,rest,out var bounds))occupied.Add(Box(bounds));}

        private void Place(Entry entry,Rect original,HudBox desired,HudBox screen,float gap)
        {
            if(entry.Conflicted) {occupied.Add(Box(original));return;}
            if(!HudPlacement.TryPlace(desired,screen,occupied,gap,out var result))
            {
                // Excessively large native content has no collision-free slot; preserve it rather than hide/resize it.
                entry.Restore();occupied.Add(Box(original));return;
            }
            var parent=entry.Rect.parent as RectTransform;
            if(parent==null || parent.rect.width<=0 || parent.rect.height<=0)return;
            var camera=CameraFor(parent);
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,new Vector2(original.x,original.y),camera,out var from)
                || !RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,new Vector2(result.X,result.Y),camera,out var to))return;
            var next=new Vector2((to.x-from.x)/parent.rect.width,(to.y-from.y)/parent.rect.height);
            entry.Apply(next);
            occupied.Add(result);
        }

        private bool TryOriginal(Entry entry,bool childrenOnly,bool restingToast,out Rect bounds)
        {
            bounds=default;
            if(entry==null || entry.Rect==null || !entry.Rect.gameObject.activeInHierarchy)return false;
            entry.CheckOwnership();
            if(!TryMeasure(entry.Rect,childrenOnly,out bounds))return false;
            var parent=entry.Rect.parent as RectTransform;
            if(parent==null)return false;
            Vector2 local=new Vector2(-entry.Applied.x*parent.rect.width,-entry.Applied.y*parent.rect.height);
            if(restingToast)
            {
                var tween=entry.Rect.GetComponent<UITweener>();
                if(tween!=null)
                {
                    // Disable() swaps from/to. The lower Y endpoint is the visible position in both directions.
                    var rest=tween.from.y<tween.to.y?tween.from:tween.to;
                    local+=new Vector2(rest.x,rest.y)-entry.Rect.anchoredPosition;
                }
            }
            var camera=CameraFor(parent);
            var origin=RectTransformUtility.WorldToScreenPoint(camera,parent.TransformPoint(Vector3.zero));
            var end=RectTransformUtility.WorldToScreenPoint(camera,parent.TransformPoint(new Vector3(local.x,local.y,0)));
            bounds.position+=end-origin;
            return bounds.width>0 && bounds.height>0;
        }

        private bool TryMeasure(RectTransform rect,bool childrenOnly,out Rect bounds)
        {
            bounds=default;if(rect==null)return false;
            bool any=false;
            if(!childrenOnly)Append(rect,ref any,ref bounds);
            // Includes the task-log key hint below ToastHint and the fitted notification rows.
            foreach(var child in rect.GetComponentsInChildren<RectTransform>(false))
                if(child!=rect && child.gameObject.activeInHierarchy)Append(child,ref any,ref bounds);
            return any;
        }
        private void Append(RectTransform rect,ref bool any,ref Rect bounds)
        {
            if(rect.rect.width<=0 || rect.rect.height<=0)return;
            var camera=CameraFor(rect);
            // Audited notification rows fade; their real world corners remain stable throughout that animation.
            rect.GetWorldCorners(corners);
            foreach(var corner in corners)
            {
                var point=RectTransformUtility.WorldToScreenPoint(camera,corner);
                if(!any) {bounds=new Rect(point.x,point.y,0,0);any=true;}
                else bounds=Rect.MinMaxRect(Mathf.Min(bounds.xMin,point.x),Mathf.Min(bounds.yMin,point.y),Mathf.Max(bounds.xMax,point.x),Mathf.Max(bounds.yMax,point.y));
            }
        }
        private static Camera CameraFor(RectTransform rect)
        {var canvas=rect.GetComponentInParent<Canvas>();return canvas==null || canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;}
        private static HudBox Box(Rect rect)=>new HudBox(rect.x,rect.y,rect.width,rect.height);
        public void Restore()
        {notifications?.Restore();tutorial?.Restore();toast?.Restore();owner=null;notifications=tutorial=toast=null;nextPoll=0;lastMini=lastTop=null;lastWidth=lastHeight=0;}
        public void Dispose()=>Restore();
    }
}
