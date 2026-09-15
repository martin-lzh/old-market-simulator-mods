using System;

namespace OldMarket.Navigation
{
    // Screen-local, center-origin geometry. MapPoint.Y is the upward viewport axis / northward UV.
    public readonly struct MapViewportGeometry
    {
        public readonly float ViewWidth, ViewHeight, Width, Height;

        public MapViewportGeometry(float viewWidth, float viewHeight, float mapAspect, float zoom)
        {
            if (!Positive(viewWidth) || !Positive(viewHeight) || !Positive(mapAspect) || !Positive(zoom))
                throw new ArgumentOutOfRangeException(nameof(viewWidth), "Viewport, aspect and zoom must be finite and positive.");
            ViewWidth = viewWidth; ViewHeight = viewHeight;
            double scale = Math.Max(1, zoom);
            double width = Math.Max(viewWidth, (double)viewHeight * mapAspect) * scale;
            double height = Math.Max(viewHeight, (double)viewWidth / mapAspect) * scale;
            if (width > float.MaxValue || height > float.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(mapAspect), "Map display size exceeds finite coordinates.");
            Width = (float)width; Height = (float)height;
        }

        public MapPoint ClampPan(MapPoint pan)
        {
            RequirePoint(pan);
            float maxX = Math.Max(0, (Width - ViewWidth) / 2);
            float maxY = Math.Max(0, (Height - ViewHeight) / 2);
            return new MapPoint(Math.Max(-maxX, Math.Min(maxX, pan.X)), Math.Max(-maxY, Math.Min(maxY, pan.Y)));
        }

        // An edge player cannot be placed in the viewport center without exposing blank map space.
        public MapPoint Center(MapPoint uv)
        {
            RequirePoint(uv);
            double x = -((double)uv.X - .5) * Width, y = -((double)uv.Y - .5) * Height;
            return ClampPan(new MapPoint(Saturate(x), Saturate(y)));
        }

        // Preserve the map point under a viewport-local anchor, except where an edge must clamp it.
        public MapPoint ZoomAt(MapViewportGeometry previous, MapPoint previousPan, MapPoint anchor)
        {
            RequirePoint(previousPan); RequirePoint(anchor);
            if (!Positive(previous.Width) || !Positive(previous.Height))
                throw new ArgumentException("Previous geometry must be initialized.", nameof(previous));
            double x = anchor.X - ((double)anchor.X - previousPan.X) * Width / previous.Width;
            double y = anchor.Y - ((double)anchor.Y - previousPan.Y) * Height / previous.Height;
            return ClampPan(new MapPoint(Saturate(x), Saturate(y)));
        }

        private static bool Positive(float value) => NavMath.Finite(value) && value > 0;
        private static float Saturate(double value) => (float)Math.Max(-float.MaxValue, Math.Min(float.MaxValue, value));
        private static void RequirePoint(MapPoint point)
        {
            if (!NavMath.Finite(point.X) || !NavMath.Finite(point.Y))
                throw new ArgumentException("Map coordinates must be finite.", nameof(point));
        }
    }
}
