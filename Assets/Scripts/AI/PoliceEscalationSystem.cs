using System;using UnityEngine;
namespace Vanta.AI
{
 public enum PoliceEscalationLevel { Patrol, Response, Tactical, Major }
 public sealed class PoliceEscalationSystem:MonoBehaviour
 {
  public PoliceEscalationLevel Level { get; private set; }=PoliceEscalationLevel.Patrol;
  public event Action<PoliceEscalationLevel> LevelChanged;
  public void UpdateFromWantedLevel(int wanted)
  {
   var next=wanted>=5?PoliceEscalationLevel.Major:wanted>=4?PoliceEscalationLevel.Tactical:wanted>=2?PoliceEscalationLevel.Response:PoliceEscalationLevel.Patrol;
   if(next==Level)return; Level=next; LevelChanged?.Invoke(next);
  }
 }
}