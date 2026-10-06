using UnityEngine;

namespace Gateway.Visuals
{
    /// <summary>
    /// Lightweight fly-style camera rig for quickly exploring abstract spaces.
    /// Uses WASD/arrow keys for planar movement and right-mouse drag for orientation.
    /// </summary>
    public interface IInputProvider
    {
        bool GetMouseButton(int button);
        float GetAxis(string axisName);
        float GetAxisRaw(string axisName);
        bool GetKey(KeyCode key);
    }

    public interface ITimeProvider
    {
        float DeltaTime { get; }
    }

    public class DefaultInputProvider : IInputProvider
    {
        public bool GetMouseButton(int button) => Input.GetMouseButton(button);
        public float GetAxis(string axisName) => Input.GetAxis(axisName);
        public float GetAxisRaw(string axisName) => Input.GetAxisRaw(axisName);
        public bool GetKey(KeyCode key) => Input.GetKey(key);
    }

    public class DefaultTimeProvider : ITimeProvider
    {
        public float DeltaTime => Time.deltaTime;
    }

    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class SimpleFlyCamera : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Movement speed in units per second.")]
        private float moveSpeed = 5f;

        [SerializeField]
        [Tooltip("Mouse look sensitivity.")]
        private float lookSensitivity = 120f;

        [SerializeField]
        [Tooltip("Optional height offset applied when strafing.")]
        private float verticalSpeed = 3f;

        private float yaw;
        private float pitch;

        public IInputProvider InputProvider { get; set; } = new DefaultInputProvider();
        public ITimeProvider TimeProvider { get; set; } = new DefaultTimeProvider();

        private void Awake()
        {
            var rotation = transform.rotation.eulerAngles;
            yaw = rotation.y;
            pitch = rotation.x;
        }

        private void Update()
        {
            UpdateRotation();
            UpdateTranslation();
        }

        private void UpdateRotation()
        {
            if (!InputProvider.GetMouseButton(1))
            {
                return;
            }

            yaw += InputProvider.GetAxis("Mouse X") * lookSensitivity * TimeProvider.DeltaTime;
            pitch -= InputProvider.GetAxis("Mouse Y") * lookSensitivity * TimeProvider.DeltaTime;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        private void UpdateTranslation()
        {
            var input = new Vector3(InputProvider.GetAxisRaw("Horizontal"), 0f, InputProvider.GetAxisRaw("Vertical"));
            if (InputProvider.GetKey(KeyCode.E))
            {
                input.y += 1f;
            }

            if (InputProvider.GetKey(KeyCode.Q))
            {
                input.y -= 1f;
            }

            var direction = transform.TransformDirection(input.normalized);
            transform.position += direction * moveSpeed * TimeProvider.DeltaTime;
            transform.position += Vector3.up * input.y * verticalSpeed * TimeProvider.DeltaTime;
        }
    }
}
