using UnityEngine;

namespace Vanta.World
{
    public readonly struct WorldRiskResult
    {
        public readonly float RiskMultiplier;
        public readonly float RewardMultiplier;
        public readonly float PursuitPressure;

        public WorldRiskResult(float riskMultiplier, float rewardMultiplier, float pursuitPressure)
        {
            RiskMultiplier = riskMultiplier;
            RewardMultiplier = rewardMultiplier;
            PursuitPressure = pursuitPressure;
        }
    }

    public sealed class WorldRiskModel
    {
        public WorldRiskResult Evaluate(bool isNight, float heat)
        {
            heat = Mathf.Clamp(heat, 0f, 5f);
            var nightRisk = isNight ? 1.25f : 1f;
            var heatRisk = 1f + heat * 0.12f;
            var risk = Mathf.Min(2f, nightRisk * heatRisk);
            var reward = isNight ? 1.35f + heat * 0.05f : 1f;
            var pressure = Mathf.Clamp01((isNight ? 0.2f : 0f) + heat / 5f);
            return new WorldRiskResult(risk, reward, pressure);
        }
    }
}
