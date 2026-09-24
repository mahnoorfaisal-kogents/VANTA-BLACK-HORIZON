using UnityEngine;
namespace Vanta.World {
 [CreateAssetMenu(menuName="Vanta/World/District Definition")]
 public sealed class DistrictDefinition:ScriptableObject {
  public string districtId="district_01";
  public string displayName="Test District";
  public int dangerLevel=1;
  public string[] activities;
  public string[] sceneNames;
 }
}