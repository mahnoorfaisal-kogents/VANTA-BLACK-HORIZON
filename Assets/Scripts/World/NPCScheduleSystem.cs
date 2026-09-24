using System;using System.Collections.Generic;using UnityEngine;
namespace Vanta.World
{
 public enum NPCActivity { Idle, Work, Shop, Social, Travel, Sleep }
 [Serializable] public sealed class ScheduleEntry { public int startHour; public int endHour; public NPCActivity activity; public string locationId; }
 public sealed class NPCScheduleSystem:MonoBehaviour
 {
  public NPCActivity ResolveActivity(IReadOnlyList<ScheduleEntry> schedule,float hour)
  {
   if(schedule==null)return NPCActivity.Idle;
   foreach(var e in schedule) if(e!=null&&IsWithin(hour,e.startHour,e.endHour)) return e.activity;
   return NPCActivity.Idle;
  }
  public static bool IsWithin(float hour,int start,int end)
  {
   hour=Mathf.Repeat(hour,24f); start=Mathf.Clamp(start,0,23); end=Mathf.Clamp(end,0,23);
   return start<=end ? hour>=start&&hour<end : hour>=start||hour<end;
  }
 }
}