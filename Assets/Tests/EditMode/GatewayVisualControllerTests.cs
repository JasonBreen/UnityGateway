using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Gateway.Visuals;

namespace Gateway.Visuals.Tests
{
    public class GatewayVisualControllerTests
    {
        private GameObject go;
        private GatewayVisualController controller;
        private Material testMaterial;
        private GatewayVisualState testState;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("TestController");
            controller = go.AddComponent<GatewayVisualController>();

            testMaterial = new Material(Shader.Find("Standard"));
            testMaterial.SetFloat("_Glossiness", 0.5f);

            var targetMaterialsField = typeof(GatewayVisualController).GetField("targetMaterials", BindingFlags.NonPublic | BindingFlags.Instance);
            targetMaterialsField.SetValue(controller, new List<Material> { testMaterial });

            testState = ScriptableObject.CreateInstance<GatewayVisualState>();

            var animParam = new AnimationParameter();
            var animParamType = typeof(AnimationParameter);
            animParamType.GetField("key", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(animParam, "Pulse");
            animParamType.GetField("curve", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(animParam, AnimationCurve.Linear(0f, 0f, 1f, 1f));

            var animParamsField = typeof(GatewayVisualState).GetField("animationParameters", BindingFlags.NonPublic | BindingFlags.Instance);
            animParamsField.SetValue(testState, new List<AnimationParameter> { animParam });

            var binding = new AnimationBinding();
            var bindingType = typeof(AnimationBinding);
            bindingType.GetField("parameterKey", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(binding, "Pulse");
            bindingType.GetField("propertyName", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(binding, "_Glossiness");

            var bindingsField = typeof(GatewayVisualController).GetField("animationBindings", BindingFlags.NonPublic | BindingFlags.Instance);
            bindingsField.SetValue(controller, new List<AnimationBinding> { binding });

            var awakeMethod = typeof(GatewayVisualController).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
            awakeMethod.Invoke(controller, null);
        }

        [TearDown]
        public void TearDown()
        {
            if (go != null) Object.DestroyImmediate(go);
            if (testMaterial != null) Object.DestroyImmediate(testMaterial);
            if (testState != null) Object.DestroyImmediate(testState);
        }

        [Test]
        public void Tick_NoActiveState_DoesNothing()
        {
            testMaterial.SetFloat("_Glossiness", 0.1f);

            controller.Tick(0.5f);

            Assert.AreEqual(0.1f, testMaterial.GetFloat("_Glossiness"));
        }

        [Test]
        public void Tick_WithActiveState_UpdatesMaterialParameters()
        {
            testMaterial.SetFloat("_Glossiness", 0.1f);

            controller.ApplyState(testState);

            controller.Tick(0.5f);

            Assert.AreEqual(0.5f, testMaterial.GetFloat("_Glossiness"));
        }

        [Test]
        public void Tick_ClampsNormalizedProgress()
        {
            testMaterial.SetFloat("_Glossiness", 0.1f);

            controller.ApplyState(testState);

            controller.Tick(1.5f);
            Assert.AreEqual(1.0f, testMaterial.GetFloat("_Glossiness"));

            controller.Tick(-0.5f);
            Assert.AreEqual(0.0f, testMaterial.GetFloat("_Glossiness"));
        }

        [Test]
        public void Tick_WithNullMaterial_DoesNotThrow()
        {
            var targetMaterialsField = typeof(GatewayVisualController).GetField("targetMaterials", BindingFlags.NonPublic | BindingFlags.Instance);
            var currentMaterials = (List<Material>)targetMaterialsField.GetValue(controller);
            currentMaterials.Add(null);

            controller.ApplyState(testState);

            Assert.DoesNotThrow(() => controller.Tick(0.5f));
        }
    }
}
