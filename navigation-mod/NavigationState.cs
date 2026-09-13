using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace OldMarket.Navigation
{
    public sealed class MapDefinition
    {
        public Texture2D Texture;
        public Texture2D OverlayTexture;
        public float MinX, MaxX, MinZ, MaxZ;
        public string Id="",Name="";
        public bool Valid => Texture!=null && NavMath.ValidBounds(MinX,MaxX,MinZ,MaxZ);
        public MapPoint Project(float x,float z) => NavMath.WorldToUv(x,z,MinX,MaxX,MinZ,MaxZ);
    }
    public sealed class NavigationState
    {
        public Vector3 PlayerPosition;
        public float CameraYaw;
        public bool Available, RotateWithCamera;
        public bool WorldTargetVisible;
        public Vector2 WorldTargetScreen;
        public TMP_FontAsset Font;
        public MapDefinition Map;
        public string ScopeId="", Locale="en", TargetId="";
        public List<NavMarker> Markers=new List<NavMarker>();
        public MarkerStore Store;
        public Action Save;
        public Action<bool> RotationChanged;
        public string ErrorKey="";
        public NavMarker Target => Markers.Find(m=>m.Id==TargetId);
        public void SaveMarkers()
        {
            try { if(Store!=null && !string.IsNullOrEmpty(ScopeId)) { Store.Save(ScopeId,Markers); ErrorKey=""; } Save?.Invoke(); }
            catch(Exception) { ErrorKey="save_error"; }
        }
    }
}



