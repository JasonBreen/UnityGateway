using UnityEngine;

namespace Gateway.Visuals
{
    /// <summary>
    /// Lightweight fly-style camera rig for quickly exploring abstract spaces.
    /// Uses WASD/arrow keys for planar movement and right-mouse drag for orientation.
    /// </summary>
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
            if (!Input.GetMouseButton(1))
            {
                return;
            }

            yaw += Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        private void UpdateTranslation()
        {
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            if (Input.GetKey(KeyCode.E))
            {
                input.y += 1f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                input.y -= 1f;
            }

            var direction = transform.TransformDirection(input.normalized);
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.position += Vector3.up * input.y * verticalSpeed * Time.deltaTime;
        }
    }
}
