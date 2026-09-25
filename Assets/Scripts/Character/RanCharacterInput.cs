using UnityEngine;

namespace RanMobile.Character
{
    public sealed class RanCharacterInput : MonoBehaviour
    {
        public Vector2 Move { get; private set; }
        public bool Sprint { get; private set; }

        private void Update()
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Move = Vector2.ClampMagnitude(Move, 1f);
            Sprint = Input.GetKey(KeyCode.LeftShift);
        }
    }
}
