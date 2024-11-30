using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    private float Rx = 0;

    public Transform Player;
    public Transform MagPos;
    public Camera fpsCamObj;
    public GameObject Crosshair;

    private InputManager inputMgr;

    private GameObject startPos;

    private bool IsAiming = false;

    private void OnEnable()
    {
        EventHandler.RegisterEvent<InputManager>(GameEvents.OnInputManagerUpdate,SetInputManager);
        EventHandler.RegisterEvent(GameEvents.OnMouseUpdate, OnUpdate);
    }

    private void OnDisable()
    {
        EventHandler.UnregisterEvent<InputManager>(GameEvents.OnInputManagerUpdate, SetInputManager);
        EventHandler.UnregisterEvent(GameEvents.OnMouseUpdate, OnUpdate);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        startPos = new GameObject("weaponStart");
        startPos.transform.parent = this.transform;
        startPos.transform.position = fpsCamObj.transform.position;
    }

    void OnUpdate()
    { 
        if (inputMgr != null)
        {

            Debug.Log(inputMgr.GetRotationAxis());

            float mX = inputMgr.GetRotationAxis().x;
            float mY = inputMgr.GetRotationAxis().y;

            Rx -= mY;

            Rx = Mathf.Clamp(Rx, -45f, 45f);


            Quaternion targetRot = Quaternion.Euler(Rx, 0, 0);
            transform.localRotation = targetRot;

            Player.Rotate(Vector3.up * mX);

            if (inputMgr.GetMouseDown(1))
                ToggleScope();

        }
    }

    void ToggleScope()
    {
        IsAiming = !IsAiming;
        if (IsAiming)
        {
            while (Vector3.Distance(fpsCamObj.transform.position, MagPos.position) > 0.0001f)
            {
                fpsCamObj.transform.position = Vector3.Lerp(fpsCamObj.transform.position, MagPos.position, Time.deltaTime * 5f);
                fpsCamObj.transform.rotation = MagPos.rotation;
                fpsCamObj.fieldOfView = 25f;
                Crosshair.SetActive(false);
            }
        }
        else
        {
            while (Vector3.Distance(fpsCamObj.transform.position, startPos.transform.position) > 0.0001f)
            {
                fpsCamObj.transform.position = Vector3.Lerp(fpsCamObj.transform.position, startPos.transform.position, Time.deltaTime * 5f);
                fpsCamObj.fieldOfView = 50f;
                Crosshair.SetActive(true);
            }
        }
    }

    void SetInputManager(InputManager inputMgr)
    {
        Debug.Log("Setting input manager");
        this.inputMgr = inputMgr;
    }
}
