using UnityEngine;

namespace RanMobile.Character
{
    public sealed class RanCharacterBootstrap : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Awake()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();

            if (characterController == null)
                characterController = gameObject.AddComponent<CharacterController>();

            characterController.center = new Vector3(0f, 0.9f, 0f);
            characterController.radius = 0.28f;
            characterController.height = 1.8f;
            characterController.stepOffset = 0.25f;
            characterController.skinWidth = 0.03f;
        }
    }
}
