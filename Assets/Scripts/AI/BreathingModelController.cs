using System;
using System.Collections.Generic;
using Unity.Sentis;
using UnityEngine;

namespace Gateway.AI
{
    /// <summary>
    /// Runs a local Sentis model to estimate breathing or relaxation metrics from audio input.
    /// The output is broadcast to listeners interested in modulation signals.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class BreathingModelController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("ONNX model asset referenced by Sentis (placed in a Resources folder or assigned in inspector).")]
        private ModelAsset modelAsset = null;

        [SerializeField]
        [Tooltip("Smoothing factor applied to the raw model output.")]
        private float smoothing = 0.1f;

        [SerializeField]
        [Tooltip("Event invoked whenever the normalized breath metric updates.")]
        private UnityEngine.Events.UnityEvent<float> onBreathMetric = new UnityEngine.Events.UnityEvent<float>();

        private Model runtimeModel = null;
        private IWorker worker = null;
        private TensorFloat inputTensor = null;
        private float smoothedValue;
        private readonly Queue<float> sampleBuffer = new Queue<float>();

        private void Start()
        {
            if (modelAsset == null)
            {
                Debug.LogWarning("BreathingModelController requires a Sentis model asset.");
                enabled = false;
                return;
            }

            runtimeModel = ModelLoader.Load(modelAsset);

            try
            {
                worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);
            }
            catch (InvalidOperationException)
            {
                worker = WorkerFactory.CreateWorker(BackendType.CPU, runtimeModel);
            }
            catch (ArgumentException)
            {
                worker = WorkerFactory.CreateWorker(BackendType.CPU, runtimeModel);
            }
        }

        private void OnDestroy()
        {
            if (worker != null)
            {
                worker.Dispose();
                worker = null;
            }

            if (runtimeModel != null)
            {
#if UNITY_SENTIS_2_0_OR_NEWER
                runtimeModel.Dispose();
#endif
                runtimeModel = null;
            }

            if (inputTensor != null)
            {
                inputTensor.Dispose();
                inputTensor = null;
            }
        }

        private void Update()
        {
            if (worker == null)
            {
                return;
            }

            // Placeholder: Acquire microphone samples.
            // In editor, you can inject prerecorded data or feed from Microphone API.
            if (!TryDequeueSamples(out var samples))
            {
                return;
            }

            PrepareInput(samples);
            worker.Execute(inputTensor);

            TensorFloat output = worker.PeekOutput() as TensorFloat;
            if (output == null || output.shape.length == 0)
            {
                return;
            }

            var value = Mathf.Clamp01(output[0]);
            smoothedValue = Mathf.Lerp(smoothedValue, value, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(smoothing, 0.001f)));
            onBreathMetric.Invoke(smoothedValue);
            output.Dispose();
        }

        private bool TryDequeueSamples(out float[] samples)
        {
            if (sampleBuffer.Count == 0)
            {
                samples = Array.Empty<float>();
                return false;
            }

            samples = sampleBuffer.ToArray();
            sampleBuffer.Clear();
            return true;
        }

        private void PrepareInput(IReadOnlyList<float> samples)
        {
            if (inputTensor != null)
            {
                inputTensor.Dispose();
            }

            var shape = new TensorShape(1, samples.Count);
            inputTensor = new TensorFloat(shape);

            for (var i = 0; i < samples.Count; i++)
            {
                inputTensor[i] = samples[i];
            }
        }

        /// <summary>
        /// Allows external audio capture components to feed processed sample data into the model controller.
        /// </summary>
        public void EnqueueSamples(IReadOnlyList<float> samples)
        {
            foreach (var sample in samples)
            {
                sampleBuffer.Enqueue(sample);
            }
        }
    }
}
