using System;using System.Collections.Generic;using UnityEngine;
namespace Vanta.Systems
{
 public enum ServiceType { Rest, Save, ChangeLoadout, Repair, Customize }
 [Serializable] public sealed class SafehouseDefinition { public string id; public string displayName; public bool garage; public ServiceType[] services; }
 public sealed class SafehouseGarageSystem:MonoBehaviour
 {
  readonly Dictionary<string,SafehouseDefinition> locations=new();
  public void Register(SafehouseDefinition definition){if(definition==null||string.IsNullOrWhiteSpace(definition.id))return;locations[definition.id]=definition;}
  public SafehouseDefinition Get(string id)=>locations.TryGetValue(id,out var d)?d:null;
  public bool Supports(string id,ServiceType service)=>Get(id)?.services!=null&&Array.IndexOf(Get(id).services,service)>=0;
 }
}