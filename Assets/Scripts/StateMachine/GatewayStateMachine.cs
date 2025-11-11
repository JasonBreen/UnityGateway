using System.Collections;
using UnityEngine;

namespace Gateway.Visuals
{
    /// <summary>
    /// Coordinates the active <see cref="GatewayVisualState"/> based on a session timeline and audio cues.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class GatewayStateMachine : MonoBehaviour
    {
        [SerializeField]
        private GatewaySessionTimeline sessionTimeline = null;

        [SerializeField]
        private GatewayVisualController visualController = null;

        [SerializeField]
        [Tooltip("Automatically start the timeline when the component is enabled.")]
        private bool autoStart = true;

        [SerializeField]
        [Tooltip("Optional override for the AudioSource used to play the Gateway session.")]
        private AudioSource audioSourceOverride = null;

        private Coroutine activeRoutine;

        private AudioSource AudioSource => audioSourceOverride != null ? audioSourceOverride : GetComponent<AudioSource>();

        private void OnEnable()
        {
            if (autoStart)
            {
                StartSession();
            }
        }

        private void OnDisable()
        {
            StopSession();
        }

        public void StartSession()
        {
            StopSession();

            if (sessionTimeline == null)
            {
                Debug.LogWarning("GatewayStateMachine requires a session timeline.");
                return;
            }

            if (visualController == null)
            {
                Debug.LogWarning("GatewayStateMachine requires a visual controller reference.");
                return;
            }

            if (!AudioSource.isPlaying)
            {
                AudioSource.Play();
            }

            activeRoutine = StartCoroutine(RunTimeline());
        }

        public void StopSession()
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                activeRoutine = null;
            }

            if (AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }
        }

        private IEnumerator RunTimeline()
        {
            foreach (var segment in sessionTimeline.Segments)
            {
                if (segment.VisualState == null)
                {
                    continue;
                }

                visualController.ApplyState(segment.VisualState);

                var elapsed = 0f;
                while (elapsed < segment.DurationSeconds)
                {
                    visualController.Tick(elapsed / Mathf.Max(segment.DurationSeconds, 0.001f));
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            visualController.Clear();
        }
    }
}
