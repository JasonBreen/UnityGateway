using NUnit.Framework;
using UnityEngine;
using Gateway.Visuals;
using System.Collections.Generic;

namespace Gateway.Visuals.Tests
{
    public class GatewayVisualControllerTests
    {
        private GameObject go;
        private GatewayVisualController controller;
        private Material material1;
        private Material material2;

        [SetUp]
        public void Setup()
        {
            go = new GameObject();
            controller = go.AddComponent<GatewayVisualController>();

            // Standard shader will have standard properties we can test
            material1 = new Material(Shader.Find("Standard"));
            material2 = new Material(Shader.Find("Standard"));
        }

        [TearDown]
        public void Teardown()
        {
            if (go != null) Object.DestroyImmediate(go);
            if (material1 != null) Object.DestroyImmediate(material1);
            if (material2 != null) Object.DestroyImmediate(material2);
        }

        [Test]
        public void ApplyState_SetsMaterialParametersCorrectly()
        {
            // Set up target materials list via reflection
            var targetMaterialsField = typeof(GatewayVisualController).GetField("targetMaterials", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            targetMaterialsField.SetValue(controller, new List<Material> { material1, material2 });

            // Arrange
            var state = ScriptableObject.CreateInstance<GatewayVisualState>();

            // Set up material parameters list via reflection
            var paramField = typeof(GatewayVisualState).GetField("materialParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var parameters = new List<MaterialParameter>();

            var param1 = new MaterialParameter();
            var nameField = typeof(MaterialParameter).GetField("propertyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var valueField = typeof(MaterialParameter).GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            nameField.SetValue(param1, "_Metallic");
            valueField.SetValue(param1, 0.5f);
            parameters.Add(param1);

            var param2 = new MaterialParameter();
            nameField.SetValue(param2, "_Glossiness");
            valueField.SetValue(param2, 0.75f);
            parameters.Add(param2);

            paramField.SetValue(state, parameters);

            // Act
            controller.ApplyState(state);

            // Assert
            Assert.AreEqual(0.5f, material1.GetFloat("_Metallic"));
            Assert.AreEqual(0.75f, material1.GetFloat("_Glossiness"));
            Assert.AreEqual(0.5f, material2.GetFloat("_Metallic"));
            Assert.AreEqual(0.75f, material2.GetFloat("_Glossiness"));

            // Cleanup
            Object.DestroyImmediate(state);
        }

        [Test]
        public void ApplyState_WithNullMaterial_DoesNotThrow()
        {
            // Set up target materials list via reflection with a null element
            var targetMaterialsField = typeof(GatewayVisualController).GetField("targetMaterials", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            targetMaterialsField.SetValue(controller, new List<Material> { material1, null, material2 });

            var state = ScriptableObject.CreateInstance<GatewayVisualState>();

            // Set up empty material parameters list via reflection
            var paramField = typeof(GatewayVisualState).GetField("materialParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            paramField.SetValue(state, new List<MaterialParameter>());

            // This will throw if the code doesn't properly handle null materials (which it does using `if (material == null) continue;`)
            Assert.DoesNotThrow(() => controller.ApplyState(state));

            Object.DestroyImmediate(state);
        }

        [Test]
        public void ApplyState_WithNullState_ThrowsNullReferenceException()
        {
            // The method doesn't do a null check on the state itself, it just attempts to read from it in a foreach loop.
            // As such it will throw a NullReferenceException on the line `foreach (var parameter in state.MaterialParameters)`
            Assert.Throws<System.NullReferenceException>(() => controller.ApplyState(null));
        }

        [Test]
        public void ApplyState_SetsActiveStateField()
        {
            // Arrange
            var state = ScriptableObject.CreateInstance<GatewayVisualState>();

            // Set up empty material parameters list via reflection
            var paramField = typeof(GatewayVisualState).GetField("materialParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            paramField.SetValue(state, new List<MaterialParameter>());

            // Act
            controller.ApplyState(state);

            // Assert
            var activeStateField = typeof(GatewayVisualController).GetField("activeState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var activeState = activeStateField.GetValue(controller);
            Assert.AreSame(state, activeState);

            // Cleanup
            Object.DestroyImmediate(state);
        }

        [Test]
        public void ApplyState_IgnoresMissingMaterialProperties()
        {
            var targetMaterialsField = typeof(GatewayVisualController).GetField("targetMaterials", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            targetMaterialsField.SetValue(controller, new List<Material> { material1 });

            // Arrange
            var state = ScriptableObject.CreateInstance<GatewayVisualState>();

            var paramField = typeof(GatewayVisualState).GetField("materialParameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var parameters = new List<MaterialParameter>();

            var param1 = new MaterialParameter();
            var nameField = typeof(MaterialParameter).GetField("propertyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var valueField = typeof(MaterialParameter).GetField("value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            nameField.SetValue(param1, "_NonExistentProperty");
            valueField.SetValue(param1, 0.5f);
            parameters.Add(param1);

            paramField.SetValue(state, parameters);

            // Act & Assert - Should not throw an exception when setting a non-existent property in Unity
            Assert.DoesNotThrow(() => controller.ApplyState(state));

            // Cleanup
            Object.DestroyImmediate(state);
        }
    }
}
