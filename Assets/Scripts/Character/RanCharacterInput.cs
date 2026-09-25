using UnityEngine;
using UnityEngine.InputSystem;

namespace RanMobile.Character
{
    public sealed class RanCharacterInput : MonoBehaviour
    {
        public Vector2 Move { get; private set; }
        public bool Sprint { get; private set; }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                Move = Vector2.zero;
                Sprint = false;
                return;
            }

            float x = 0f;
            float y = 0f;

            if (keyboard.aKey.isPressed) x -= 1f;
            if (keyboard.dKey.isPressed) x += 1f;
            if (keyboard.sKey.isPressed) y -= 1f;
            if (keyboard.wKey.isPressed) y += 1f;

            Move = Vector2.ClampMagnitude(new Vector2(x, y), 1f);
            Sprint = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        }
    }
}
