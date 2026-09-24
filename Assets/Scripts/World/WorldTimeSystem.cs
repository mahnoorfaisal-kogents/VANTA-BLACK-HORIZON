using System;
using UnityEngine;
namespace Vanta.World {
 public sealed class WorldTimeSystem : MonoBehaviour {
  [SerializeField, Range(0f,24f)] private float timeOfDay=8f;
  [SerializeField] private float hoursPerRealSecond=0.01f;
  public float TimeOfDay => timeOfDay;
  public event Action<float> TimeChanged;
  void Update(){ timeOfDay=(timeOfDay+hoursPerRealSecond*Time.deltaTime)%24f; TimeChanged?.Invoke(timeOfDay); }
  public bool IsNight => timeOfDay>=20f || timeOfDay<6f;
 }
}