using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[Serializable]
public struct PlayerLocomotion
{
    [Range(0, 10f), Tooltip("[0, 10]")] public float acceleration;
    [Range(0, 10f), Tooltip("[0, 10]")] public float walkSpeed;
    [Range(0, 10f), Tooltip("[0, 10]")] public float runSpeed;
    [Range(0, 10f), Tooltip("[0, 10]")] public float jumpHeight;
    [Range(-10f, 10f), Tooltip("[-10, 10]")] public float gravity;
}

[Serializable]
public struct PlayerLookSettings
{
    [Tooltip("Place the Camera Parent Object Here")] public Transform CamMainObj;
    [Tooltip("Place the Camera Component Here")] public Camera fpsCamObj;
    [Tooltip("Place the Aiming Position Here")] public Transform MagPos;
    [Tooltip("Adjust the Min Pitch")] public float minPitch;
    [Tooltip("Adjust the Max Pitch")] public float maxPitch;
    [Tooltip("Look Interpolation Smooth Time")] public float smoothTime;
}

public struct PlayerAnimators
{
    public Animator anim;
    public Animator tps_anim;
}


public class PlayerController : BaseNetworkBehaviour
{
    private Rigidbody rb;
    private CharacterController characterController;
    private Vector3 moveVelocity;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private float yaw;
    private float pitch;
    private float currentYaw;
    private float currentPitch;
    private float yawVelocity;
    private float pitchVelocity;
    private bool IsAiming = false;

    public Animator anim, tps_anim;

    [SerializeField]
    private PlayerLookSettings playerLookSettings = new PlayerLookSettings()
    {
        minPitch = -90f,
        maxPitch = 90,
        smoothTime = 0.1f
    };

    [SerializeField]
    private PlayerLocomotion playerLocomotion = new PlayerLocomotion()
    {
        acceleration = 10f,
        gravity = -9.8f,
        jumpHeight = 2f,
        walkSpeed = 5f,
        runSpeed = 10f
    };
    
    //temporary
    [SerializeField] private InputManager inputManager;

    [SerializeField] private PlayerHealth playerHealth;


    private GameObject startPos;

    private void Awake()
    {
        Application.targetFrameRate = 120;

        inputManager = new InputManager();
        playerHealth = new PlayerHealth();

        EventHandler.ExecuteEvent<InputManager>(GameEvents.OnInputManagerUpdate, inputManager);
    }

    protected override void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        startPos = new GameObject("weaponStart");
        startPos.transform.parent = playerLookSettings.CamMainObj.transform;
        startPos.transform.position = playerLookSettings.fpsCamObj.transform.position;

        //rb = GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        UpdateInput();
        MouseControl();
        Move();
        CheckShoot();
    }

    //private void rbMove()
    //{
    //    if (direction.magnitude >= 0.1f)
    //    {
    //        Vector3 dir = transform.TransformDirection(direction);
    //        Vector3 move = dir * moveSpeed * Time.fixedDeltaTime;
    //        rb.MovePosition(rb.position + move);
    //    }
    //    anim.SetFloat("Speed", direction.magnitude, 0.1f, 0.1f);
    //    tps_anim.SetFloat("SpeedX", direction.x, 0.1f, 0.1f);
    //    tps_anim.SetFloat("SpeedY", direction.z, 0.1f, 0.1f);
    //}

    private void UpdateInput()
    {
        inputManager.UpdateInput();
    }

    private void MouseControl()
    {
        //EventHandler.ExecuteEvent(GameEvents.OnMouseUpdate);
        if (inputManager != null)
        {

            Debug.Log(inputManager.GetRotationAxis());

            float mX = inputManager.GetRotationAxis().x;
            float mY = inputManager.GetRotationAxis().y;

            yaw += mX;
            pitch -= mY;

            pitch = Mathf.Clamp(pitch, playerLookSettings.minPitch, playerLookSettings.maxPitch);

            currentYaw = Mathf.SmoothDamp(currentYaw, yaw, ref yawVelocity, playerLookSettings.smoothTime);
            currentPitch = Mathf.SmoothDamp(currentPitch, pitch, ref pitchVelocity, playerLookSettings.smoothTime);

            transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
            playerLookSettings.CamMainObj.localRotation = Quaternion.Euler(-currentPitch, 0f, 0f);

            if (inputManager.GetMouseDown(1))
                ToggleScope();
        }
    }

    void ToggleScope()
    {
        IsAiming = !IsAiming;
        if (IsAiming)
        {
            while (Vector3.Distance(playerLookSettings.fpsCamObj.transform.position, playerLookSettings.MagPos.position) > 0.0001f)
            {
                playerLookSettings.fpsCamObj.transform.position = Vector3.Lerp(playerLookSettings.fpsCamObj.transform.position, playerLookSettings.MagPos.position, Time.smoothDeltaTime * 5f);
                playerLookSettings.fpsCamObj.transform.rotation = playerLookSettings.MagPos.rotation;
                playerLookSettings.fpsCamObj.fieldOfView = 25f;
                //Crosshair.SetActive(false);
            }
        }
        else
        {
            while (Vector3.Distance(playerLookSettings.fpsCamObj.transform.position, startPos.transform.position) > 0.0001f)
            {
                playerLookSettings.fpsCamObj.transform.position = Vector3.Lerp(playerLookSettings.fpsCamObj.transform.position, startPos.transform.position, Time.deltaTime * 5f);
                playerLookSettings.fpsCamObj.fieldOfView = 50f;
                //Crosshair.SetActive(true);
            }
        }
    }

    private void CheckShoot()
    {
        if (inputManager.GetMouseDown(0))
        {
            EventHandler.ExecuteEvent(GameEvents.OnPlayerFire);
            anim.SetBool("IsShooting", true);
        }
        else
        {
            anim.SetBool("IsShooting", false);
        }
    }

    private void Move()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }


        float horizontal = inputManager.GetInputAxis().x;
        float vertical = inputManager.GetInputAxis().y;

        var targetDirection = transform.right * horizontal + transform.forward * vertical;
        targetDirection *= playerLocomotion.walkSpeed;

        moveVelocity = Vector3.Lerp(moveVelocity, targetDirection, playerLocomotion.acceleration * Time.deltaTime);

        characterController.Move(moveVelocity * Time.deltaTime);

        playerVelocity.y += playerLocomotion.gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(playerLocomotion.jumpHeight * -2f * playerLocomotion.gravity);
        }

        anim.SetFloat("Speed", targetDirection.normalized.magnitude, 0.1f, 0.1f);
        tps_anim.SetFloat("SpeedX", targetDirection.normalized.x, 0.1f, 0.1f);
        tps_anim.SetFloat("SpeedY", targetDirection.normalized.z, 0.1f, 0.1f);

    }

    //private void MouseControl()
    //{

    //}
}
