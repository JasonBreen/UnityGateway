using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Gateway.Visuals;
using System.Reflection;

namespace Gateway.Tests
{
    public class GatewayStateMachineTests
    {
        private GameObject go;
        private GatewayStateMachine stateMachine;
        private AudioSource audioSource;
        private GatewayVisualController visualController;
        private GatewaySessionTimeline timeline;

        [SetUp]
        public void Setup()
        {
            go = new GameObject("GatewayStateMachineTest");

            audioSource = go.AddComponent<AudioSource>();
            audioSource.clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);

            visualController = go.AddComponent<GatewayVisualController>();

            timeline = ScriptableObject.CreateInstance<GatewaySessionTimeline>();

            stateMachine = go.AddComponent<GatewayStateMachine>();

            var timelineField = typeof(GatewayStateMachine).GetField("sessionTimeline", BindingFlags.NonPublic | BindingFlags.Instance);
            if (timelineField != null)
                timelineField.SetValue(stateMachine, timeline);

            var visualControllerField = typeof(GatewayStateMachine).GetField("visualController", BindingFlags.NonPublic | BindingFlags.Instance);
            if (visualControllerField != null)
                visualControllerField.SetValue(stateMachine, visualController);

            var autoStartField = typeof(GatewayStateMachine).GetField("autoStart", BindingFlags.NonPublic | BindingFlags.Instance);
            if (autoStartField != null)
                autoStartField.SetValue(stateMachine, false);
        }

        [TearDown]
        public void Teardown()
        {
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
            if (timeline != null)
            {
                Object.DestroyImmediate(timeline);
            }
        }

        [UnityTest]
        public IEnumerator StartSession_WithValidSetup_PlaysAudioSource()
        {
            stateMachine.StartSession();
            yield return null;

            Assert.IsTrue(audioSource.isPlaying, "AudioSource should be playing after StartSession.");
        }

        [UnityTest]
        public IEnumerator StartSession_NullTimeline_DoesNotPlayAudio()
        {
            var timelineField = typeof(GatewayStateMachine).GetField("sessionTimeline", BindingFlags.NonPublic | BindingFlags.Instance);
            if (timelineField != null)
                timelineField.SetValue(stateMachine, null);

            stateMachine.StartSession();
            yield return null;

            Assert.IsFalse(audioSource.isPlaying, "AudioSource should not play if timeline is missing.");
        }

        [UnityTest]
        public IEnumerator StartSession_NullVisualController_DoesNotPlayAudio()
        {
            var visualControllerField = typeof(GatewayStateMachine).GetField("visualController", BindingFlags.NonPublic | BindingFlags.Instance);
            if (visualControllerField != null)
                visualControllerField.SetValue(stateMachine, null);

            stateMachine.StartSession();
            yield return null;

            Assert.IsFalse(audioSource.isPlaying, "AudioSource should not play if visualController is missing.");
        }

        [UnityTest]
        public IEnumerator StopSession_StopsAudioSource()
        {
            stateMachine.StartSession();
            yield return null;
            Assert.IsTrue(audioSource.isPlaying);

            stateMachine.StopSession();
            yield return null;
            Assert.IsFalse(audioSource.isPlaying, "AudioSource should be stopped after StopSession.");
        }
    }
}
