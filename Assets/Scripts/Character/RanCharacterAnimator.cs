using UnityEngine;

namespace RanMobile.Character
{
    [RequireComponent(typeof(Animator))]
    public sealed class RanCharacterAnimator : MonoBehaviour
    {
        [SerializeField] private RanCharacterController movement;
        [SerializeField] private string speedParameter = "Speed";
        [SerializeField] private string movingParameter = "Moving";
        [SerializeField] private string sprintParameter = "Sprint";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (movement == null)
                movement = GetComponent<RanCharacterController>();
        }

        private void Update()
        {
            if (movement == null)
                return;

            float speed = 0f;
            if (movement.TryGetComponent<CharacterController>(out var controller))
                speed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;

            animator.SetFloat(speedParameter, speed);
            animator.SetBool(movingParameter, speed > 0.05f);
            animator.SetBool(sprintParameter, Input.GetKey(KeyCode.LeftShift));
        }
    }
}
