using System;
using UnityEngine;

namespace Vanta.AI
{
    public enum SuspicionState { Unaware, Suspicious, Investigating, Alerted }

    public sealed class StealthPerceptionSystem : MonoBehaviour
    {
        [Range(0,100)] public float suspicion;
        public float suspicionThreshold = 100f;
        public float alertThreshold = 70f;
        public float decayPerSecond = 18f;
        public SuspicionState State { get; private set; } = SuspicionState.Unaware;
        public event Action<SuspicionState> StateChanged;

        public void AddSuspicion(float amount)
        {
            if (amount <= 0f || State == SuspicionState.Alerted) return;
            suspicion = Mathf.Clamp(suspicion + amount, 0f, suspicionThreshold);
            UpdateState();
        }

        public void ReduceSuspicion(float amount)
        {
            if (amount <= 0f || State == SuspicionState.Alerted) return;
            suspicion = Mathf.Max(0f, suspicion - amount);
            UpdateState();
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || State == SuspicionState.Alerted) return;
            ReduceSuspicion(decayPerSecond * deltaTime);
        }

        public void Alert()
        {
            suspicion = suspicionThreshold;
            SetState(SuspicionState.Alerted);
        }

        void UpdateState()
        {
            if (suspicion >= suspicionThreshold) SetState(SuspicionState.Alerted);
            else if (suspicion >= alertThreshold) SetState(SuspicionState.Investigating);
            else if (suspicion > 0f) SetState(SuspicionState.Suspicious);
            else SetState(SuspicionState.Unaware);
        }

        void SetState(SuspicionState next)
        {
            if (State == next) return;
            State = next;
            StateChanged?.Invoke(next);
        }
    }
}