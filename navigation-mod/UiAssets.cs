using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace OldMarket.Navigation
{
    public static class UiAssets
    {
        public static byte[] Read(string path)
        {
            if(File.Exists(path)) return File.ReadAllBytes(path);
            string resource = "OldMarket.Navigation.assets." + Path.GetFileName(path);
            using(var stream=typeof(UiAssets).Assembly.GetManifestResourceStream(resource))
            {
                if(stream==null) return null;
                using(var buffer=new MemoryStream()) { stream.CopyTo(buffer); return buffer.ToArray(); }
            }
        }
    }
}
