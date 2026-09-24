using UnityEngine;
using Vanta.Systems;

namespace Vanta.AI
{
    public sealed class PolicePursuitCoordinator : MonoBehaviour
    {
        readonly PoliceEscalationSystem escalation = new();
        readonly PursuitStateMachine pursuit = new();
        public PoliceEscalationLevel Escalation => escalation.Level;
        public PursuitStateMachine.State PursuitState => pursuit.Current;
        public void Update(int wantedLevel)
        {
            escalation.UpdateFromWantedLevel(wantedLevel);
            if (wantedLevel <= 0) pursuit.Set(PursuitStateMachine.State.Dormant);
            else if (pursuit.Current == PursuitStateMachine.State.Dormant ||
                     pursuit.Current == PursuitStateMachine.State.Cooldown)
                pursuit.Set(PursuitStateMachine.State.Intercepting);
        }
    }
}