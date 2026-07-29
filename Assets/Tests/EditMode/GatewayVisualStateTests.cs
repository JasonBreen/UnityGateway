using NUnit.Framework;
using UnityEngine;
using Gateway.Visuals;
using System.Collections.Generic;

namespace Gateway.Visuals.Tests
{
    public class GatewayVisualStateTests
    {
        [Test]
        public void Creation_SetsDefaultValues()
        {
            var state = ScriptableObject.CreateInstance<GatewayVisualState>();

            Assert.AreEqual("Focus State", state.DisplayName);
            Assert.IsNotNull(state.MaterialParameters);
            Assert.AreEqual(0, state.MaterialParameters.Count);
            Assert.IsNotNull(state.AnimationParameters);
            Assert.AreEqual(0, state.AnimationParameters.Count);

            Object.DestroyImmediate(state);
        }
    }
}
