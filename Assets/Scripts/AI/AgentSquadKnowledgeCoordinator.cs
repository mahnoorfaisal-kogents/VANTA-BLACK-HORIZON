using UnityEngine;

namespace Vanta.AI
{
    /// <summary>Periodically shares selected high-value knowledge from the squad leader brain to nearby squad brains.</summary>
    public sealed class AgentSquadKnowledgeCoordinator : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] float refreshSeconds = 2f;
        [SerializeField, Min(1)] int maxMembers = 8;
        float timer;
        readonly AgentKnowledgePropagationSystem propagation = new();

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            timer = refreshSeconds;

            var brains = GetComponentsInChildren<VantaAgentBrain>(true);
            if (brains.Length == 0) return;
            var limit = Mathf.Min(maxMembers, brains.Length);
            VantaAgentBrain leader = null;
            var leaderPriority = float.MinValue;
            for (var i = 0; i < limit; i++)
            {
                var brain = brains[i];
                if (!brain) continue;
                var priority = brain.LastAction == AgentAction.Pursue ? 1f : 0f;
                if (priority > leaderPriority)
                {
                    leader = brain;
                    leaderPriority = priority;
                }
            }
            if (!leader || leader.Knowledge == null) return;

            var keys = new[] { "last_known_player", "player_lost" };
            for (var i = 0; i < limit; i++)
            {
                var member = brains[i];
                if (!member || member == leader || member.Knowledge == null) continue;
                for (var k = 0; k < keys.Length; k++)
                    propagation.Share(leader.Knowledge, member.Knowledge, keys[k]);
            }
        }
    }
}
