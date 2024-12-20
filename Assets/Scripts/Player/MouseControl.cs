using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public float minPitch = -90f;
    public float maxPitch = 90f;

    public float smoothTime = 0.1f;

    private Transform cameraTransform;
    private float yaw;
    private float pitch;

    private float currentYaw;
    private float currentPitch;
    private float yawVelocity;
    private float pitchVelocity;

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

            yaw += mX;
            pitch -= mY;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            currentYaw = Mathf.SmoothDamp(currentYaw, yaw, ref yawVelocity, smoothTime);
            currentPitch = Mathf.SmoothDamp(currentPitch, pitch, ref pitchVelocity, smoothTime);

            Player.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
            transform.localRotation = Quaternion.Euler(-currentPitch, 0f, 0f);

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
                fpsCamObj.transform.position = Vector3.Lerp(fpsCamObj.transform.position, MagPos.position, Time.smoothDeltaTime * 5f);
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
