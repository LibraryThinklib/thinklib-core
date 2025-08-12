using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Thinklib/Platformer/Movement/Input Handler", -100)]
public class InputHandler : MonoBehaviour
{
    public Vector2 GetKeyboardInput(List<KeyCode> rightKeys, List<KeyCode> leftKeys)
    {
        float x = 0f;

        foreach (KeyCode key in rightKeys)
        {
            if (Input.GetKey(key)) x += 1f;
        }

        foreach (KeyCode key in leftKeys)
        {
            if (Input.GetKey(key)) x -= 1f;
        }

        return new Vector2(x, 0f);
    }

    public Vector2 GetJoystickInput(Joystick joystick)
    {
        return new Vector2(joystick.Horizontal, joystick.Vertical);
    }
}
