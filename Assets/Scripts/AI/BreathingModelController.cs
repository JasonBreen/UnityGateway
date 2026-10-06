using System;
using System.Collections.Generic;
using Unity.InferenceEngine;
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
        [Tooltip("ONNX model asset referenced by Inference Engine (placed in a Resources folder or assigned in inspector).")]
        private ModelAsset modelAsset = null;

        [SerializeField]
        [Tooltip("Smoothing factor applied to the raw model output.")]
        private float smoothing = 0.1f;

        [SerializeField]
        [Tooltip("Event invoked whenever the normalized breath metric updates.")]
        private UnityEngine.Events.UnityEvent<float> onBreathMetric = new UnityEngine.Events.UnityEvent<float>();

        [SerializeField]
        [Min(32)]
        [Tooltip("Number of audio samples processed per model evaluation.")]
        private int sampleWindow = 1024;

        [SerializeField]
        [Tooltip("Automatically capture audio from the attached AudioSource for inference input.")]
        private bool autoCaptureAudioSource = true;

        [SerializeField]
        [Tooltip("Normalize the input window by its peak amplitude before inference.")]
        private bool normalizeInput = true;

        [SerializeField]
        [Tooltip("Use the Microphone API to acquire audio samples instead of the attached AudioSource.")]
        private bool useMicrophone = false;

        [SerializeField]
        [Tooltip("The name of the microphone device to use. Leave empty for the default device.")]
        private string microphoneDeviceName = null;

        [SerializeField]
        [Tooltip("Optional prerecorded AudioClip to use instead of the microphone when testing.")]
        private AudioClip prerecordedClip = null;

        private Model runtimeModel = null;
        private Worker worker = null;
        private Tensor<float> inputTensor = null;
        private float[] reusableWindow = null;
        private float smoothedValue;
        private readonly Queue<float> sampleBuffer = new Queue<float>();

        private AudioClip activeClip = null;
        private int lastSamplePosition = 0;
        private double simulatedAudioTime = 0.0;
        private float[] clipBuffer = null;

        public UnityEngine.Events.UnityEvent<float> OnBreathMetric => onBreathMetric;

        private void Awake()
        {
            if (sampleWindow < 32)
            {
                sampleWindow = 32;
            }
        }

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
                worker = new Worker(runtimeModel, BackendType.GPUCompute);
            }
            catch (InvalidOperationException)
            {
                worker = new Worker(runtimeModel, BackendType.CPU);
            }
            catch (ArgumentException)
            {
                worker = new Worker(runtimeModel, BackendType.CPU);
            }

            if (useMicrophone)
            {
                var device = string.IsNullOrEmpty(microphoneDeviceName) ? null : microphoneDeviceName;
                activeClip = Microphone.Start(device, true, 1, 44100);
                lastSamplePosition = 0;
            }
            else if (prerecordedClip != null)
            {
                activeClip = prerecordedClip;
                lastSamplePosition = 0;
                simulatedAudioTime = 0.0;
            }
        }

        private void OnDestroy()
        {
            if (useMicrophone)
            {
                var device = string.IsNullOrEmpty(microphoneDeviceName) ? null : microphoneDeviceName;
                Microphone.End(device);
            }

            if (worker != null)
            {
                worker.Dispose();
                worker = null;
            }

            runtimeModel = null;

            if (inputTensor != null)
            {
                inputTensor.Dispose();
                inputTensor = null;
            }
        }

        private void AcquireAudioSamples()
        {
            if (activeClip == null)
            {
                return;
            }

            int currentPosition = 0;
            if (useMicrophone)
            {
                var device = string.IsNullOrEmpty(microphoneDeviceName) ? null : microphoneDeviceName;
                currentPosition = Microphone.GetPosition(device);
            }
            else if (prerecordedClip != null)
            {
                simulatedAudioTime += Time.deltaTime;
                currentPosition = Mathf.FloorToInt((float)simulatedAudioTime * activeClip.frequency) % activeClip.samples;
            }

            if (currentPosition == lastSamplePosition)
            {
                return;
            }

            int totalSamples = activeClip.samples * activeClip.channels;
            if (clipBuffer == null || clipBuffer.Length != totalSamples)
            {
                clipBuffer = new float[totalSamples];
                if (!useMicrophone)
                {
                    activeClip.GetData(clipBuffer, 0);
                }
            }

            if (useMicrophone)
            {
                activeClip.GetData(clipBuffer, 0);
            }

            int numChannels = activeClip.channels;

            if (currentPosition < lastSamplePosition)
            {
                ProcessClipBuffer(lastSamplePosition, activeClip.samples, numChannels);
                ProcessClipBuffer(0, currentPosition, numChannels);
            }
            else
            {
                ProcessClipBuffer(lastSamplePosition, currentPosition, numChannels);
            }

            lastSamplePosition = currentPosition;
        }

        private void ProcessClipBuffer(int startSample, int endSample, int channels)
        {
            for (int i = startSample; i < endSample; i++)
            {
                float sum = 0f;
                for (int c = 0; c < channels; c++)
                {
                    sum += clipBuffer[i * channels + c];
                }
                var monoSample = sum / channels;
                sampleBuffer.Enqueue(monoSample);

                if (sampleBuffer.Count > sampleWindow * 4)
                {
                    sampleBuffer.Dequeue();
                }
            }
        }

        private void Update()
        {
            if (worker == null)
            {
                return;
            }

            AcquireAudioSamples();

            if (!TryDequeueSamples(out var samples))
            {
                return;
            }

            PrepareInput(samples);
            worker.Schedule(inputTensor);

            Tensor<float> output = worker.PeekOutput() as Tensor<float>;
            if (output == null || output.count == 0)
            {
                return;
            }

            using Tensor<float> readableOutput = output.ReadbackAndClone();
            var value = Mathf.Clamp01(readableOutput[0]);
            smoothedValue = Mathf.Lerp(smoothedValue, value, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(smoothing, 0.001f)));
            onBreathMetric.Invoke(smoothedValue);
        }

        private bool TryDequeueSamples(out float[] samples)
        {
            if (sampleBuffer.Count < sampleWindow)
            {
                samples = Array.Empty<float>();
                return false;
            }

            if (reusableWindow == null || reusableWindow.Length != sampleWindow)
            {
                reusableWindow = new float[sampleWindow];
            }

            for (var i = 0; i < sampleWindow; i++)
            {
                reusableWindow[i] = sampleBuffer.Dequeue();
            }

            samples = reusableWindow;
            return true;
        }

        private void PrepareInput(IReadOnlyList<float> samples)
        {
            if (inputTensor != null)
            {
                inputTensor.Dispose();
            }

            var shape = new TensorShape(1, samples.Count);
            inputTensor = new Tensor<float>(shape);

            float normalizationFactor = 1f;
            if (normalizeInput)
            {
                float maxMagnitude = 0f;
                for (var i = 0; i < samples.Count; i++)
                {
                    var magnitude = Mathf.Abs(samples[i]);
                    if (magnitude > maxMagnitude)
                    {
                        maxMagnitude = magnitude;
                    }
                }

                if (maxMagnitude > 0.0001f)
                {
                    normalizationFactor = 1f / maxMagnitude;
                }
            }

            for (var i = 0; i < samples.Count; i++)
            {
                inputTensor[i] = samples[i] * normalizationFactor;
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
                if (sampleBuffer.Count > sampleWindow * 4)
                {
                    sampleBuffer.Dequeue();
                }
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!autoCaptureAudioSource || data == null || channels <= 0)
            {
                return;
            }

            for (var index = 0; index < data.Length; index += channels)
            {
                float sum = 0f;
                for (var channel = 0; channel < channels; channel++)
                {
                    sum += data[index + channel];
                }

                var monoSample = sum / channels;
                sampleBuffer.Enqueue(monoSample);
                if (sampleBuffer.Count > sampleWindow * 4)
                {
                    sampleBuffer.Dequeue();
                }
            }
        }
    }
}
