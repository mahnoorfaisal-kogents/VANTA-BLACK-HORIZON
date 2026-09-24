using UnityEngine;

namespace Vanta.AI
{
    public sealed class PolicePursuitCoordinator : MonoBehaviour
    {
        readonly PursuitStateMachine pursuit = new();
        public PoliceEscalationLevel Escalation { get; private set; } = PoliceEscalationLevel.Patrol;
        public PursuitState PursuitState => pursuit.Current;

        public void Update(int wantedLevel)
        {
            Escalation = ResolveEscalation(wantedLevel);
            if (wantedLevel <= 0) pursuit.Reset();
            else if (pursuit.Current == PursuitState.Dormant || pursuit.Current == PursuitState.Cooldown)
                pursuit.BeginIntercept();
        }

        public static PoliceEscalationLevel ResolveEscalation(int wantedLevel)
        {
            return wantedLevel >= 5 ? PoliceEscalationLevel.Major :
                   wantedLevel >= 4 ? PoliceEscalationLevel.Tactical :
                   wantedLevel >= 2 ? PoliceEscalationLevel.Response :
                   PoliceEscalationLevel.Patrol;
        }
    }
}