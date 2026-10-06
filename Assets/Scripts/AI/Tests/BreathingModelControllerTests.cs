using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Gateway.AI;

namespace Gateway.AI.Tests
{
    public class BreathingModelControllerTests
    {
        private GameObject go;
        private BreathingModelController controller;
        private FieldInfo sampleBufferField;
        private FieldInfo sampleWindowField;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("BreathingModelControllerTest");
            // BreathingModelController requires an AudioSource according to its attributes
            go.AddComponent<AudioSource>();
            controller = go.AddComponent<BreathingModelController>();

            // Get reflection info for private fields to verify internal state
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            sampleBufferField = typeof(BreathingModelController).GetField("sampleBuffer", flags);
            sampleWindowField = typeof(BreathingModelController).GetField("sampleWindow", flags);

            // Set sample window to a smaller testable size for easier buffer overflow tests, if needed
            // Default is 1024 (minimum 32 clamped in Awake)
            // But we can just use the initialized value (32 because Awake clamped 1024 -> wait, Awake clamps if < 32)
            // Let's explicitly set it via reflection so we know its exact size
            sampleWindowField.SetValue(controller, 32);
        }

        [TearDown]
        public void TearDown()
        {
            if (go != null)
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        private Queue<float> GetSampleBuffer()
        {
            return (Queue<float>)sampleBufferField.GetValue(controller);
        }

        [Test]
        public void EnqueueSamples_AddsSamplesToBuffer()
        {
            var samples = new float[] { 0.1f, 0.2f, 0.3f };

            controller.EnqueueSamples(samples);

            var buffer = GetSampleBuffer();
            Assert.AreEqual(3, buffer.Count);

            // Convert to array to check elements
            var bufferArray = buffer.ToArray();
            Assert.AreEqual(0.1f, bufferArray[0]);
            Assert.AreEqual(0.2f, bufferArray[1]);
            Assert.AreEqual(0.3f, bufferArray[2]);
        }

        [Test]
        public void EnqueueSamples_EmptyList_DoesNotModifyBuffer()
        {
            var samples = new float[0];

            controller.EnqueueSamples(samples);

            var buffer = GetSampleBuffer();
            Assert.AreEqual(0, buffer.Count);
        }

        [Test]
        public void EnqueueSamples_RespectsMaximumBufferSize()
        {
            // Max buffer size is sampleWindow * 4
            int window = (int)sampleWindowField.GetValue(controller);
            int maxBufferSize = window * 4;

            // Enqueue exactly the max buffer size
            var initialSamples = new float[maxBufferSize];
            for (int i = 0; i < maxBufferSize; i++)
            {
                initialSamples[i] = i;
            }
            controller.EnqueueSamples(initialSamples);

            var buffer = GetSampleBuffer();
            Assert.AreEqual(maxBufferSize, buffer.Count);

            // Enqueue one more sample, it should dequeue the oldest one
            controller.EnqueueSamples(new float[] { 999f });

            Assert.AreEqual(maxBufferSize, buffer.Count);
            var bufferArray = buffer.ToArray();

            // The first element should now be 1, not 0
            Assert.AreEqual(1f, bufferArray[0]);
            // The last element should be the new sample
            Assert.AreEqual(999f, bufferArray[maxBufferSize - 1]);
        }

        [Test]
        public void EnqueueSamples_MassiveInput_TrimsToMaximumBufferSize()
        {
            int window = (int)sampleWindowField.GetValue(controller);
            int maxBufferSize = window * 4;

            // Create samples way larger than buffer
            var massiveSamples = new float[maxBufferSize + 10];
            for(int i = 0; i < massiveSamples.Length; i++)
            {
                massiveSamples[i] = i;
            }

            controller.EnqueueSamples(massiveSamples);

            var buffer = GetSampleBuffer();
            Assert.AreEqual(maxBufferSize, buffer.Count);

            var bufferArray = buffer.ToArray();
            // Should contain the LAST maxBufferSize elements
            Assert.AreEqual(10f, bufferArray[0]);
            Assert.AreEqual(massiveSamples.Length - 1, bufferArray[maxBufferSize - 1]);
        }
    }
}
