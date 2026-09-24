using System;
using UnityEngine;
namespace Vanta.Systems {
 public sealed class WantedSystem:MonoBehaviour {
  [SerializeField,Range(0,5)] private int level;
  [SerializeField] private float decayDelay=12f, decayPerSecond=.2f;
  private float lastCrime;
  public int Level=>level;
  public float Heat {get;private set;}
  public event Action<int> LevelChanged;
  public float NormalizedHeat => Heat / 5f;
  public void SetHeatForLoad(float value){ Heat=Mathf.Clamp(value,0f,5f); SetLevel(Mathf.Clamp(Mathf.CeilToInt(Heat),0,5)); }
  void Update(){ if(Time.time-lastCrime<decayDelay)return; Heat=Mathf.Max(0,Heat-decayPerSecond*Time.deltaTime); SetLevel(Mathf.Clamp(Mathf.CeilToInt(Heat),0,5));}
  public void AddCrime(float amount){Heat=Mathf.Clamp(Heat+Mathf.Max(0,amount),0,5);lastCrime=Time.time;SetLevel(Mathf.Clamp(Mathf.CeilToInt(Heat),0,5));}
  void SetLevel(int value){if(level==value)return;level=value;LevelChanged?.Invoke(level);}
 }
}