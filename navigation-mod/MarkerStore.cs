using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
namespace OldMarket.Navigation
{
    public class NavMarker
    {
        public string Id = Guid.NewGuid().ToString("N");
        public string Name = "";
        public float X, Z;
        public int Color, Icon;
    }
    [XmlRoot("NavigationMarkers")]
    public class MarkerDocument
    {
        public int Version = 1;
        public string Scope = "";
        public List<NavMarker> Markers = new List<NavMarker>();
    }
    public sealed class MarkerStore
    {
        private readonly string directory;
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(MarkerDocument));
        public MarkerStore(string directory) { this.directory=Path.GetFullPath(directory); }
        private string FileFor(string scope)
        {
            if (string.IsNullOrWhiteSpace(scope)) throw new ArgumentException("A stable save and region scope is required");
            using (var hash=SHA256.Create())
            { return Path.Combine(directory,BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(scope))).Replace("-","").ToLowerInvariant()+".xml"); }
        }
        private static void Validate(MarkerDocument doc,string scope)
        {
            if(doc.Version!=1 || doc.Scope!=scope || doc.Markers==null || doc.Markers.Count>512) throw new InvalidDataException("Invalid marker document");
            var ids=new HashSet<string>();
            foreach(var m in doc.Markers)
                if(m==null || string.IsNullOrEmpty(m.Id) || m.Id.Length>64 || !ids.Add(m.Id) || m.Name==null || m.Name.Length>80 || !NavMath.Finite(m.X) || !NavMath.Finite(m.Z) || m.Color<0 || m.Color>3 || m.Icon<0 || m.Icon>2) throw new InvalidDataException("Invalid marker");
        }
        public List<NavMarker> Load(string scope)
        {
            var path=FileFor(scope);
            if(!File.Exists(path)) return new List<NavMarker>();
            if(new FileInfo(path).Length>1024*1024) throw new InvalidDataException("Marker file exceeds size limit");
            using(var reader=System.Xml.XmlReader.Create(path,new System.Xml.XmlReaderSettings { DtdProcessing=System.Xml.DtdProcessing.Prohibit, XmlResolver=null }))
            { var doc=(MarkerDocument)Serializer.Deserialize(reader); Validate(doc,scope); return doc.Markers; }
        }
        public void Save(string scope,IEnumerable<NavMarker> markers)
        {
            var path=FileFor(scope);
            var doc=new MarkerDocument { Scope=scope,Markers=new List<NavMarker>(markers) };
            Validate(doc,scope);
            Directory.CreateDirectory(directory);
            var temporary=path+"."+Guid.NewGuid().ToString("N")+".tmp";
            try
            {
                using(var stream=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)) { Serializer.Serialize(stream,doc); stream.Flush(true); }
                if(File.Exists(path)) File.Replace(temporary,path,path+".bak"); else File.Move(temporary,path);
            }
            finally { if(File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
