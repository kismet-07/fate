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
            if (movement == null || animator == null || animator.runtimeAnimatorController == null)
                return;

            animator.SetFloat(speedParameter, movement.CurrentHorizontalSpeed);
            animator.SetBool(movingParameter, movement.CurrentHorizontalSpeed > 0.05f);
            animator.SetBool(sprintParameter, movement.IsRunning);
        }
    }
}
