using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{

    Vector3 move_Dir = Vector2.zero;
    Vector2 rot_Dir = Vector2.zero;

    float mouseSensitivity = 100f;

    public InputManager() {
        Debug.Log("Init Input");
    }
    ~InputManager() { }

    public void UpdateInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        move_Dir = new Vector2(horizontal, vertical).normalized;

        float mX = Input.GetAxis("Mouse X") * Time.smoothDeltaTime * mouseSensitivity * 5f;
        float mY = Input.GetAxis("Mouse Y") * Time.smoothDeltaTime * mouseSensitivity * 5f;

        rot_Dir = new Vector3(mX, mY, 0);
    }

    public bool GetButtonDown(string buttonName) {  return Input.GetButtonDown(buttonName); }
    public Vector3 GetInputAxis() { return move_Dir; }
    public Vector2 GetRotationAxis() { return rot_Dir; }

    public bool GetMouseDown(int index) { return Input.GetMouseButtonDown(index);}
    public bool GetMouseHold(int index) { return Input.GetMouseButton(index); }
}
