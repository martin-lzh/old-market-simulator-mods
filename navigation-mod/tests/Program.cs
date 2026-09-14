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
 static void Main(string[] args)
 {
  PoiNativeNameChecks.Run();
  MapManifestChecks.Run(Check,Reject);
  foreach(string path in args)
  {
   var map=MapManifestReader.Read(File.ReadAllText(path));
   Check(NavMath.ValidBounds(map.MinX,map.MaxX,map.MinZ,map.MaxZ),"packaged map bounds");
   Check(MapPoi.Validate(map.Pois,map.MinX,map.MaxX,map.MinZ,map.MaxZ).Count>0,"packaged map contains valid POIs");
   Console.WriteLine(Path.GetFileName(path)+": "+map.Pois.Length+" POIs");
  }
  MapWheelZoomChecks.Run(Check);
  PoiLabelChecks.Check(Check);
  var validPoi=new MapPoi {Id="shop",Name="Shop",Category="shop",X=5,Z=5};
  Check(MapPoi.Validate(new[]{validPoi},0,10,0,10).Count==1,"POI metadata accepts bounded shop");
  Reject(()=>MapPoi.Validate(new[]{validPoi,validPoi},0,10,0,10),"POI duplicate id rejected");
  Reject(()=>MapPoi.Validate(new[]{validPoi},6,10,0,10),"POI outside map rejected");
  Check(MapPoi.Validate(null,0,10,0,10).Count==0,"legacy maps without POI remain supported");
  foreach(var p in new[]{new MapPoint(-10,-30),new MapPoint(90,170),new MapPoint(40,70),new MapPoint(0,0)})
  {var uv=NavMath.WorldToUv(p.X,p.Y,-10,90,-30,170);var round=NavMath.UvToWorld(uv.X,uv.Y,-10,90,-30,170);Near(round.X,p.X,"XZ roundtrip x");Near(round.Y,p.Y,"XZ roundtrip z");}
  Reject(()=>NavMath.WorldToUv(0,0,1,1,0,1),"zero bounds");Reject(()=>NavMath.UvToWorld(0,0,0,float.NaN,0,1),"NaN bounds");
  var north=NavMath.RotateForCamera(0,1,90);Near(north.X,-1,"north is left when looking east");Near(north.Y,0,"north y");
  var east=NavMath.RotateForCamera(1,0,90);Near(east.X,0,"east x");Near(east.Y,1,"east forward");
  foreach(float angle in new[]{0f,45f,90f,180f,270f}){var a=NavMath.RotateForCamera(3,4,angle);Near(a.X*a.X+a.Y*a.Y,25,"rotation length");}
  Near(NavMath.Bearing(0,1),0,"north bearing");Near(NavMath.Bearing(1,0),90,"east bearing");Near(NavMath.RelativeBearing(5,355),10,"wrap bearing");
  // A cover map fills both axes, even when its world aspect differs from the viewport.
  var tallMap=new MapViewportGeometry(900,600,1,1);
  Near(tallMap.Width,900,"cover square width");Near(tallMap.Height,900,"cover square height avoids side gutters");
  var wideMap=new MapViewportGeometry(600,900,2,1);
  Near(wideMap.Width,1800,"portrait viewport wide map cover");Near(wideMap.Height,900,"portrait viewport filled height");
  foreach(var size in new[]{new MapPoint(900,600),new MapPoint(600,900),new MapPoint(720,720)})
  foreach(float aspect in new[]{.5f,1f,2f})
  foreach(float level in new[]{1f,2.7f,8f})
  {
   var geometry=new MapViewportGeometry(size.X,size.Y,aspect,level);
   Check(geometry.Width>=size.X&&geometry.Height>=size.Y,"cover contains viewport at each aspect and zoom");
   Near(geometry.Width/geometry.Height,aspect,"cover preserves world aspect");
   foreach(float direction in new[]{-1f,1f})
   {
    var edge=geometry.ClampPan(new MapPoint(direction*1e6f,direction*1e6f));
    Check(edge.X-geometry.Width/2<=-size.X/2+.001f&&edge.X+geometry.Width/2>=size.X/2-.001f,"drag limits expose no horizontal blank space");
    Check(edge.Y-geometry.Height/2<=-size.Y/2+.001f&&edge.Y+geometry.Height/2>=size.Y/2-.001f,"drag limits expose no vertical blank space");
   }
  }
  var centered=tallMap.Center(new MapPoint(.5f,.5f));Near(centered.X,0,"center player x");Near(centered.Y,0,"center player y");
  var nearCenter=tallMap.Center(new MapPoint(.5f,.6f));Near(nearCenter.Y,-90,"interior player centers exactly");
  var edgeCenter=tallMap.Center(new MapPoint(1,1));Near(edgeCenter.X,0,"edge player cannot expose horizontal blank space");Near(edgeCenter.Y,-150,"edge player stops at vertical cover boundary");
  var outsideCenter=tallMap.Center(new MapPoint(-100,100));Near(outsideCenter.X,0,"out-of-map player clamp x");Near(outsideCenter.Y,-150,"out-of-map player clamp y");
  var lowZoom=new MapViewportGeometry(900,600,1,.1f);Near(lowZoom.Width,tallMap.Width,"zoom-out cannot undershoot cover");
  var zoomStart=new MapViewportGeometry(800,500,1.5f,2);
  var zoomEnd=new MapViewportGeometry(800,500,1.5f,5);
  var oldPan=new MapPoint(60,-40);var anchor=new MapPoint(175,-90);
  var newPan=zoomEnd.ZoomAt(zoomStart,oldPan,anchor);
  Near((anchor.X-oldPan.X)/zoomStart.Width,(anchor.X-newPan.X)/zoomEnd.Width,"cursor anchor world x retained while zooming");
  Near((anchor.Y-oldPan.Y)/zoomStart.Height,(anchor.Y-newPan.Y)/zoomEnd.Height,"cursor anchor world z retained while zooming");
  var roundPan=zoomStart.ZoomAt(zoomEnd,newPan,anchor);Near(roundPan.X,oldPan.X,"zoom roundtrip x");Near(roundPan.Y,oldPan.Y,"zoom roundtrip y");
  var centerZoom=zoomEnd.ZoomAt(zoomStart,oldPan,new MapPoint(0,0));Near(centerZoom.X,150,"button zoom preserves viewport center x");Near(centerZoom.Y,-100,"button zoom preserves viewport center y");
  var smoothPan=oldPan;var previous=zoomStart;
  foreach(float level in new[]{2.1f,2.5f,3.2f,4.7f,5f})
  {var next=new MapViewportGeometry(800,500,1.5f,level);smoothPan=next.ZoomAt(previous,smoothPan,anchor);previous=next;}
  Near(smoothPan.X,newPan.X,"smooth zoom matches direct anchored x");Near(smoothPan.Y,newPan.Y,"smooth zoom matches direct anchored y");
  var zoomBase=new MapViewportGeometry(800,500,1.5f,1);
  var clampedZoom=zoomBase.ZoomAt(zoomEnd,zoomEnd.ClampPan(new MapPoint(1e6f,-1e6f)),anchor);
  Check(Math.Abs(clampedZoom.X)<.001f&&Math.Abs(clampedZoom.Y)<= (zoomBase.Height-500)/2+.001f,"zoom out clamps edges without gutters");
  foreach(float bad in new[]{0f,-1f,float.NaN,float.PositiveInfinity})Reject(()=>new MapViewportGeometry(bad,600,1,1),"invalid viewport rejected");
  Reject(()=>new MapViewportGeometry(800,500,float.Epsilon,8),"unrepresentable aspect rejected");
  Reject(()=>tallMap.ClampPan(new MapPoint(float.NaN,0)),"nonfinite pan rejected");
  Reject(()=>tallMap.ZoomAt(default,new MapPoint(0,0),new MapPoint(0,0)),"uninitialized zoom origin rejected");
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
  Check(new NavigationLayout().MinimapBottomLeft,"minimap defaults to bottom left");
  Check(new NavigationLayout{MinimapBottomLeft=false}.IsValid(),"legacy top-right anchoring remains supported");
  var bounds=new Dictionary<string,(float min,float max)>{["MinimapSize"]=(120,480),["MinimapRight"]=(0,1200),["MinimapTop"]=(0,800),["MinimapLeft"]=(0,1200),["MinimapBottom"]=(0,800),["MinimapRange"]=(20,1000),["CompassWidth"]=(240,1000),["CompassTop"]=(0,800),["WorldMarkerSize"]=(12,80),["MapWidth"]=(600,1800),["MapHeight"]=(400,1000),["Opacity"]=(.2f,1)};
  foreach(var bound in bounds)
  {
   var field=typeof(NavigationLayout).GetField(bound.Key);
   foreach(float value in new[]{bound.Value.min,bound.Value.max}){var layout=new NavigationLayout();field.SetValue(layout,value);Check(layout.IsValid(),"inclusive layout bound "+bound.Key);}
   foreach(float value in new[]{bound.Value.min-.01f,bound.Value.max+.01f,float.NaN,float.PositiveInfinity,float.NegativeInfinity}){var layout=new NavigationLayout();field.SetValue(layout,value);Check(!layout.IsValid(),"reject invalid layout "+bound.Key);}
  }
  // Native notification stacks must fit above the map, or take a genuinely empty adjacent slot.
  var viewport=new HudBox(12,12,1896,1056);
  var mini=new HudBox(1650,650,250,280);
  var compass=new HudBox(700,920,520,110);
  var occupied=new List<HudBox>{mini,compass};
  Check(HudPlacement.TryPlace(new HudBox(1300,942,600,60),viewport,occupied,12,out var message),"notification fits above minimap");
  Near(message.Y,942,"notification above minimap");
  occupied.Add(message);
  Check(HudPlacement.TryPlace(new HudBox(1540,540,360,98),viewport,occupied,12,out var tutorial),"tutorial fits below minimap");
  Check(!tutorial.Overlaps(mini,12)&&!tutorial.Overlaps(message,12),"tutorial avoids map and messages");
  Check(HudPlacement.TryPlace(new HudBox(1300,942,600,420),viewport,new[]{mini,compass},12,out var burst),"large burst has fallback");
  Check(burst.X>=viewport.X&&burst.Y>=viewport.Y&&burst.Right<=viewport.Right&&burst.Top<=viewport.Top,"burst stays onscreen");
  Check(!burst.Overlaps(mini,12)&&!burst.Overlaps(compass,12),"burst fallback avoids navigation");
  Check(!HudPlacement.TryPlace(new HudBox(0,0,2000,1100),viewport,occupied,12,out _),"oversized content fails without clipping");
  Check(!HudPlacement.TryPlace(new HudBox(0,0,100,100),viewport,new[]{viewport},12,out _),"no space fails without overlap");
  var belowCompass=new HudBox(12,12,1896,896);
  Check(HudPlacement.TryPlace(new HudBox(660,804,600,104),belowCompass,new[]{new HudBox(660,790,600,120)},12,out var hint),"top banner finds space below compass");
  Check(hint.Top<=compass.Y-12,"top banner fallback never jumps above compass");
  var free=new HudBox(110,210,300,60);
  Check(HudPlacement.TryPlace(free,viewport,Array.Empty<HudBox>(),12,out var untouched),"free original location accepted");
  Near(untouched.X,free.X,"free original x preserved");Near(untouched.Y,free.Y,"free original y preserved");
  var leftMini=new HudBox(18,24,252,282);
  var desiredAboveLeft=HudPlacement.AboveMap(leftMini,600,60,1920,12);
  var leftOccupied=new List<HudBox>{leftMini,compass,new HudBox(16,1020,500,48)};
  Check(HudPlacement.TryPlace(desiredAboveLeft,viewport,leftOccupied,12,out var leftMessage),"left minimap notification fits");
  Near(leftMessage.X,leftMini.X,"left minimap notifications align to left edge");Near(leftMessage.Y,leftMini.Top+12,"left minimap notifications above map");
  leftOccupied.Add(leftMessage);
  var originalTutorial=new HudBox(1544,960,360,98);
  Check(HudPlacement.TryPlace(originalTutorial,viewport,leftOccupied,12,out var retainedTutorial),"right task is unobstructed with left minimap");
  Near(retainedTutorial.X,originalTutorial.X,"right task keeps original x");Near(retainedTutorial.Y,originalTutorial.Y,"right task keeps original y");
  leftOccupied.Add(retainedTutorial);
  Check(HudPlacement.TryPlace(new HudBox(660,804,600,104),belowCompass,leftOccupied,12,out var leftToast),"toast fits below compass with left minimap");
  Check(leftToast.Top<=compass.Y-12&&!leftToast.Overlaps(leftMessage,12)&&!leftToast.Overlaps(leftMini,12),"toast avoids lower-left elements");
  Near(HudPlacement.AboveMap(mini,600,60,1920,12).X,mini.Right-600,"right minimap notifications retain right alignment");
  Check(HudPlacement.TryPlace(new HudBox(leftMini.Right-360,leftMini.Y-110,360,98),viewport,leftOccupied,12,out var lowFallback),"lower-left conflicting card has safe fallback");
  Check(lowFallback.X>=viewport.X&&lowFallback.Y>=viewport.Y&&lowFallback.Right<=viewport.Right&&lowFallback.Top<=viewport.Top,"lower-left fallback stays inside screen");
  // Scaling both canvases to physical pixels must preserve the chosen logical slot.
  foreach(float scale in new[]{.5f,.75f,1.5f,2f})
  {
   HudBox Scaled(HudBox b)=>new HudBox(b.X*scale,b.Y*scale,b.Width*scale,b.Height*scale);
   Check(HudPlacement.TryPlace(Scaled(new HudBox(1300,942,600,60)),Scaled(viewport),new[]{Scaled(mini),Scaled(compass)},12*scale,out var scaled),"resolution scaled placement");
   Near(scaled.X/scale,message.X,"resolution stable x");Near(scaled.Y/scale,message.Y,"resolution stable y");
  }
  Near(MinimapZoom.Step(100,true),75,"equals zooms in by reducing range");
  Near(MinimapZoom.Step(100,false),150,"minus zooms out by increasing range");
  Near(MinimapZoom.Step(123,true),100,"custom range steps inward to nearest level");
  Near(MinimapZoom.Step(123,false),150,"custom range steps outward to nearest level");
  Near(MinimapZoom.Step(20,true),20,"zoom-in lower limit");
  Near(MinimapZoom.Step(1000,false),1000,"zoom-out upper limit");
  foreach(float range in new[]{20f,35,50,75,100,150,225,350,500,750,1000})
  {
   Check(MinimapZoom.Step(range,true)<=range && MinimapZoom.Step(range,true)>=20,"inward zoom bounded and monotonic");
   Check(MinimapZoom.Step(range,false)>=range && MinimapZoom.Step(range,false)<=1000,"outward zoom bounded and monotonic");
  }
  Console.WriteLine($"Passed {count} navigation checks.");
 }
}
