using System.Collections.Generic;
using UnityEngine;
namespace Vanta.Systems {
 [CreateAssetMenu(menuName="Vanta/Systems/Faction Database")]
 public sealed class FactionDatabase:ScriptableObject { public string[] factionIds={"iron_jackals","black_veil","nova_shield"}; }
 public sealed class FactionSystem:MonoBehaviour {
  readonly Dictionary<string,int> reputation=new();
  public int GetReputation(string faction)=>reputation.TryGetValue(faction,out var v)?v:0;
  public void ChangeReputation(string faction,int delta){reputation[faction]=Mathf.Clamp(GetReputation(faction)+delta,-100,100);}
  public bool IsHostile(string faction)=>GetReputation(faction)<-25;
 }
}