namespace Vanta.Player
{
    public enum StealthAlertState
    {
        Unaware,
        Investigating,
        Alerted
    }

    public readonly struct StealthPerceptionResult
    {
        public readonly float Suspicion;
        public readonly bool IsHidden;

        public StealthPerceptionResult(float suspicion, bool isHidden)
        {
            Suspicion = suspicion;
            IsHidden = isHidden;
        }
    }

    public sealed class StealthPerceptionModel
    {
        public StealthPerceptionResult Evaluate(float visibility, float noise, float lightExposure, bool crouched)
        {
            visibility = Mathf.Clamp01(visibility);
            noise = Mathf.Clamp01(noise);
            lightExposure = Mathf.Clamp01(lightExposure);

            var movementFactor = crouched ? 0.55f : 1f;
            var suspicion = Mathf.Clamp01(
                visibility * 0.45f +
                noise * movementFactor * 0.30f +
                lightExposure * 0.25f);

            return new StealthPerceptionResult(suspicion, suspicion < 0.30f);
        }
    }

    public sealed class StealthSuspicionState
    {
        readonly float investigateThreshold;
        readonly float alertThreshold;

        public StealthAlertState Current { get; private set; }

        public StealthSuspicionState(float investigateThreshold, float alertThreshold)
        {
            if (investigateThreshold < 0f || investigateThreshold >= alertThreshold)
                throw new ArgumentOutOfRangeException(nameof(investigateThreshold));
            if (alertThreshold > 1f)
                throw new ArgumentOutOfRangeException(nameof(alertThreshold));

            this.investigateThreshold = investigateThreshold;
            this.alertThreshold = alertThreshold;
            Current = StealthAlertState.Unaware;
        }

        public StealthAlertState Update(float suspicion)
        {
            suspicion = Mathf.Clamp01(suspicion);

            if (Current == StealthAlertState.Alerted)
                return Current;

            if (suspicion >= alertThreshold)
                Current = StealthAlertState.Alerted;
            else if (suspicion >= investigateThreshold)
                Current = StealthAlertState.Investigating;
            else
                Current = StealthAlertState.Unaware;

            return Current;
        }

        public void Reset() => Current = StealthAlertState.Unaware;
    }
}
