using UnityEngine;

namespace RanMobile.Character
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class RanCharacterController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float walkSpeed = 2.2f;
        [SerializeField, Min(0f)] private float runSpeed = 4.8f;
        [SerializeField, Min(0f)] private float rotationSpeed = 720f;
        [SerializeField] private bool keyboardSprint = true;

        private CharacterController controller;
        private Camera mainCamera;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input = Vector2.ClampMagnitude(input, 1f);

            Vector3 forward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;
            Vector3 right = mainCamera != null ? mainCamera.transform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = (forward * input.y + right * input.x);
            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            bool sprint = keyboardSprint && Input.GetKey(KeyCode.LeftShift);
            float speed = sprint ? runSpeed : walkSpeed;

            Vector3 horizontal = direction * speed;
            controller.Move(horizontal * Time.deltaTime);

            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    target,
                    rotationSpeed * Time.deltaTime);
            }
        }
    }
}
