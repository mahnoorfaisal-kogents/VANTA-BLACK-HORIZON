using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vanta.World
{
    public enum DistrictStreamState { Unloaded, Loading, Loaded, Unloading }

    [Serializable]
    public sealed class DistrictStreamDefinition
    {
        public string districtId;
        public string sceneName;
    }

    public sealed class DistrictStreamingSystem : MonoBehaviour
    {
        [SerializeField] DistrictStreamDefinition[] districts = Array.Empty<DistrictStreamDefinition>();
        readonly Dictionary<string, DistrictStreamState> states = new();
        readonly Dictionary<string, AsyncOperation> operations = new();
        readonly Dictionary<string, string> sceneNames = new();

        public event Action<string, DistrictStreamState> StateChanged;

        void Awake()
        {
            foreach (var definition in districts)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.districtId)) continue;
                if (!string.IsNullOrWhiteSpace(definition.sceneName))
                    sceneNames[definition.districtId] = definition.sceneName;
            }
        }

        public DistrictStreamState GetState(string districtId) =>
            states.TryGetValue(districtId, out var state) ? state : DistrictStreamState.Unloaded;

        public bool BeginLoad(string districtId)
        {
            if (string.IsNullOrWhiteSpace(districtId) || GetState(districtId) != DistrictStreamState.Unloaded) return false;
            return Set(districtId, DistrictStreamState.Loading);
        }

        public bool CompleteLoad(string districtId)
        {
            if (GetState(districtId) != DistrictStreamState.Loading) return false;
            return Set(districtId, DistrictStreamState.Loaded);
        }

        public bool BeginUnload(string districtId)
        {
            if (GetState(districtId) != DistrictStreamState.Loaded) return false;
            return Set(districtId, DistrictStreamState.Unloading);
        }

        public bool CompleteUnload(string districtId)
        {
            if (GetState(districtId) != DistrictStreamState.Unloading) return false;
            return Set(districtId, DistrictStreamState.Unloaded);
        }

        public bool TryGetSceneName(string districtId, out string sceneName) =>
            sceneNames.TryGetValue(districtId, out sceneName);

        public bool LoadDistrictAsync(string districtId)
        {
            if (!TryGetSceneName(districtId, out var sceneName) || !BeginLoad(districtId)) return false;
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (operation == null)
            {
                Set(districtId, DistrictStreamState.Unloaded);
                return false;
            }

            operations[districtId] = operation;
            operation.completed += _ =>
            {
                operations.Remove(districtId);
                if (GetState(districtId) == DistrictStreamState.Loading)
                    CompleteLoad(districtId);
            };
            return true;
        }

        public bool UnloadDistrictAsync(string districtId)
        {
            if (!TryGetSceneName(districtId, out var sceneName) || !BeginUnload(districtId)) return false;
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                CompleteUnload(districtId);
                return false;
            }

            var operation = SceneManager.UnloadSceneAsync(scene);
            if (operation == null)
            {
                Set(districtId, DistrictStreamState.Loaded);
                return false;
            }

            operations[districtId] = operation;
            operation.completed += _ =>
            {
                operations.Remove(districtId);
                if (GetState(districtId) == DistrictStreamState.Unloading)
                    CompleteUnload(districtId);
            };
            return true;
        }

        bool Set(string id, DistrictStreamState state)
        {
            states[id] = state;
            StateChanged?.Invoke(id, state);
            return true;
        }
    }
}