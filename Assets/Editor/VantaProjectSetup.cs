#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vanta.AI;
using Vanta.CameraSystem;
using Vanta.Combat;
using Vanta.Core;
using Vanta.Player;
using Vanta.UI;
using Vanta.Systems;
using Vanta.Missions;
using Vanta.Save;
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

            CreateWeaponAssets();
            var menu = CreateMainMenu();
            var mission = CreateMissionAsset();
            var district = CreateDistrict(mission);
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

        private static Scene CreateDistrict(MissionDefinition mission)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var environment = new GameObject("Environment");
            var navSurface = environment.AddComponent<NavMeshSurface>();
            navSurface.collectObjects = CollectObjects.All;
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
            world.AddComponent<WorldEventSystem>();
            world.AddComponent<BulletTimeSystem>();
            world.AddComponent<DistrictRuntimeSystem>();
            world.AddComponent<DistrictStreamingSystem>();
            world.AddComponent<PerformanceBudgetSystem>();
            world.AddComponent<AIFrameBudgetSystem>();
            world.AddComponent<AgentSquadCoordinatorComponent>();
            world.AddComponent<AgentSquadKnowledgeCoordinator>();
            world.AddComponent<VantaSettingsSystem>();
            world.AddComponent<ActivitySystem>();
            world.AddComponent<InteractionSystem>();
            world.AddComponent<DialogueSystem>();
            world.AddComponent<DiscoverySystem>();
            world.AddComponent<ScoutingSystem>();
            world.AddComponent<IntelMapSystem>();
            world.AddComponent<PersistentWorldState>();
            var trafficSystem = world.AddComponent<TrafficSystem>();
            var trafficSlots = world.AddComponent<TrafficSlotCoordinator>();
            world.AddComponent<TrafficPopulationSystem>();
            world.AddComponent<NPCScheduleSystem>();
            var wantedSystem = world.AddComponent<WantedSystem>();
            var aiDirector = world.AddComponent<AIDirector>();
            var aiDirectorSo = new SerializedObject(aiDirector);
            aiDirectorSo.FindProperty("wanted").objectReferenceValue = wantedSystem;
            aiDirectorSo.FindProperty("worldEvents").objectReferenceValue = world.GetComponent<WorldEventSystem>();
            aiDirectorSo.ApplyModifiedPropertiesWithoutUndo();
            world.AddComponent<PoliceEscalationSystem>();
            var pursuitCoordinator = world.AddComponent<PolicePursuitCoordinator>();
            world.AddComponent<FactionSystem>();
            world.AddComponent<FactionTerritorySystem>();
            world.AddComponent<InventorySystem>();
            world.AddComponent<EconomySystem>();
            world.AddComponent<ProgressionSystem>();
            world.AddComponent<SafehouseGarageSystem>();
            world.AddComponent<MissionSystem>();
            world.AddComponent<MissionConsequenceSystem>();
            var saveSystem = world.AddComponent<SaveSystem>();
            var saveCoordinator = world.AddComponent<SaveGameCoordinator>();
            var saveSo = new SerializedObject(saveCoordinator);
            saveSo.FindProperty("saveSystem").objectReferenceValue = saveSystem;
            var session = world.AddComponent<GameSession>();
            var coordinator = world.AddComponent<GameWorldCoordinator>();
            var coordinatorSo = new SerializedObject(coordinator);
            coordinatorSo.FindProperty("wanted").objectReferenceValue = world.GetComponent<WantedSystem>();
            coordinatorSo.FindProperty("policeEscalation").objectReferenceValue = world.GetComponent<PoliceEscalationSystem>();
            coordinatorSo.FindProperty("pursuit").objectReferenceValue = world.GetComponent<PolicePursuitCoordinator>();
            coordinatorSo.FindProperty("missions").objectReferenceValue = world.GetComponent<MissionSystem>();
            coordinatorSo.FindProperty("consequences").objectReferenceValue = world.GetComponent<MissionConsequenceSystem>();
            coordinatorSo.FindProperty("factions").objectReferenceValue = world.GetComponent<FactionSystem>();
            coordinatorSo.FindProperty("territories").objectReferenceValue = world.GetComponent<FactionTerritorySystem>();
            coordinatorSo.FindProperty("economy").objectReferenceValue = world.GetComponent<EconomySystem>();
            coordinatorSo.FindProperty("progression").objectReferenceValue = world.GetComponent<ProgressionSystem>();
            coordinatorSo.FindProperty("intel").objectReferenceValue = world.GetComponent<IntelMapSystem>();

            var interactionDevices = CreateWorldInteractionDevices(world.transform);
            var devicesProperty = coordinatorSo.FindProperty("worldInteractionDevices");
            devicesProperty.arraySize = interactionDevices.Length;
            for (var i = 0; i < interactionDevices.Length; i++)
                devicesProperty.GetArrayElementAtIndex(i).objectReferenceValue = interactionDevices[i];
            coordinatorSo.ApplyModifiedPropertiesWithoutUndo();
            world.AddComponent<GameSession>();

            var player = CreatePlayer();
            saveSo.FindProperty("player").objectReferenceValue = player.transform;
            saveSo.FindProperty("economy").objectReferenceValue = world.GetComponent<EconomySystem>();
            saveSo.FindProperty("wanted").objectReferenceValue = world.GetComponent<WantedSystem>();
            saveSo.FindProperty("progression").objectReferenceValue = world.GetComponent<ProgressionSystem>();
            saveSo.FindProperty("worldTime").objectReferenceValue = world.GetComponent<WorldTimeSystem>();
            saveSo.FindProperty("missions").objectReferenceValue = world.GetComponent<MissionSystem>();
            saveSo.FindProperty("missionDefinitions").arraySize = 1;
            saveSo.FindProperty("missionDefinitions").GetArrayElementAtIndex(0).objectReferenceValue = mission;
            saveSo.ApplyModifiedPropertiesWithoutUndo();
            var sessionSo = new SerializedObject(session);
            sessionSo.FindProperty("playerHealth").objectReferenceValue = player.GetComponent<Health>();
            sessionSo.ApplyModifiedPropertiesWithoutUndo();

            var camera = CreateCamera(player);
            ConfigureWeapon(player.GetComponent<WeaponController>(), player.transform.Find("Muzzle"), camera.GetComponent<Camera>());
            CreateEnemy(player.transform);
            CreatePolice(player.transform, world.GetComponent<WantedSystem>());
            CreatePolicePursuitVehicle(world.transform, player.transform, pursuitCoordinator, trafficSlots);
            CreateDiscoveryPoints();
            CreateGrappleAnchors(world.transform);
            CreateCivilians(player.transform);
            CreateVehicles();
            CreateTrafficRuntime(world.transform, trafficSystem, world.GetComponent<TrafficPopulationSystem>());
            navSurface.BuildNavMesh();

            CreateMissionRuntime(world.transform, mission);

            var saveInput = new GameObject("SaveInput");
            var saveController = saveInput.AddComponent<VantaSaveController>();
            saveController.Configure(saveCoordinator, "slot_01");

            var pause = new GameObject("PauseController");
            var pauseController = pause.AddComponent<VantaPauseController>();
            pauseController.Configure(session);
            var pauseSo = new SerializedObject(pauseController);
            pauseSo.FindProperty("session").objectReferenceValue = world.GetComponent<GameSession>();
            pauseSo.ApplyModifiedPropertiesWithoutUndo();

            var hud = new GameObject("HUD");
            var hudComponent = hud.AddComponent<VantaHud>();
            var hudSo = new SerializedObject(hudComponent);
            hudSo.FindProperty("playerHealth").objectReferenceValue = player.GetComponent<Health>();
            hudSo.FindProperty("weapon").objectReferenceValue = player.GetComponent<WeaponController>();
            hudSo.FindProperty("wanted").objectReferenceValue = world.GetComponent<WantedSystem>();
            hudSo.FindProperty("interaction").objectReferenceValue = player.GetComponent<WorldInteractionInteractor>();
            hudSo.ApplyModifiedPropertiesWithoutUndo();

            var bootstrap = world.AddComponent<WorldBootstrap>();
            var bootstrapSo = new SerializedObject(bootstrap);
            bootstrapSo.FindProperty("cameraRig").objectReferenceValue = camera.GetComponent<ThirdPersonCamera>();
            bootstrapSo.FindProperty("player").objectReferenceValue = player.transform;
            bootstrapSo.ApplyModifiedPropertiesWithoutUndo();

            return scene;
        }

        private static WorldInteractionDevice[] CreateWorldInteractionDevices(Transform parent)
        {
            var definitions = new[]
            {
                ("traffic_01", WorldInteractionDeviceType.TrafficLight, new Vector3(8f, 1f, 18f)),
                ("camera_01", WorldInteractionDeviceType.Camera, new Vector3(18f, 4f, 18f)),
                ("gate_01", WorldInteractionDeviceType.SecurityGate, new Vector3(28f, 1f, 8f)),
                ("alarm_01", WorldInteractionDeviceType.Alarm, new Vector3(-10f, 2f, 20f)),
                ("bridge_01", WorldInteractionDeviceType.Bridge, new Vector3(-24f, 1f, -8f))
            };

            var devices = new WorldInteractionDevice[definitions.Length];
            for (var i = 0; i < definitions.Length; i++)
            {
                var (id, type, position) = definitions[i];
                var size = type == WorldInteractionDeviceType.Camera
                    ? new Vector3(0.7f, 1.5f, 0.7f)
                    : new Vector3(1.2f, 1.2f, 1.2f);
                var go = CreateCube($"Interaction_{id}", parent, position, size, GetMaterial("Building"));
                var device = go.AddComponent<WorldInteractionDevice>();
                device.Configure(id, type, true);
                var deviceSo = new SerializedObject(device);
                deviceSo.FindProperty("wanted").objectReferenceValue = parent.GetComponentInParent<WantedSystem>();
                deviceSo.ApplyModifiedPropertiesWithoutUndo();
                devices[i] = device;
            }

            return devices;
        }

        private static GameObject CreatePlayer()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Player";
            go.tag = "Player";
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
            go.AddComponent<GrappleTraversalSystem>();
            go.AddComponent<WorldInteractionInteractor>();
            go.AddComponent<MeleeCombatSystem>();
            go.AddComponent<VehicleInteractor>();

            var health = go.AddComponent<Health>();
            var weapon = go.AddComponent<WeaponController>();
            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(go.transform);
            muzzle.localPosition = new Vector3(0.35f, 1.15f, 0.75f);

            ConfigureWeapon(weapon, muzzle, null);

            return go;
        }

        private static void ConfigureWeapon(WeaponController weapon, Transform muzzle, Camera aimCamera)
        {
            if (!weapon) return;
            var weaponSo = new SerializedObject(weapon);
            if (aimCamera) weaponSo.FindProperty("aimCamera").objectReferenceValue = aimCamera;
            weaponSo.FindProperty("muzzle").objectReferenceValue = muzzle;
            weaponSo.FindProperty("weapon").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Generated/Weapons/Sidearm.asset");
            weaponSo.ApplyModifiedPropertiesWithoutUndo();
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
            go.AddComponent<VantaAgentBrain>();
            var so = new SerializedObject(ai);
            so.FindProperty("target").objectReferenceValue = player;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreatePolicePursuitVehicle(
            Transform parent,
            Transform player,
            PolicePursuitCoordinator coordinator,
            TrafficSlotCoordinator trafficSlots)
        {
            var go = CreateCube(
                "Police_PursuitVehicle",
                parent,
                new Vector3(34f, 0.9f, 30f),
                new Vector3(2.2f, 0.9f, 4.2f),
                GetMaterial("PoliceVehicle"));
            var body = go.AddComponent<Rigidbody>();
            body.mass = 1500f;
            body.drag = 0.2f;
            body.angularDrag = 0.5f;

            var runtime = go.AddComponent<PolicePursuitVehicleRuntime>();
            var runtimeSo = new SerializedObject(runtime);
            runtimeSo.FindProperty("target").objectReferenceValue = player;
            runtimeSo.FindProperty("trafficSlots").objectReferenceValue = trafficSlots;
            runtimeSo.ApplyModifiedPropertiesWithoutUndo();

            var coordinatorSo = new SerializedObject(coordinator);
            coordinatorSo.FindProperty("target").objectReferenceValue = player;
            coordinatorSo.FindProperty("pursuitVehicle").objectReferenceValue = runtime;
            coordinatorSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreatePolice(Transform player, WantedSystem wanted)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Police_Interceptor";
            go.transform.position = new Vector3(26f, 1f, 18f);
            go.transform.localScale = new Vector3(1.4f, 1.2f, 2.2f);
            var ai = go.AddComponent<PoliceAI>();
            go.AddComponent<VantaAgentBrain>();
            var so = new SerializedObject(ai);
            so.FindProperty("wanted").objectReferenceValue = wanted;
            so.FindProperty("target").objectReferenceValue = player;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static MissionDefinition CreateMissionAsset()
        {
            var path = "Assets/Generated/Missions/FirstContact.asset";
            EnsureFolder("Assets/Generated", "Missions");
            var mission = AssetDatabase.LoadAssetAtPath<MissionDefinition>(path);
            if (!mission) mission = ScriptableObject.CreateInstance<MissionDefinition>();
            mission.missionId = "first_contact";
            mission.title = "FIRST CONTACT";
            mission.briefing = "Reach the marked location, inspect the signal, then extract through the safe route.";
            mission.rewardCash = 250;
            mission.objectiveGraph = new[]
            {
                new MissionObjectiveNode { id = "reach_signal", title = "Reach the signal", approaches = new[] { "stealth", "direct" } },
                new MissionObjectiveNode { id = "inspect_signal", title = "Inspect the signal", prerequisites = new[] { "reach_signal" }, approaches = new[] { "interact" } },
                new MissionObjectiveNode { id = "extract", title = "Extract from the district", prerequisites = new[] { "inspect_signal" }, approaches = new[] { "stealth", "direct" } }
            };
            mission.consequence = new MissionConsequence { cash = 250, xp = 150, factionId = "vanta", reputationDelta = 5, territoryId = "sector_01", territoryDelta = 3, wantedHeat = 0.15f, revealIds = new[] { "signal_01" } };
            if (!AssetDatabase.Contains(mission)) AssetDatabase.CreateAsset(mission, path);
            EditorUtility.SetDirty(mission);
            return mission;
        }

        private static void CreateMissionRuntime(Transform parent, MissionDefinition mission)
        {
            var root = new GameObject("MissionRuntime");
            root.transform.SetParent(parent);
            var bootstrap = root.AddComponent<VerticalSliceMissionBootstrap>();
            var system = parent.GetComponent<MissionSystem>();
            var bootstrapSo = new SerializedObject(bootstrap);
            bootstrapSo.FindProperty("missionSystem").objectReferenceValue = system;
            bootstrapSo.FindProperty("missionDefinition").objectReferenceValue = mission;
            bootstrapSo.ApplyModifiedPropertiesWithoutUndo();

            var triggerSet = root.AddComponent<MissionObjectiveTriggerSet>();
            var setSo = new SerializedObject(triggerSet);
            setSo.FindProperty("missionSystem").objectReferenceValue = system;
            setSo.FindProperty("missionId").stringValue = mission.missionId;
            setSo.FindProperty("objectiveIds").arraySize = 3;
            for (var i = 0; i < 3; i++) setSo.FindProperty("objectiveIds").GetArrayElementAtIndex(i).stringValue = mission.objectiveGraph[i].id;
            setSo.ApplyModifiedPropertiesWithoutUndo();

            var points = new[] { new Vector3(24f, 1f, 24f), new Vector3(30f, 1f, 34f), new Vector3(-18f, 1f, -12f) };
            for (var i = 0; i < points.Length; i++)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                marker.name = $"MissionObjective_{i:00}";
                marker.transform.SetParent(root.transform);
                marker.transform.position = points[i];
                marker.transform.localScale = new Vector3(1.2f, 0.25f, 1.2f);
                var collider = marker.GetComponent<Collider>();
                collider.isTrigger = true;
                var trigger = marker.AddComponent<MissionObjectiveTrigger>();
                var triggerSo = new SerializedObject(trigger);
                triggerSo.FindProperty("missionSystem").objectReferenceValue = system;
                triggerSo.FindProperty("missionId").stringValue = mission.missionId;
                triggerSo.FindProperty("objectiveId").stringValue = mission.objectiveGraph[i].id;
                triggerSo.FindProperty("consumeOnComplete").boolValue = true;
                triggerSo.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void CreateGrappleAnchors(Transform parent)
        {
            var points = new[]
            {
                new Vector3(24f, 8f, 24f),
                new Vector3(-24f, 10f, 24f),
                new Vector3(24f, 12f, -24f),
                new Vector3(-24f, 9f, -24f)
            };

            for (var i = 0; i < points.Length; i++)
            {
                var anchorObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                anchorObject.name = $"GrappleAnchor_{i:00}";
                anchorObject.transform.SetParent(parent);
                anchorObject.transform.position = points[i];
                anchorObject.transform.localScale = Vector3.one * 0.35f;
                anchorObject.AddComponent<GrappleAnchor>().name = $"GrappleAnchor_{i:00}";
            }
        }

        private static void CreateDiscoveryPoints()
        {
            var points = new[]
            {
                new Vector3(-18f, 0.5f, -12f),
                new Vector3(18f, 0.5f, -12f),
                new Vector3(30f, 0.5f, 34f)
            };

            for (var i = 0; i < points.Length; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                go.name = $"DiscoveryPoint_{i:00}";
                go.transform.position = points[i];
                go.transform.localScale = new Vector3(0.8f, 0.15f, 0.8f);
                var discovery = go.AddComponent<DiscoveryPoint>();
                var so = new SerializedObject(discovery);
                so.FindProperty("discoveryId").stringValue = $"sector_landmark_{i:00}";
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void CreateCivilians(Transform player)
        {
            for (var i = 0; i < DistrictLayout.CivilianCount; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.name = $"Civilian_{i:00}";
                go.transform.position = new Vector3(-22f + i * 8f, 1f, -2f);
                go.transform.localScale = new Vector3(0.65f, 0.9f, 0.65f);
                var civilian = go.AddComponent<CivilianAI>();
                civilian.SetThreat(player);
                go.AddComponent<VantaAgentBrain>();
            }
        }

        private static void CreateTrafficRuntime(
            Transform parent,
            TrafficSystem trafficSystem,
            TrafficPopulationSystem population)
        {
            var route = new TrafficRoute
            {
                routeId = "district_loop",
                speedLimit = 10f,
                points = new[]
                {
                    new Vector3(-34f, 0.9f, -34f),
                    new Vector3(34f, 0.9f, -34f),
                    new Vector3(34f, 0.9f, 34f),
                    new Vector3(-34f, 0.9f, 34f)
                }
            };
            trafficSystem.Register(route);

            for (var i = 0; i < 6; i++)
            {
                var id = $"traffic_{i:00}";
                if (population && !population.TrySpawn(id))
                    continue;

                var go = CreateCube(
                    $"Traffic_{i:00}",
                    parent,
                    route.points[i % route.points.Length],
                    new Vector3(1.8f, 0.8f, 3.2f),
                    GetMaterial("Vehicle"));
                var body = go.AddComponent<Rigidbody>();
                body.mass = 1100f;
                body.isKinematic = true;

                var runtime = go.AddComponent<TrafficVehicleRuntime>();
                runtime.Configure(trafficSystem, route.routeId);
                runtime.ConfigurePopulation(population, id);
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
                var interaction = go.AddComponent<VehicleInteraction>();
                var seat = new GameObject("DriverSeat").transform;
                seat.SetParent(go.transform);
                seat.localPosition = new Vector3(0f, 0.9f, 0f);
                var interactionSo = new SerializedObject(interaction);
                interactionSo.FindProperty("seat").objectReferenceValue = seat;
                interactionSo.ApplyModifiedPropertiesWithoutUndo();
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
