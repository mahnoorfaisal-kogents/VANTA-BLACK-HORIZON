using System;
using System.IO;
using UnityEngine;

namespace Vanta.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public Vector3 playerPosition;
        public int cash;
        public int wantedLevel;
        public float wantedHeat;
        public int xp;
        public float timeOfDay;
        public string activeMission;
    }

    public sealed class SaveSystem : MonoBehaviour
    {
        public string SlotPath(string slot)
        {
            var safeSlot = SanitizeSlot(slot);
            return Path.Combine(Application.persistentDataPath, "vanta_" + safeSlot + ".json");
        }

        public bool Save(string slot, SaveData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(slot)) return false;
            try
            {
                var path = SlotPath(slot);
                var temp = path + ".tmp";
                File.WriteAllText(temp, JsonUtility.ToJson(data, true));
                if (File.Exists(path)) File.Delete(path);
                File.Move(temp, path);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"VANTA save failed: {exception.Message}");
                return false;
            }
        }

        public SaveData Load(string slot)
        {
            if (string.IsNullOrWhiteSpace(slot)) return null;
            try
            {
                var path = SlotPath(slot);
                return File.Exists(path)
                    ? JsonUtility.FromJson<SaveData>(File.ReadAllText(path))
                    : null;
            }
            catch (Exception exception)
            {
                Debug.LogError($"VANTA load failed: {exception.Message}");
                return null;
            }
        }

        public bool Exists(string slot) =>
            !string.IsNullOrWhiteSpace(slot) && File.Exists(SlotPath(slot));

        static string SanitizeSlot(string slot)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var chars = slot.Trim().ToCharArray();
            for (var i = 0; i < chars.Length; i++)
                if (Array.IndexOf(invalid, chars[i]) >= 0 || chars[i] == '/' || chars[i] == '\')
                    chars[i] = '_';
            return new string(chars).Trim('.', ' ');
        }
    }
}