using UnityEngine;
using UnityEngine.InputSystem;

namespace RanMobile.CameraSystem
{
    public sealed class RanThirdPersonCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.45f, 0f);

        [Header("Position")]
        [SerializeField, Min(0.5f)] private float distance = 4.5f;
        [SerializeField, Range(-20f, 20f)] private float initialPitch = 8f;
        [SerializeField, Range(-89f, 89f)] private float minPitch = -30f;
        [SerializeField, Range(-89f, 89f)] private float maxPitch = 60f;

        [Header("Mouse Look")]
        [SerializeField, Min(0f)] private float lookSensitivity = 0.12f;
        [SerializeField] private bool requireRightMouseButton = true;

        [Header("Smoothing")]
        [SerializeField, Min(0f)] private float positionSmoothTime = 0.08f;
        [SerializeField, Min(0f)] private float rotationSmoothTime = 0.05f;

        private float yaw;
        private float pitch;
        private Vector3 positionVelocity;
        private float currentYaw;
        private float currentPitch;
        private bool initialized;

        private void Awake()
        {
            currentYaw = transform.eulerAngles.y;
            yaw = currentYaw;
            pitch = NormalizePitch(transform.eulerAngles.x);

            if (Mathf.Abs(pitch) < 0.01f)
                pitch = initialPitch;

            currentPitch = pitch;
            initialized = true;
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            UpdateLook();

            currentYaw = Mathf.SmoothDampAngle(
                currentYaw,
                yaw,
                ref positionVelocity.x,
                rotationSmoothTime);

            currentPitch = Mathf.SmoothDamp(
                currentPitch,
                pitch,
                ref positionVelocity.y,
                rotationSmoothTime);

            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 focusPoint = target.position + targetOffset;
            Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * distance;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref positionVelocity,
                positionSmoothTime);

            transform.rotation = rotation;
        }

        private void UpdateLook()
        {
            if (!initialized)
                return;

            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            if (requireRightMouseButton && !mouse.rightButton.isPressed)
                return;

            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * lookSensitivity;
            pitch -= delta.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        private static float NormalizePitch(float angle)
        {
            if (angle > 180f)
                angle -= 360f;

            return angle;
        }

        private void OnValidate()
        {
            maxPitch = Mathf.Max(minPitch, maxPitch);
            distance = Mathf.Max(0.5f, distance);
            positionSmoothTime = Mathf.Max(0f, positionSmoothTime);
            rotationSmoothTime = Mathf.Max(0f, rotationSmoothTime);
        }
    }
}
