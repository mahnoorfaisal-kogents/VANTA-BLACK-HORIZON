using UnityEngine;

namespace Vanta.Systems
{
    public sealed class BulletTimeSystem : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float duration = 2f;
        [SerializeField, Min(0f)] private float cooldown = 5f;
        [SerializeField, Range(0.05f, 1f)] private float timeScale = 0.25f;
        [SerializeField] private KeyCode activationKey = KeyCode.B;

        private BulletTimeModel model;
        private bool appliedSlowdown;

        public bool IsActive => model != null && model.IsActive;

        private void Awake()
        {
            model = new BulletTimeModel(duration, cooldown, timeScale);
        }

        private void Update()
        {
            var now = Time.unscaledTime;

            if (Input.GetKeyDown(activationKey))
                model.TryActivate(now);

            model.Tick(Time.unscaledDeltaTime, now);

            var desiredScale = model.CurrentTimeScale;
            if (!Mathf.Approximately(Time.timeScale, desiredScale))
                Time.timeScale = desiredScale;

            appliedSlowdown = model.IsActive;

            if (!appliedSlowdown && !Mathf.Approximately(Time.timeScale, 1f))
                Time.timeScale = 1f;
        }

        private void OnDisable()
        {
            if (Mathf.Abs(Time.timeScale - 1f) > 0.001f)
                Time.timeScale = 1f;
        }
    }
}
