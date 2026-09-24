using System;
using UnityEngine;

namespace Vanta.UI
{
    [Serializable]
    public sealed class VantaSettingsData
    {
        [Range(0.1f, 3f)] public float lookSensitivity = 1f;
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float effectsVolume = 1f;
        public bool subtitles = true;
        public bool reducedMotion;
        public bool colorAssist;
    }

    public sealed class VantaSettingsSystem : MonoBehaviour
    {
        public VantaSettingsData Data { get; private set; } = new();
        public event Action SettingsChanged;

        public void Apply(VantaSettingsData data)
        {
            if (data == null) return;
            Data = data;
            Data.lookSensitivity = Mathf.Clamp(Data.lookSensitivity, 0.1f, 3f);
            Data.masterVolume = Mathf.Clamp01(Data.masterVolume);
            Data.musicVolume = Mathf.Clamp01(Data.musicVolume);
            Data.effectsVolume = Mathf.Clamp01(Data.effectsVolume);
            SettingsChanged?.Invoke();
        }

        public void ResetToDefaults() => Apply(new VantaSettingsData());
    }
}