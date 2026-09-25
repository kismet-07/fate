using UnityEngine;

namespace RanMobile.Character
{
    [DisallowMultipleComponent]
    public sealed class RanCharacterBootstrap : MonoBehaviour
    {
        [Header("Character Controller")]
        [SerializeField] private CharacterController characterController;

        [Header("Animator")]
        [SerializeField] private Animator animator;

        [Header("Collider Defaults")]
        [SerializeField, Min(0.01f)] private float radius = 0.28f;
        [SerializeField, Min(0.1f)] private float height = 1.8f;
        [SerializeField, Min(0f)] private float stepOffset = 0.25f;
        [SerializeField, Min(0.001f)] private float skinWidth = 0.03f;
        [SerializeField] private Vector3 center = new Vector3(0f, 0.9f, 0f);

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            EnsureComponents();
            ConfigureCharacterController();
        }

        private void EnsureComponents()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();

            if (characterController == null)
                characterController = gameObject.AddComponent<CharacterController>();

            if (GetComponent<RanCharacterInput>() == null)
                gameObject.AddComponent<RanCharacterInput>();

            if (GetComponent<RanCharacterController>() == null)
                gameObject.AddComponent<RanCharacterController>();

            if (animator == null)
                animator = GetComponent<Animator>();

            if (animator == null)
                animator = gameObject.AddComponent<Animator>();

            if (GetComponent<RanCharacterAnimator>() == null)
                gameObject.AddComponent<RanCharacterAnimator>();
        }

        private void ConfigureCharacterController()
        {
            characterController.center = center;
            characterController.radius = radius;
            characterController.height = height;
            characterController.stepOffset = Mathf.Min(stepOffset, height);
            characterController.skinWidth = skinWidth;
        }

        private void OnValidate()
        {
            radius = Mathf.Max(0.01f, radius);
            height = Mathf.Max(radius * 2f, height);
            stepOffset = Mathf.Clamp(stepOffset, 0f, height);
            skinWidth = Mathf.Max(0.001f, skinWidth);
        }
    }
}
