using System.Collections.Generic;
using UnityEngine;

namespace Gateway.Visuals
{
    /// <summary>
    /// Applies <see cref="GatewayVisualState"/> data to Unity materials and exposes hooks for AI modulation.
    /// </summary>
    public sealed class GatewayVisualController : MonoBehaviour
    {
        [SerializeField]
        private List<Material> targetMaterials = new();

        [SerializeField]
        [Tooltip("Optional mapping from animation parameter keys to material property names.")]
        private List<AnimationBinding> animationBindings = new();

        private readonly Dictionary<string, AnimationBinding> bindingLookup = new();
        private GatewayVisualState? activeState;

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
