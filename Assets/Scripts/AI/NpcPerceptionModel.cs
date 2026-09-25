using System;

namespace Vanta.AI
{
    public readonly struct NpcPerceptionResult
    {
        public readonly bool Detected;
        public readonly bool Investigating;
        public readonly bool Alerted;
        public readonly float Confidence;

        public NpcPerceptionResult(bool detected, bool investigating, bool alerted, float confidence)
        {
            Detected = detected;
            Investigating = investigating;
            Alerted = alerted;
            Confidence = confidence;
        }
    }

    public sealed class NpcPerceptionModel
    {
        public NpcPerceptionResult Evaluate(
            float distance,
            float detectionRange,
            float suspicion,
            bool isHidden,
            bool wantedActive)
        {
            if (distance < 0f)
                throw new ArgumentOutOfRangeException(nameof(distance));
            if (detectionRange <= 0f)
                throw new ArgumentOutOfRangeException(nameof(detectionRange));

            suspicion = Clamp01(suspicion);
            var inRange = distance <= detectionRange;

            if (wantedActive)
                return new NpcPerceptionResult(
                    detected: true,
                    investigating: false,
                    alerted: true,
                    confidence: 1f);

            if (!inRange || isHidden && suspicion < 0.35f)
                return new NpcPerceptionResult(false, false, false, 0f);

            var rangeFactor = 1f - Math.Min(1f, distance / detectionRange);
            var confidence = Clamp01(suspicion * 0.75f + rangeFactor * 0.25f);
            var investigating = suspicion >= 0.35f;
            var alerted = suspicion >= 0.75f;

            return new NpcPerceptionResult(
                detected: investigating || alerted,
                investigating: investigating && !alerted,
                alerted: alerted,
                confidence: confidence);
        }

        static float Clamp01(float value) => Math.Max(0f, Math.Min(1f, value));
    }
}
