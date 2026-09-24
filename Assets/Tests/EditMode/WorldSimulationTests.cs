using NUnit.Framework;using System.Collections.Generic;using UnityEngine;using Vanta.AI;using Vanta.Systems;using Vanta.World;
namespace Vanta.Tests
{
 public sealed class WorldSimulationTests
 {
  [Test] public void TrafficRouteLoopsToFirstPoint(){var r=new TrafficRoute{routeId="r",points=new[]{Vector3.zero,Vector3.right}};Assert.That(TrafficSystem.NextPoint(r,1),Is.EqualTo(Vector3.zero));}
  [Test] public void OvernightScheduleWrapsAcrossMidnight(){Assert.That(NPCScheduleSystem.IsWithin(23,22,2),Is.True);Assert.That(NPCScheduleSystem.IsWithin(1,22,2),Is.True);Assert.That(NPCScheduleSystem.IsWithin(12,22,2),Is.False);}
  [Test] public void SafehouseExposesOnlyRegisteredServices(){var s=new SafehouseGarageSystem();var d=new SafehouseDefinition{id="home",services=new[]{ServiceType.Rest,ServiceType.Save}};s.Register(d);Assert.That(s.Supports("home",ServiceType.Save),Is.True);Assert.That(s.Supports("home",ServiceType.Repair),Is.False);}
  [Test] public void PoliceEscalatesByWantedLevel(){var p=new PoliceEscalationSystem();p.UpdateFromWantedLevel(2);Assert.That(p.Level,Is.EqualTo(PoliceEscalationLevel.Response));p.UpdateFromWantedLevel(5);Assert.That(p.Level,Is.EqualTo(PoliceEscalationLevel.Major));}
  [Test] public void WorldSnapshotCopiesCollections(){var p=new PersistentWorldState();var snap=p.Capture(12,Vanta.World.WeatherSystem.WeatherState.Rain,new[]{"a"},new[]{"m"},new Dictionary<string,int>{{"z",10}});Assert.That(snap.discovered,Contains.Item("a"));Assert.That(snap.completedMissions,Contains.Item("m"));Assert.That(snap.territoryInfluence["z"],Is.EqualTo(10));}
 }
}