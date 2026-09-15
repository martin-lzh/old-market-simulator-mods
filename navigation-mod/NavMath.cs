using System;
namespace OldMarket.Navigation
{
    public readonly struct MapPoint
    {
        public readonly float X, Y;
        public MapPoint(float x, float y) { X = x; Y = y; }
    }
    public static class NavMath
    {
        public static bool Finite(float n) => !float.IsNaN(n) && !float.IsInfinity(n);
        public static bool ValidBounds(float minX, float maxX, float minZ, float maxZ) => Finite(minX) && Finite(maxX) && Finite(minZ) && Finite(maxZ) && maxX > minX && maxZ > minZ;
        public static MapPoint WorldToUv(float x, float z, float minX, float maxX, float minZ, float maxZ)
        {
            if (!ValidBounds(minX,maxX,minZ,maxZ)) throw new ArgumentException("Invalid map bounds");
            return new MapPoint((x-minX)/(maxX-minX), (z-minZ)/(maxZ-minZ));
        }
        public static MapPoint UvToWorld(float u, float v, float minX, float maxX, float minZ, float maxZ)
        {
            if (!ValidBounds(minX,maxX,minZ,maxZ)) throw new ArgumentException("Invalid map bounds");
            return new MapPoint(minX+u*(maxX-minX),minZ+v*(maxZ-minZ));
        }
        // X is screen-right, Y is screen-up. A clockwise camera bearing rotates the map counterclockwise.
        public static MapPoint RotateForCamera(float east, float north, float yaw)
        {
            double r = yaw*Math.PI/180;
            return new MapPoint((float)(east*Math.Cos(r)-north*Math.Sin(r)),(float)(east*Math.Sin(r)+north*Math.Cos(r)));
        }
        public static float Bearing(float dx,float dz) => (float)((Math.Atan2(dx,dz)*180/Math.PI+360)%360);
        public static float RelativeBearing(float bearing,float cameraYaw) => ((bearing-cameraYaw+540)%360+360)%360-180;
    }
}
