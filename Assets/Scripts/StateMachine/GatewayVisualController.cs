using System.Collections.Generic;
using UnityEngine;

namespace Gateway.Visuals
{
    /// <summary>
    /// Applies <see cref="GatewayVisualState"/> data to Unity materials and exposes hooks for AI modulation.
    /// Manages a list of target materials and updates their shader properties based on the active visual state.
    /// Can be driven by the state machine's timeline or by external AI systems via the Tick() method.
    /// </summary>
    public sealed class GatewayVisualController : MonoBehaviour
    {
        [SerializeField]
        private List<Material> targetMaterials = new List<Material>();

        [SerializeField]
        [Tooltip("Optional mapping from animation parameter keys to material property names.")]
        private List<AnimationBinding> animationBindings = new List<AnimationBinding>();

        private readonly Dictionary<string, AnimationBinding> bindingLookup = new Dictionary<string, AnimationBinding>();
        private GatewayVisualState activeState;

        private void Awake()
        {
            bindingLookup.Clear();
            foreach (var binding in animationBindings)
            {
                if (!string.IsNullOrEmpty(binding.ParameterKey))
                {
                    bindingLookup[binding.ParameterKey] = binding;
                }
            }
        }

        public void ApplyState(GatewayVisualState state)
        {
            activeState = state;

            foreach (var material in targetMaterials)
            {
                if (material == null)
                {
                    continue;
                }

                foreach (var parameter in state.MaterialParameters)
                {
                    material.SetFloat(parameter.PropertyName, parameter.Value);
                }
            }
        }

        /// <summary>
        /// Called by the <see cref="GatewayStateMachine"/> each frame with normalized progress (0-1).
        /// External AI systems can also call this with custom values to drive the same parameters.
        /// </summary>
        public void Tick(float normalizedProgress)
        {
            if (activeState == null)
            {
                return;
            }

            foreach (var animation in activeState.AnimationParameters)
            {
                if (!bindingLookup.TryGetValue(animation.Key, out var binding))
                {
                    continue;
                }

                var value = animation.Curve.Evaluate(Mathf.Clamp01(normalizedProgress));

                foreach (var material in targetMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    material.SetFloat(binding.PropertyName, value);
                }
            }
        }

        public void Clear()
        {
            activeState = null;
        }
    }

    [System.Serializable]
    public sealed class AnimationBinding
    {
        [SerializeField]
        private string parameterKey = string.Empty;

        [SerializeField]
        private string propertyName = string.Empty;

        public string ParameterKey => parameterKey;
        public string PropertyName => propertyName;
    }
}
