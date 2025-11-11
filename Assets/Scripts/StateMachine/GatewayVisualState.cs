using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gateway.Visuals
{
    /// <summary>
    /// Defines a single visual presentation that can be triggered by the Gateway state machine.
    /// </summary>
    [CreateAssetMenu(
        fileName = "GatewayVisualState",
        menuName = "Gateway/Visual State",
        order = 0)]
    public sealed class GatewayVisualState : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Human readable name for the focus level or cue this state represents.")]
        private string displayName = "Focus State";

        [SerializeField]
        [Tooltip("Material parameters that will be applied when this state activates.")]
        private List<MaterialParameter> materialParameters = new();

        [SerializeField]
        [Tooltip("Optional animation curves that can be driven over time or by AI signals.")]
        private List<AnimationParameter> animationParameters = new();

        public string DisplayName => displayName;
        public IReadOnlyList<MaterialParameter> MaterialParameters => materialParameters;
        public IReadOnlyList<AnimationParameter> AnimationParameters => animationParameters;
    }

    [Serializable]
    public sealed class MaterialParameter
    {
        [SerializeField]
        [Tooltip("Name of the shader property to change (e.g., _PulseSpeed).")]
        private string propertyName = string.Empty;

        [SerializeField]
        [Tooltip("Target value applied when the state is activated.")]
        private float value = 0f;

        public string PropertyName => propertyName;
        public float Value => value;
    }

    [Serializable]
    public sealed class AnimationParameter
    {
        [SerializeField]
        [Tooltip("Identifier used in code for matching incoming modulation signals.")]
        private string key = string.Empty;

        [SerializeField]
        [Tooltip("Default animation curve to evaluate over the state duration.")]
        private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public string Key => key;
        public AnimationCurve Curve => curve;
    }
}
