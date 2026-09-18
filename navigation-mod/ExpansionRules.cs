using System.Collections.Generic;
namespace OldMarket.Navigation
{
    /// <summary>Null unlocked means unavailable state and never matches. Null rules impose no constraints.</summary>
    public static class ExpansionRules
    {
        public static bool Matches(IReadOnlyList<long> unlocked,long[] required,long[] excluded)
        {
            if(unlocked==null)return false;
            var ids=new HashSet<long>(unlocked);
            if(required!=null)foreach(long id in required)if(!ids.Contains(id))return false;
            if(excluded!=null)foreach(long id in excluded)if(ids.Contains(id))return false;
            return true;
        }
    }
}
