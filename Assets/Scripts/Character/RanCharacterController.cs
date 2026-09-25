using UnityEngine;
using UnityEngine.InputSystem;

namespace RanMobile.Character
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(RanCharacterInput))]
    public sealed class RanCharacterController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float walkSpeed = 2.2f;
        [SerializeField, Min(0f)] private float runSpeed = 4.8f;
        [SerializeField, Min(0f)] private float rotationSpeed = 720f;
        [SerializeField, Min(0f)] private float gravity = 25f;

        private CharacterController controller;
        private RanCharacterInput input;
        private Camera mainCamera;
        private float verticalVelocity;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float CurrentHorizontalSpeed { get; private set; }
        public bool IsRunning { get; private set; }

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

            Vector2 moveInput = input.Move;
            Vector3 direction = CameraRelativeDirection(moveInput);

            IsRunning = input.Sprint && moveInput.sqrMagnitude > 0.01f;
            float speed = IsRunning ? runSpeed : walkSpeed;

            if (controller.isGrounded)
                verticalVelocity = -2f;
            else
                verticalVelocity -= gravity * Time.deltaTime;

            Vector3 velocity = direction * speed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

            CurrentHorizontalSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;

            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
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
            forward.Normalize();
            right.Normalize();

            return Vector3.ClampMagnitude(forward * move.y + right * move.x, 1f);
        }
    }
}
