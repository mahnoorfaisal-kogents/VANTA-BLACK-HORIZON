using System;using System.Collections.Generic;using UnityEngine;
namespace Vanta.World
{
 [Serializable] public sealed class TrafficRoute { public string routeId; public Vector3[] points; public float speedLimit=12f; }
 public sealed class TrafficSystem:MonoBehaviour
 {
  readonly Dictionary<string,TrafficRoute> routes=new();
  public event Action<string> RouteRegistered;
  public int RouteCount=>routes.Count;
  public void Register(TrafficRoute route){if(route==null||string.IsNullOrWhiteSpace(route.routeId)||route.points==null||route.points.Length<2)return;routes[route.routeId]=route;RouteRegistered?.Invoke(route.routeId);}
  public TrafficRoute GetRoute(string id)=>routes.TryGetValue(id,out var r)?r:null;
  public static Vector3 NextPoint(TrafficRoute route,int currentIndex){if(route?.points==null||route.points.Length==0)return Vector3.zero;return route.points[(currentIndex+1)%route.points.Length];}
 } 
}