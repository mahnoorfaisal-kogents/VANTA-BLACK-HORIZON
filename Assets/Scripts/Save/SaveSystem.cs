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
        public System.Collections.Generic.List<MissionSaveState> missions = new();
    }

    public sealed class SaveSystem : MonoBehaviour
    {
        public string SlotPath(string slot)
        {
            var safeSlot = SanitizeSlot(slot);
            return string.IsNullOrEmpty(safeSlot)
                ? null
                : Path.Combine(Application.persistentDataPath, "vanta_" + safeSlot + ".json");
        }

        public bool Save(string slot, SaveData data)
        {
            if (data == null) return false;
            var path = SlotPath(slot);
            if (string.IsNullOrEmpty(path)) return false;

            var temp = path + ".tmp";
            try
            {
                File.WriteAllText(temp, JsonUtility.ToJson(data, true));
                if (File.Exists(path))
                    File.Replace(temp, path, null);
                else
                    File.Move(temp, path);
                return true;
            }
            catch (Exception exception)
            {
                if (File.Exists(temp))
                {
                    try { File.Delete(temp); }
                    catch (Exception cleanupException) { Debug.LogWarning($"VANTA save temp cleanup failed: {cleanupException.Message}"); }
                }
                Debug.LogError($"VANTA save failed: {exception.Message}");
                return false;
            }
        }

        public SaveData Load(string slot)
        {
            var path = SlotPath(slot);
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
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

        public bool Exists(string slot)
        {
            var path = SlotPath(slot);
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        static string SanitizeSlot(string slot)
        {
            if (string.IsNullOrWhiteSpace(slot)) return null;
            var invalid = Path.GetInvalidFileNameChars();
            var chars = slot.Trim().ToCharArray();
            for (var i = 0; i < chars.Length; i++)
                if (Array.IndexOf(invalid, chars[i]) >= 0 || chars[i] == '/' || chars[i] == '\\')
                    chars[i] = '_';
            var safe = new string(chars).Trim('.', ' ');
            return string.IsNullOrWhiteSpace(safe) ? null : safe;
        }
    }
}
