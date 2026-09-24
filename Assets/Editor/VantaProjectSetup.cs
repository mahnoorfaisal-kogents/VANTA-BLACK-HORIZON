#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vanta.AI;
using Vanta.CameraSystem;
using Vanta.Combat;
using Vanta.Core;
using Vanta.Player;
using Vanta.UI;
using Vanta.Vehicles;
using Vanta.World;

namespace Vanta.EditorTools
{
    public static class VantaProjectSetup
    {
        private const string Root = "Assets/Generated";

        [MenuItem("VANTA/Build Playable Vertical Slice")]
        public static void BuildPlayableVerticalSlice()
        {
            EnsureFolder("Assets", "Generated");
            EnsureFolder(Root, "Materials");
            EnsureFolder(Root, "Weapons");

            var menu = CreateMainMenu();
            var district = CreateDistrict();
            CreateWeaponAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorSceneManager.SaveScene(menu, "Assets/Generated/MainMenu.unity");
            EditorSceneManager.SaveScene(district, "Assets/Generated/PlayableDistrict.unity");

            var scenes = new[] { menu.path, district.path };
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(scenes[0], true),
                new EditorBuildSettingsScene(scenes[1], true)
            };

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets/Generated/PlayableDistrict.unity");
            Debug.Log("VANTA: playable vertical slice generated. Runtime verification still required.");
        }

        private static Scene CreateMainMenu()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("VANTA_MainMenu");
            root.AddComponent<VantaMainMenu>();
            return scene;
        }

        private static Scene CreateDistrict()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var environment = new GameObject("Environment");
            CreateCube("Ground", environment.transform, Vector3.zero, new Vector3(80f, 0.2f, 80f), GetMaterial("Ground"));
            CreateRoads(environment.transform);
            for (var i = 0; i < DistrictLayout.BuildingCount; i++)
            {
                var size = new Vector3(10f, 7f + (i % 3) * 3f, 9f);
                CreateCube($"Building_{i:00}", environment.transform, DistrictLayout.BuildingPosition(i), size, GetMaterial("Building"));
            }

            var light = new GameObject("Sun");
            var directional = light.AddComponent<Light>();
            directional.type = LightType.Directional;
            directional.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var world = new GameObject("WorldSystems");
            world.AddComponent<WorldTimeSystem>();
            world.AddComponent<WeatherSystem>();
            world.AddComponent<WorldEventDirector>();
            world.AddComponent<WantedSystem>();
            world.AddComponent<FactionSystem>();
            world.AddComponent<InventorySystem>();
            world.AddComponent<EconomySystem>();
            world.AddComponent<MissionSystem>();
            world.AddComponent<SaveSystem>();
            world.AddComponent<GameSession>();

            var player = CreatePlayer();
            var camera = CreateCamera(player.transform);
            CreateEnemy(player.transform);
            CreateCivilians();
            CreateVehicles();

            var hud = new GameObject("HUD");
            var hudComponent = hud.AddComponent<VantaHud>();
            var hudSo = new SerializedObject(hudComponent);
            hudSo.FindProperty("playerHealth").objectReferenceValue = player.GetComponent<Health>();
            hudSo.ApplyModifiedPropertiesWithoutUndo();

            var bootstrap = world.AddComponent<WorldBootstrap>();
            var bootstrapSo = new SerializedObject(bootstrap);
            bootstrapSo.FindProperty("cameraRig").objectReferenceValue = camera.GetComponent<ThirdPersonCamera>();
            bootstrapSo.FindProperty("player").objectReferenceValue = player.transform;
            bootstrapSo.ApplyModifiedPropertiesWithoutUndo();

            return scene;
        }

        private static GameObject CreatePlayer()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Player";
            go.transform.position = DistrictLayout.PlayerSpawn;
            go.transform.localScale = new Vector3(0.9f, 1.1f, 0.9f);
            Object.DestroyImmediate(go.GetComponent<CapsuleCollider>());

            var controller = go.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.45f;
            controller.center = new Vector3(0f, 1f, 0f);

            go.AddComponent<PlayerController>();
            go.AddComponent<StealthSystem>();
            go.AddComponent<ParkourSystem>();

            var health = go.AddComponent<Health>();
            var weapon = go.AddComponent<WeaponController>();
            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(go.transform);
            muzzle.localPosition = new Vector3(0.35f, 1.15f, 0.75f);

            var camera = Camera.main;
            var weaponSo = new SerializedObject(weapon);
            weaponSo.FindProperty("aimCamera").objectReferenceValue = camera;
            weaponSo.FindProperty("muzzle").objectReferenceValue = muzzle;
            weaponSo.FindProperty("weapon").objectReferenceValue = AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Generated/Weapons/Sidearm.asset");
            weaponSo.ApplyModifiedPropertiesWithoutUndo();

            return go;
        }

        private static GameObject CreateCamera(GameObject player)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var camera = go.AddComponent<Camera>();
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 500f;
            var rig = go.AddComponent<ThirdPersonCamera>();
            rig.SetTarget(player.transform);
            return go;
        }

        private static void CreateEnemy(Transform player)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Enemy_Training";
            go.transform.position = DistrictLayout.EnemySpawn;
            go.transform.localScale = new Vector3(0.9f, 1.1f, 0.9f);
            var health = go.AddComponent<Health>();
            var ai = go.AddComponent<EnemyAI>();
            var so = new SerializedObject(ai);
            so.FindProperty("target").objectReferenceValue = player;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateCivilians()
        {
            for (var i = 0; i < DistrictLayout.CivilianCount; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.name = $"Civilian_{i:00}";
                go.transform.position = new Vector3(-22f + i * 8f, 1f, -2f);
                go.transform.localScale = new Vector3(0.65f, 0.9f, 0.65f);
                go.AddComponent<CivilianAI>();
            }
        }

        private static void CreateVehicles()
        {
            for (var i = 0; i < DistrictLayout.VehicleCount; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"Vehicle_{i:00}";
                go.transform.position = new Vector3(-24f + i * 16f, 0.8f, -26f);
                go.transform.localScale = new Vector3(2.2f, 0.8f, 4f);
                var body = go.AddComponent<Rigidbody>();
                body.mass = 1400f;
                body.drag = 0.2f;
                body.angularDrag = 0.5f;
                go.AddComponent<VehicleController>();
                go.AddComponent<VehicleInteraction>();
            }
        }

        private static void CreateRoads(Transform parent)
        {
            CreateCube("Road_Main", parent, new Vector3(0f, 0.11f, 0f), new Vector3(9f, 0.08f, 80f), GetMaterial("Road"));
            CreateCube("Road_Cross", parent, new Vector3(0f, 0.12f, 0f), new Vector3(80f, 0.08f, 9f), GetMaterial("Road"));
        }

        private static GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = position;
            go.transform.localScale = scale;
            if (material) go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static void CreateWeaponAssets()
        {
            CreateWeapon("Sidearm", WeaponType.Pistol, 25f, 6f, 12, 1.2f, 0.5f, 1);
            CreateWeapon("SMG", WeaponType.SMG, 16f, 12f, 30, 1.8f, 1.4f, 1);
            CreateWeapon("Rifle", WeaponType.AssaultRifle, 28f, 8f, 30, 2.0f, 0.8f, 1);
            CreateWeapon("Shotgun", WeaponType.Shotgun, 12f, 1.2f, 8, 2.2f, 4.5f, 8);
            CreateWeapon("Precision", WeaponType.PrecisionRifle, 75f, 1.5f, 5, 2.4f, 0.1f, 1);
        }

        private static void CreateWeapon(string id, WeaponType type, float damage, float rps, int magazine, float reload, float spread, int pellets)
        {
            var path = $"Assets/Generated/Weapons/{id}.asset";
            var data = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            if (!data) data = ScriptableObject.CreateInstance<WeaponData>();
            data.weaponId = id.ToLowerInvariant();
            data.weaponType = type;
            data.damage = damage;
            data.roundsPerSecond = rps;
            data.magazineSize = magazine;
            data.reloadSeconds = reload;
            data.spreadDegrees = spread;
            data.pellets = pellets;
            data.range = 140f;
            data.hitMask = ~0;
            if (!AssetDatabase.Contains(data))
                AssetDatabase.CreateAsset(data, path);
            EditorUtility.SetDirty(data);
        }

        private static Material GetMaterial(string id)
        {
            var path = $"Assets/Generated/Materials/{id}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material) return material;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            material = new Material(shader);
            material.name = id;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void EnsureFolder(string parent, string name)
        {
            var path = $"{parent}/{name}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, name);
        }
    }
}
