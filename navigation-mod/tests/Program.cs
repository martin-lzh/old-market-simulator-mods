using System;
using System.IO;
using System.Collections.Generic;
using OldMarket.Navigation;
static class Program
{
 static int count;
 static void Check(bool ok,string message){count++;if(!ok)throw new Exception(message);}
 static void Near(float actual,float expected,string name)=>Check(Math.Abs(actual-expected)<.001,name);
 static void Reject(Action action,string name){try{action();}catch{count++;return;}throw new Exception(name);}
 static void Main()
 {
  foreach(var p in new[]{new MapPoint(-10,-30),new MapPoint(90,170),new MapPoint(40,70),new MapPoint(0,0)})
  {var uv=NavMath.WorldToUv(p.X,p.Y,-10,90,-30,170);var round=NavMath.UvToWorld(uv.X,uv.Y,-10,90,-30,170);Near(round.X,p.X,"XZ roundtrip x");Near(round.Y,p.Y,"XZ roundtrip z");}
  Reject(()=>NavMath.WorldToUv(0,0,1,1,0,1),"zero bounds");Reject(()=>NavMath.UvToWorld(0,0,0,float.NaN,0,1),"NaN bounds");
  var north=NavMath.RotateForCamera(0,1,90);Near(north.X,-1,"north is left when looking east");Near(north.Y,0,"north y");
  var east=NavMath.RotateForCamera(1,0,90);Near(east.X,0,"east x");Near(east.Y,1,"east forward");
  foreach(float angle in new[]{0f,45f,90f,180f,270f}){var a=NavMath.RotateForCamera(3,4,angle);Near(a.X*a.X+a.Y*a.Y,25,"rotation length");}
  Near(NavMath.Bearing(0,1),0,"north bearing");Near(NavMath.Bearing(1,0),90,"east bearing");Near(NavMath.RelativeBearing(5,355),10,"wrap bearing");
  var directory=Path.Combine(Path.GetTempPath(),"navigation-tests-"+Guid.NewGuid().ToString("N"));
  try {
   var store=new MarkerStore(directory);var markers=new List<NavMarker>{new NavMarker{Name="港口 🧭",X=12.5f,Z=-6,Color=2,Icon=1}};
   store.Save("world1|town",markers);Check(store.Load("world1|town")[0].Name==markers[0].Name,"unicode persistence");Check(store.Load("world2|town").Count==0,"save isolation");Check(store.Load("world1|dungeon").Count==0,"region isolation");
   markers[0].Name="Renamed";store.Save("world1|town",markers);Check(store.Load("world1|town")[0].Name=="Renamed","atomic replace");Check(Directory.GetFiles(directory,"*.bak").Length==1,"previous backup");Check(Directory.GetFiles(directory,"*.tmp").Length==0,"no temp leak");
   Reject(()=>store.Save("",markers),"empty scope rejected");markers[0].X=float.NaN;Reject(()=>store.Save("world1|town",markers),"NaN rejected");Check(store.Load("world1|town")[0].Name=="Renamed","bad write preserves file");markers[0].X=0;
   markers.Add(markers[0]);Reject(()=>store.Save("world1|town",markers),"duplicate ID rejected");
  } finally { if(Directory.Exists(directory))Directory.Delete(directory,true); }
  foreach(var language in new[]{"zh","zh-Hant","en","de","fr","it","ja","ko","pt","ru","es","tr","uk"})Check(Texts.Get(language,"NoMap")!="NoMap","translated "+language);
  Check(Texts.Get("xx","NoMap")==Texts.Get("en","NoMap"),"unknown fallback");Check(Texts.Normalize("zh-TW")=="zh-Hant","traditional locale");Check(Texts.Normalize("pt-BR")=="pt","region locale");
    var values=(Dictionary<string,string[]>)typeof(Texts).GetField("Values",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).GetValue(null);
  foreach(var row in values) {Check(row.Value.Length==13,"locale count "+row.Key);foreach(var translation in row.Value)Check(!string.IsNullOrWhiteSpace(translation),"translation content "+row.Key);}
  // Synthetic identifiers verify the four legal stages without publishing game data.
  long[][] required={Array.Empty<long>(),new long[]{11},new long[]{11,22},new long[]{11,22,33}};
  long[][] excluded={new long[]{11,22,33},new long[]{22,33},new long[]{33},Array.Empty<long>()};
  int[] legalMasks={0,1,3,7};
  for(int mask=0;mask<8;mask++)
  {
   var unlocked=new List<long>();for(int bit=0;bit<3;bit++)if((mask&(1<<bit))!=0)unlocked.Add((bit+1)*11);
   int matches=0,selected=-1;for(int stage=0;stage<4;stage++)if(ExpansionRules.Matches(unlocked,required[stage],excluded[stage])){matches++;selected=stage;}
   int expected=Array.IndexOf(legalMasks,mask);Check(matches==(expected<0?0:1),"stage uniqueness or anomaly "+mask);Check(selected==expected,"selected expansion stage "+mask);
   unlocked.Add(999);int withUnrelated=0;for(int stage=0;stage<4;stage++)if(ExpansionRules.Matches(unlocked,required[stage],excluded[stage]))withUnrelated++;
   Check(withUnrelated==matches,"unrelated expansion invariant "+mask);
  }
  Check(!ExpansionRules.Matches(null,null,null),"unknown unlocked state fails closed");
  Check(ExpansionRules.Matches(Array.Empty<long>(),null,null),"null constraints mean unrestricted");
  Check(!ExpansionRules.Matches(new long[]{11},new long[]{11},new long[]{11}),"contradictory constraints do not match");
  Check(ExpansionRules.Matches(new long[]{11,11},new long[]{11,11},null),"duplicate IDs treated as a set");
  Check(new NavigationLayout().IsValid(),"default layout valid");
  var bounds=new Dictionary<string,(float min,float max)>{["MinimapSize"]=(120,480),["MinimapRight"]=(0,1200),["MinimapTop"]=(0,800),["MinimapRange"]=(20,1000),["CompassWidth"]=(240,1000),["CompassTop"]=(0,800),["WorldMarkerSize"]=(12,80),["MapWidth"]=(600,1800),["MapHeight"]=(400,1000),["Opacity"]=(.2f,1)};
  foreach(var bound in bounds)
  {
   var field=typeof(NavigationLayout).GetField(bound.Key);
   foreach(float value in new[]{bound.Value.min,bound.Value.max}){var layout=new NavigationLayout();field.SetValue(layout,value);Check(layout.IsValid(),"inclusive layout bound "+bound.Key);}
   foreach(float value in new[]{bound.Value.min-.01f,bound.Value.max+.01f,float.NaN,float.PositiveInfinity,float.NegativeInfinity}){var layout=new NavigationLayout();field.SetValue(layout,value);Check(!layout.IsValid(),"reject invalid layout "+bound.Key);}
  }
  Console.WriteLine($"Passed {count} navigation checks.");
 }
}
