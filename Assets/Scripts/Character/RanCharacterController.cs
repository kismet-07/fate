using UnityEngine;

namespace RanMobile.Character
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(RanCharacterInput))]
    public sealed class RanCharacterController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float walkSpeed = 2.2f;
        [SerializeField, Min(0f)] private float runSpeed = 4.8f;
        [SerializeField, Min(0f)] private float acceleration = 14f;
        [SerializeField, Min(0f)] private float deceleration = 18f;
        [SerializeField, Min(0f)] private float rotationSpeed = 720f;
        [SerializeField, Min(0f)] private float gravity = 25f;

        private CharacterController controller;
        private RanCharacterInput input;
        private Camera mainCamera;
        private float verticalVelocity;
        private float currentSpeed;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float CurrentHorizontalSpeed { get; private set; }
        public bool IsRunning { get; private set; }
        public Vector3 CurrentMoveDirection { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<RanCharacterInput>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector2 moveInput = input != null ? input.Move : Vector2.zero;
            Vector3 direction = CameraRelativeDirection(moveInput);
            CurrentMoveDirection = direction;

            IsRunning = input != null && input.Sprint && moveInput.sqrMagnitude > 0.01f;
            float targetSpeed = IsRunning ? runSpeed : walkSpeed;
            float targetHorizontalSpeed = direction.sqrMagnitude > 0.0001f ? targetSpeed : 0f;
            float speedChange = targetHorizontalSpeed > currentSpeed ? acceleration : deceleration;

            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetHorizontalSpeed,
                speedChange * Time.deltaTime);

            if (controller.isGrounded)
                verticalVelocity = -2f;
            else
                verticalVelocity -= gravity * Time.deltaTime;

            Vector3 velocity = direction * currentSpeed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

            CurrentHorizontalSpeed = new Vector3(
                controller.velocity.x,
                0f,
                controller.velocity.z).magnitude;

            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 CameraRelativeDirection(Vector2 move)
        {
            if (move.sqrMagnitude < 0.0001f)
                return Vector3.zero;

            Vector3 forward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;
            Vector3 right = mainCamera != null ? mainCamera.transform.right : Vector3.right;

            forward.y = 0f;
            right.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;
            else
                forward.Normalize();

            if (right.sqrMagnitude < 0.0001f)
                right = Vector3.right;
            else
                right.Normalize();

            return Vector3.ClampMagnitude(forward * move.y + right * move.x, 1f);
        }

        private void OnValidate()
        {
            walkSpeed = Mathf.Max(0f, walkSpeed);
            runSpeed = Mathf.Max(0f, runSpeed);
            acceleration = Mathf.Max(0f, acceleration);
            deceleration = Mathf.Max(0f, deceleration);
            rotationSpeed = Mathf.Max(0f, rotationSpeed);
            gravity = Mathf.Max(0f, gravity);
        }
    }
}
