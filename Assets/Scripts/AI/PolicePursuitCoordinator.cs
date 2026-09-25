using UnityEngine;
using Vanta.Vehicles;

namespace Vanta.AI
{
    public sealed class PolicePursuitCoordinator : MonoBehaviour
    {
        readonly PursuitStateMachine pursuit = new();
        readonly VehiclePursuitTacticsModel vehicleTactics = new();

        [SerializeField] private Transform target;
        public PoliceEscalationLevel Escalation { get; private set; } = PoliceEscalationLevel.Patrol;
        public PursuitState PursuitState => pursuit.Current;
        public VehiclePursuitTactic VehicleTactic { get; private set; } = VehiclePursuitTactic.Pursue;

        public void Update(int wantedLevel)
        {
            Escalation = ResolveEscalation(wantedLevel);

            if (wantedLevel <= 0)
            {
                VehicleTactic = VehiclePursuitTactic.Pursue;
                pursuit.Reset();
                return;
            }

            bool targetInVehicle = target && target.GetComponent<VehicleController>() != null;
            VehicleTactic = vehicleTactics.Resolve(wantedLevel, targetInVehicle);

            if (pursuit.Current == PursuitState.Dormant || pursuit.Current == PursuitState.Cooldown)
                pursuit.BeginIntercept();
        }

        public void SetTarget(Transform newTarget) => target = newTarget;

        public static PoliceEscalationLevel ResolveEscalation(int wantedLevel)
        {
            return wantedLevel >= 5 ? PoliceEscalationLevel.Major :
                   wantedLevel >= 4 ? PoliceEscalationLevel.Tactical :
                   wantedLevel >= 2 ? PoliceEscalationLevel.Response :
                   PoliceEscalationLevel.Patrol;
        }
    }
}
