using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using Gateway.Visuals;
using System.Collections.Generic;

public class SimpleFlyCameraTests
{
    private GameObject cameraObj;
    private SimpleFlyCamera flyCamera;

    private class MockInputProvider : IInputProvider
    {
        public bool RightMouseButton = false;
        public Dictionary<string, float> Axes = new Dictionary<string, float>();
        public Dictionary<string, float> RawAxes = new Dictionary<string, float>();
        public HashSet<KeyCode> Keys = new HashSet<KeyCode>();

        public bool GetMouseButton(int button) => button == 1 && RightMouseButton;
        public float GetAxis(string axisName) => Axes.ContainsKey(axisName) ? Axes[axisName] : 0f;
        public float GetAxisRaw(string axisName) => RawAxes.ContainsKey(axisName) ? RawAxes[axisName] : 0f;
        public bool GetKey(KeyCode key) => Keys.Contains(key);
    }

    private class MockTimeProvider : ITimeProvider
    {
        public float DeltaTime { get; set; } = 0.1f;
    }

    private MockInputProvider mockInput;
    private MockTimeProvider mockTime;

    [SetUp]
    public void Setup()
    {
        cameraObj = new GameObject("TestCamera");
        cameraObj.AddComponent<Camera>();
        flyCamera = cameraObj.AddComponent<SimpleFlyCamera>();

        mockInput = new MockInputProvider();
        mockTime = new MockTimeProvider();

        flyCamera.InputProvider = mockInput;
        flyCamera.TimeProvider = mockTime;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(cameraObj);
    }

    [Test]
    public void RequireCameraComponent()
    {
        var requireComponent = (RequireComponent)System.Attribute.GetCustomAttribute(typeof(SimpleFlyCamera), typeof(RequireComponent));
        Assert.IsNotNull(requireComponent);
        Assert.AreEqual(typeof(Camera), requireComponent.m_Type0);
    }

    [Test]
    public void Initialization_GetsCorrectRotation()
    {
        cameraObj.transform.rotation = Quaternion.Euler(30f, 45f, 0f);

        var awakeMethod = typeof(SimpleFlyCamera).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod.Invoke(flyCamera, null);

        var pitchField = typeof(SimpleFlyCamera).GetField("pitch", BindingFlags.NonPublic | BindingFlags.Instance);
        var yawField = typeof(SimpleFlyCamera).GetField("yaw", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.AreEqual(30f, (float)pitchField.GetValue(flyCamera));
        Assert.AreEqual(45f, (float)yawField.GetValue(flyCamera));
    }

    [Test]
    public void UpdateRotation_WhenRightClickNotHeld_DoesNotChangeRotation()
    {
        cameraObj.transform.rotation = Quaternion.Euler(10f, 20f, 0f);
        mockInput.RightMouseButton = false;
        mockInput.Axes["Mouse X"] = 10f;
        mockInput.Axes["Mouse Y"] = 10f;

        var updateMethod = typeof(SimpleFlyCamera).GetMethod("UpdateRotation", BindingFlags.NonPublic | BindingFlags.Instance);
        updateMethod.Invoke(flyCamera, null);

        Assert.AreEqual(Quaternion.Euler(10f, 20f, 0f), cameraObj.transform.rotation);
    }

    [Test]
    public void UpdateRotation_WhenRightClickHeld_UpdatesRotation()
    {
        cameraObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        mockInput.RightMouseButton = true;
        mockInput.Axes["Mouse X"] = 1f; // yaw
        mockInput.Axes["Mouse Y"] = -1f; // pitch

        var awakeMethod = typeof(SimpleFlyCamera).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod.Invoke(flyCamera, null);

        var updateMethod = typeof(SimpleFlyCamera).GetMethod("UpdateRotation", BindingFlags.NonPublic | BindingFlags.Instance);
        updateMethod.Invoke(flyCamera, null);

        // lookSensitivity = 120, DeltaTime = 0.1
        // yaw = 1 * 120 * 0.1 = 12
        // pitch = -(-1 * 120 * 0.1) = 12
        Assert.AreEqual(Quaternion.Euler(12f, 12f, 0f).eulerAngles.x, cameraObj.transform.rotation.eulerAngles.x, 0.01f);
        Assert.AreEqual(Quaternion.Euler(12f, 12f, 0f).eulerAngles.y, cameraObj.transform.rotation.eulerAngles.y, 0.01f);
    }

    [Test]
    public void UpdateTranslation_MovesCameraCorrectly()
    {
        cameraObj.transform.position = Vector3.zero;
        cameraObj.transform.rotation = Quaternion.identity;

        mockInput.RawAxes["Horizontal"] = 1f;
        mockInput.RawAxes["Vertical"] = 1f;
        mockInput.Keys.Add(KeyCode.E);

        var updateMethod = typeof(SimpleFlyCamera).GetMethod("UpdateTranslation", BindingFlags.NonPublic | BindingFlags.Instance);
        updateMethod.Invoke(flyCamera, null);

        // input = (1, 1, 1)
        // input normalized = (0.577, 0.577, 0.577)
        // moveSpeed = 5, verticalSpeed = 3, deltaTime = 0.1
        // move: direction * 5 * 0.1 = direction * 0.5 -> (0.2885, 0.2885, 0.2885)
        // vertical move: up * input.y * 3 * 0.1 = up * 1 * 0.3 = (0, 0.3, 0)
        // total position = (0.2885, 0.5885, 0.2885)

        Assert.AreEqual(0.2885f, cameraObj.transform.position.x, 0.01f);
        Assert.AreEqual(0.5885f, cameraObj.transform.position.y, 0.01f);
        Assert.AreEqual(0.2885f, cameraObj.transform.position.z, 0.01f);
    }
}
