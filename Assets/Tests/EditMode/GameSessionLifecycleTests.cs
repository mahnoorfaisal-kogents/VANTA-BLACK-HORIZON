using NUnit.Framework;
using UnityEngine;
using Vanta.Core;

namespace Vanta.Tests
{
    public sealed class GameSessionLifecycleTests
    {
        [Test]
        public void StateChangedObserversSeeUpdatedTimeScale()
        {
            var previousScale = Time.timeScale;
            var go = new GameObject("GameSessionLifecycleTest");
            try
            {
                var session = go.AddComponent<GameSession>();
                Assert.That(session.StartGame(), Is.True);

                float observedScale = -1f;
                session.StateChanged += (_, next) =>
                {
                    if (next == GameplayState.Paused || next == GameplayState.Playing)
                        observedScale = Time.timeScale;
                };

                Assert.That(session.PauseGame(), Is.True);
                Assert.That(observedScale, Is.EqualTo(0f));

                Assert.That(session.ResumeGame(), Is.True);
                Assert.That(observedScale, Is.EqualTo(1f));
            }
            finally
            {
                Object.DestroyImmediate(go);
                Time.timeScale = previousScale;
            }
        }
    }
}
