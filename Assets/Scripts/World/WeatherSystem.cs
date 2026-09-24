using System;
using UnityEngine;
namespace Vanta.World {
 public enum WeatherState { Clear, Cloudy, Rain, Storm, Fog }
 public sealed class WeatherSystem : MonoBehaviour {
  [SerializeField] private WeatherState current=WeatherState.Clear;
  public WeatherState Current=>current;
  public event Action<WeatherState> Changed;
  public void SetWeather(WeatherState state){ if(current==state)return; current=state; Changed?.Invoke(state); }
 }
}