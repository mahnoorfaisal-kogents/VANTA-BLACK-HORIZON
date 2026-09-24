using System;
using UnityEngine;
using Vanta.Systems;
using Vanta.World;

namespace Vanta.AI
{
    public readonly struct AIDirectorResult
    {
        public readonly float Pressure;
        public readonly int EventTier;
        public AIDirectorResult(float pressure, int eventTier) { Pressure = pressure; EventTier = eventTier; }
    }

    public sealed class AIDirectorModel
    {
        readonly WorldSimulationDirector director;
        public AIDirectorModel(int seed) => director = new WorldSimulationDirector(seed);
        public AIDirectorResult Evaluate(float wantedHeat, float playerVisibility, float worldInstability)
        {
            var pressure = director.CalculatePressure(wantedHeat, playerVisibility, worldInstability);
            return new AIDirectorResult(pressure, director.ChooseEventTier(pressure));
        }
    }

    /// <summary>Scene-level systemic AI director. Pressure creates contextual events and resolves them deterministically.</summary>
    public sealed class AIDirector : MonoBehaviour
    {
        [SerializeField] WantedSystem wanted;
        [SerializeField] WorldEventSystem worldEvents;
        [SerializeField, Range(0f, 1f)] float worldInstability = 0.2f;
        [SerializeField] float eventCooldown = 20f;
        [SerializeField] float activeEventDuration = 10f;
        [SerializeField] int seed = 1337;
        Transform player;
        WorldSimulationDirector model;
        float cooldown;
        float activeRemaining;
        WorldEventRuntime activeRuntime;
        string activeEventId;

        public float Pressure { get; private set; }
        public int EventTier { get; private set; }
        public string ActiveEventId => activeEventId;
        public event Action<float, int> PressureChanged;
        public event Action<string, int> ContextualEventRequested;
        public event Action<string, WorldEventState> ContextualEventStateChanged;

        public void Initialize(WantedSystem wantedSystem, WorldEventSystem eventSystem)
        {
            wanted = wantedSystem;
            worldEvents = eventSystem;
            if (model == null)
                model = new WorldSimulationDirector(seed);
            if (!player)
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        void Awake()
        {
            if (model == null)
                model = new WorldSimulationDirector(seed);
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            cooldown = 0f;
        }

        void Update()
        {
            if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
            var visibility = player ? Mathf.Clamp01(1f / (1f + Vector3.Distance(transform.position, player.position) * 0.02f)) : 0f;
            var heat = wanted ? wanted.NormalizedHeat : 0f;
            var result = model == null ? new AIDirectorResult(0f, 0) : Evaluate(heat, visibility, worldInstability);
            Pressure = result.Pressure;
            EventTier = result.EventTier;
            PressureChanged?.Invoke(Pressure, EventTier);

            if (activeRuntime != null)
            {
                activeRemaining -= Time.deltaTime;
                if (activeRemaining <= 0f && worldEvents != null)
                {
                    if (worldEvents.Resolve(activeEventId, activeRuntime))
                        ContextualEventStateChanged?.Invoke(activeEventId, WorldEventState.Resolved);
                    activeRuntime = null;
                    activeEventId = null;
                    cooldown = Mathf.Max(0f, eventCooldown);
                }
                return;
            }

            cooldown -= Time.deltaTime;
            if (worldEvents && EventTier > 0 && cooldown <= 0f)
            {
                var id = "ai_director_tier_" + EventTier;
                var runtime = worldEvents.CreateRuntime(new WorldEventDefinition
                {
                    eventId = id,
                    durationHours = 0.5f,
                    rewardCash = EventTier * 50
                });
                if (worldEvents.Activate(id, runtime))
                {
                    activeRuntime = runtime;
                    activeEventId = id;
                    activeRemaining = Mathf.Max(0.1f, activeEventDuration);
                    ContextualEventRequested?.Invoke(id, EventTier);
                    ContextualEventStateChanged?.Invoke(id, WorldEventState.Active);
                }
            }
        }

        public AIDirectorResult Evaluate(float wantedHeat, float playerVisibility, float instability)
        {
            return model.Evaluate(wantedHeat, playerVisibility, instability);
        }
    }
}
