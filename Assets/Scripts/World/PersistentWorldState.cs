using System;using System.Collections.Generic;using UnityEngine;
namespace Vanta.World
{
 [Serializable] public sealed class WorldStateSnapshot
 {
  public float timeOfDay; public WeatherSystem.WeatherState weather; public List<string> discovered=new(); public List<string> completedMissions=new(); public Dictionary<string,int> territoryInfluence=new();
 }
 public sealed class PersistentWorldState:MonoBehaviour
 {
  public WorldStateSnapshot Capture(float time,WeatherSystem.WeatherState weather,IEnumerable<string> discoveredIds,IEnumerable<string> missions,IDictionary<string,int> territory)
  {
   return new WorldStateSnapshot{timeOfDay=time,weather=weather,discovered=discoveredIds==null?new List<string>():new List<string>(discoveredIds),completedMissions=missions==null?new List<string>():new List<string>(missions),territoryInfluence=territory==null?new Dictionary<string,int>():new Dictionary<string,int>(territory)};
  }
 }
}