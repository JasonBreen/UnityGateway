using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gateway.Visuals
{
    /// <summary>
    /// Scriptable object that describes the chronological sequence of visual states
    /// for a Gateway audio session. The timeline is consumed by <see cref="GatewayStateMachine"/>.
    /// </summary>
    [CreateAssetMenu(
        fileName = "GatewaySessionTimeline",
        menuName = "Gateway/Session Timeline",
        order = 1)]
    public sealed class GatewaySessionTimeline : ScriptableObject
    {
        [SerializeField]
        private List<TimelineSegment> segments = new List<TimelineSegment>();

        public IReadOnlyList<TimelineSegment> Segments => segments;
    }

    [Serializable]
    public sealed class TimelineSegment
    {
        [SerializeField]
        private GatewayVisualState visualState = null;

        [SerializeField]
        [Tooltip("Duration of this focus level segment in seconds.")]
        private float durationSeconds = 60f;

        [SerializeField]
        [Tooltip("Optional audio cue timestamp (in seconds) to align with the segment start.")]
        private float cueTimestampSeconds = 0f;

        public GatewayVisualState VisualState => visualState;
        public float DurationSeconds => durationSeconds;
        public float CueTimestampSeconds => cueTimestampSeconds;
    }
}
