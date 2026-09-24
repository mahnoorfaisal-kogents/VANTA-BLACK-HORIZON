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

    /// <summary>Scene-level systemic AI director. It converts world pressure into contextual event requests.</summary>
    public sealed class AIDirector : MonoBehaviour
    {
        [SerializeField] WantedSystem wanted;
        [SerializeField] WorldEventSystem worldEvents;
        [SerializeField, Range(0f, 1f)] float worldInstability = 0.2f;
        [SerializeField] float eventCooldown = 20f;
        [SerializeField] int seed = 1337;
        Transform player;
        WorldSimulationDirector model;
        float cooldown;

        public float Pressure { get; private set; }
        public int EventTier { get; private set; }
        public event Action<float, int> PressureChanged;
        public event Action<string, int> ContextualEventRequested;

        void Awake()
        {
            model = new WorldSimulationDirector(seed);
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        void Update()
        {
            if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
            var visibility = player ? Mathf.Clamp01(1f / (1f + Vector3.Distance(transform.position, player.position) * 0.02f)) : 0f;
            var heat = wanted ? wanted.NormalizedHeat : 0f;
            var result = new AIDirectorResult(model.CalculatePressure(heat, visibility, worldInstability), 0);
            result = new AIDirectorResult(result.Pressure, model.ChooseEventTier(result.Pressure));
            Pressure = result.Pressure;
            EventTier = result.EventTier;
            PressureChanged?.Invoke(Pressure, EventTier);

            cooldown -= Time.deltaTime;
            if (worldEvents && EventTier > 0 && cooldown <= 0f)
            {
                var id = "ai_director_tier_" + EventTier;
                var runtime = worldEvents.CreateRuntime(new WorldEventDefinition { eventId = id, durationHours = 0.5f });
                if (worldEvents.Activate(id, runtime))
                {
                    ContextualEventRequested?.Invoke(id, EventTier);
                    cooldown = eventCooldown;
                }
            }
        }
    }
}
