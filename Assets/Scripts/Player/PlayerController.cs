using System;
using UnityEngine;

[Serializable]
public struct PlayerLocomotion
{
    [Range(0, 10f), Tooltip("[0, 10]")] public float acceleration;
    [Range(0, 10f), Tooltip("[0, 10]")] public float walkSpeed;
    [Range(0, 10f), Tooltip("[0, 10]")] public float runSpeed;
    [Range(0, 10f), Tooltip("[0, 10]")] public float jumpHeight;
    [Range(-10f, 10f), Tooltip("[-10, 10]")] public float gravity;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float walkStepInterval;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float runStepInterval;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float moveDampTime;
    [Range(0f, 10f), Tooltip("[0, 10]")] public float moveMultiplierForDamp;
}

[Serializable]
public struct PlayerLookSettings
{
    [Tooltip("REMEMBER TO REMOVE THIS")] public Transform CamMainObj;
    [Tooltip("REMEMBER TO REMOVE THIS")] public Camera fpsCamObj;
    [Tooltip("Place the Aiming Position Here")] public Transform MagPos;
    [Tooltip("Adjust the Min Pitch")] public float minPitch;
    [Tooltip("Adjust the Max Pitch")] public float maxPitch;
    [Tooltip("Look Interpolation Smooth Time")] public float smoothTime;

    public Transform HeadBone;
    public Transform CameraTranform;
    public Vector3 CameraOffset;
}

public struct PlayerAnimators
{
    public Animator anim;
    public Animator tps_anim;
}

[Serializable]
public struct PlayerIK
{
    [Tooltip("Place the Right Hand Reference")] public Transform RightHandRef;
    [Tooltip("Place the Right Elbow Reference")] public Transform RightElbowRef;
    [Tooltip("Place the Left Hand Reference")] public Transform LeftHandRef;
    [Tooltip("Place the Left Elbow Reference")] public Transform LeftElbowRef;

    [Tooltip("Place the ShoulderIK transform")] public Transform ShoulderIK;
    [Tooltip("Place the Right Shoulder Bone")] public Transform ShoulderBone;
    [Tooltip("Place the Right Hand Bone")] public Transform RightHand;

    [Tooltip("Place the camera look at transform")] public Transform LookAtPosition;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float RighthandWeight;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float LefthandWeight;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float BodyWeight;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float HeadWeight;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float EyeWeight;
    [Range(0f, 1f), Tooltip("[0, 1]")] public float MainWeight;
}

[Serializable]
public struct PlayerCameraSettings
{
    public Transform HeadBone;
    public Transform CameraTranform;
    public Vector3 CameraOffset;

}


public class PlayerController : MonoBehaviour
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

    public Transform RaycastPoint;
    public Animator anim, tps_anim;
    public PlayerTPSController tpsController;
    public bool IsPlayerActive = true;

    public GameObject BoneCombinerFPSRef,BoneCombinerTPSRef;

    private AudioSource footstepSource;
    private BoneCombiner boneCombiner;

    //public EquippableItem inventoryItem;

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
        runSpeed = 10f,
        walkStepInterval = 0.5f,
        runStepInterval = 0.3f
    };
    public AudioClip[] footstepClips;
    //temporary
    [SerializeField] private InputManager inputManager;

    [SerializeField] private PlayerHealth playerHealth;


    private GameObject startPos;
    private float stepTimer = 0f;
    private Vector3 targetDirection;

    private void OnEnable()
    {
        Init();
        EventHandler.RegisterEvent(gameObject,GameEvents.OnPlayerDied, PlayerDied);
    }

    private void OnDisable()
    {
        EventHandler.UnregisterEvent(gameObject,GameEvents.OnPlayerDied, PlayerDied);
    }

    private void Init()
    {
        Application.targetFrameRate = 120;

        boneCombiner = new BoneCombiner(BoneCombinerFPSRef, 62);
        
        inputManager = new InputManager();
        playerHealth = new PlayerHealth(gameObject);

        EventHandler.ExecuteEvent<InputManager>(GameEvents.OnInputManagerUpdate, inputManager);
    }

    protected void Start()
    {
        characterController = GetComponent<CharacterController>();
        footstepSource = GetComponent<AudioSource>();

        //Cursor.lockState = CursorLockMode.Locked;
        startPos = new GameObject("weaponStart");
        startPos.transform.parent = playerLookSettings.CamMainObj.transform;
        startPos.transform.position = playerLookSettings.fpsCamObj.transform.position;

        //OnInitInventory();
        //boneCombiner.AddLimb(inventoryItem.DisplayObject, inventoryItem.boneNames);
        //rb = GetComponent<Rigidbody>();
    }

    protected void Update()
    {
        if (IsPlayerActive)
        {
            UpdateInput();
            MouseControl();
            Move();
            CheckShoot();
            HandleFootsteps();
        }
        else
        {
            playerVelocity.y += playerLocomotion.gravity * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);
        }
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

            //Debug.Log(inputManager.GetRotationAxis());

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
            EventHandler.ExecuteEvent(gameObject,GameEvents.OnPlayerFire);
            anim.SetBool("IsShooting", true);
        }
        else
        {
            anim.SetBool("IsShooting", false);
        }
    }

    private void PlayerDied()
    {
        IsPlayerActive = false;
        characterController.height = 0.7f;
        tpsController.DeadWeight();
        tps_anim.Play("death");
        anim.gameObject.SetActive(false);
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

        targetDirection = transform.right * horizontal + transform.forward * vertical;
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

    private void HandleFootsteps()
    {
        if (characterController.isGrounded && targetDirection.normalized.magnitude > 0)
        {
            float stepInterval = playerLocomotion.walkStepInterval;

            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                if (footstepClips.Length > 0)
                {
                    int index = UnityEngine.Random.Range(0, footstepClips.Length);
                    AudioClip clip = footstepClips[index];

                    footstepSource.PlayOneShot(clip);
                }
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}
