namespace OldMarket.Navigation
{
    public static class MinimapZoom
    {
        private static readonly float[] Ranges = {20,35,50,75,100,150,225,350,500,750,1000};

        public static float Step(float current, bool zoomIn)
        {
            if(!NavMath.Finite(current))current=100;
            if(zoomIn)
            {
                for(int i=Ranges.Length-1;i>=0;i--)if(Ranges[i]<current-.001f)return Ranges[i];
                return Ranges[0];
            }
            foreach(float range in Ranges)if(range>current+.001f)return range;
            return Ranges[Ranges.Length-1];
        }
    }
}
