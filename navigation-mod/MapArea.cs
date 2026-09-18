using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OldMarket.Navigation
{
    /// <summary>Cartographic bounds of an expansion's affected surface, not a walkability polygon.</summary>
    [DataContract]
    public sealed class MapArea
    {
        [DataMember] public string Id;
        [DataMember] public float MinX,MaxX,MinZ,MaxZ;
        [DataMember] public long[] RequiredExpansions;

        public bool IsLocked(IReadOnlyList<long> unlocked) => !ExpansionRules.Matches(unlocked,RequiredExpansions,null);

        public static void Validate(MapArea[] areas,float minX,float maxX,float minZ,float maxZ)
        {
            if(areas==null)return;
            if(areas.Length>128)throw new ArgumentException("Too many map areas");
            var ids=new HashSet<string>();
            foreach(var area in areas)
                if(area==null||string.IsNullOrWhiteSpace(area.Id)||area.Id.Length>80||!ids.Add(area.Id)
                    ||!NavMath.ValidBounds(area.MinX,area.MaxX,area.MinZ,area.MaxZ)
                    ||area.MinX<minX||area.MaxX>maxX||area.MinZ<minZ||area.MaxZ>maxZ
                    ||area.RequiredExpansions==null||area.RequiredExpansions.Length==0||area.RequiredExpansions.Length>16)
                    throw new ArgumentException("Invalid map area");
        }
    }
}
