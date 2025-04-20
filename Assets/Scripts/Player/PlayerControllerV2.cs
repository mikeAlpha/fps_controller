using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PlayerControllerV2 : MonoBehaviour, IPlayerController
{
    private GameObject rsp;

    private float yaw;
    private float pitch;
    private float currentYaw;
    private float currentPitch;
    private float yawVelocity;
    private float pitchVelocity;
    private bool IsAiming = false;

    private CharacterController characterController;
    private Vector3 moveVelocity;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private Transform ArmTransform;

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
        runStepInterval = 0.3f,
        moveDampTime = 0.5f,
        moveMultiplierForDamp = 8f
    };

    private GameObject startPos;
    private float stepTimer = 0f;
    private Vector3 targetDirection;

    [SerializeField]
    public PlayerIK playerIK = new PlayerIK()
    {
        RighthandWeight = 1.0f,
        LefthandWeight = 1.0f,
        BodyWeight = 1.0f,
        HeadWeight = 1.0f,
        EyeWeight = 1.0f,
        MainWeight = 1.0f
    };

    [HideInInspector]
    public AudioSource audioSource;
    Animator anim;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerHealth playerHealth;

    [HideInInspector]
    public bool IsPlayerActive = true;
    private void OnEnable()
    {
        EventHandler.RegisterEvent(gameObject, GameEvents.OnPlayerDied, PlayerDied);
    }

    private void OnDisable()
    {
        EventHandler.UnregisterEvent(gameObject, GameEvents.OnPlayerDied, PlayerDied);
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        ArmTransform = playerIK.ShoulderIK.GetChild(0);
    }

    private void Start()
    {
        inputManager = new InputManager();
        Cursor.lockState = CursorLockMode.Locked;
        InitIK();
        OnInitInventory();
        playerHealth = new PlayerHealth(gameObject);
    }

    private void Update()
    {
        if (IsPlayerActive)
        {
            HandleShoulder();
            UpdateInput();
            CheckShoot();
            MouseControl();
            UpdateCamera();
            Move();
        }
        else
        {
            playerVelocity.y += playerLocomotion.gravity * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void UpdateCamera()
    {
        playerLookSettings.CameraTranform.position = playerLookSettings.HeadBone.transform.position;
    }

    public Transform GetCameraTransform()
    {
        return playerLookSettings.CameraTranform;
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
            playerLookSettings.CameraTranform.localRotation = Quaternion.Euler(-currentPitch, 0f, 0f);
        }
    }

    void ToggleScope()
    {
        IsAiming = !IsAiming;
        if (IsAiming)
        {
            while (Vector3.Distance(playerLookSettings.CameraTranform.transform.position, playerLookSettings.MagPos.position) > 0.0001f)
            {
                playerLookSettings.fpsCamObj.transform.position = Vector3.Lerp(playerLookSettings.fpsCamObj.transform.position, playerLookSettings.MagPos.position, Time.smoothDeltaTime * 5f);
                playerLookSettings.fpsCamObj.transform.rotation = playerLookSettings.MagPos.rotation;
                playerLookSettings.fpsCamObj.fieldOfView = 25f;
                //Crosshair.SetActive(false);
            }
        }
        else
        {
            while (Vector3.Distance(playerLookSettings.CameraTranform.transform.position, startPos.transform.position) > 0.0001f)
            {
                playerLookSettings.fpsCamObj.transform.position = Vector3.Lerp(playerLookSettings.fpsCamObj.transform.position, startPos.transform.position, Time.deltaTime * 5f);
                playerLookSettings.fpsCamObj.fieldOfView = 50f;
                //Crosshair.SetActive(true);
            }
        }
    }

    private void UpdateInput()
    {
        inputManager.UpdateInput();
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

        var dir = new Vector3(horizontal, 0, vertical).normalized;

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

        //anim.SetFloat("Speed", targetDirection.normalized.magnitude, 0.1f, 0.1f);
        anim.SetFloat("SpeedX", dir.x, playerLocomotion.moveDampTime, playerLocomotion.moveMultiplierForDamp * Time.deltaTime);
        anim.SetFloat("SpeedY", dir.z, playerLocomotion.moveDampTime, playerLocomotion.moveMultiplierForDamp * Time.deltaTime);

    }

    private void CheckShoot()
    {
        if (inputManager.GetMouseDown(0))
        {
            EventHandler.ExecuteEvent(gameObject, GameEvents.OnPlayerFire);
            //anim.SetBool("IsShooting", true);
        }
        else
        {
            //anim.SetBool("IsShooting", false);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        SolveIK();
    }

    void InitIK()
    {
        rsp = new GameObject("IKHandler");
        rsp.transform.parent = transform;
    }

    void HandleShoulder()
    {
        playerIK.ShoulderBone.LookAt(playerIK.LookAtPosition);
        rsp.transform.position = playerIK.ShoulderBone.TransformPoint(Vector3.zero);
        playerIK.ShoulderIK.transform.position = rsp.transform.position;

        playerIK.ShoulderIK.LookAt(playerIK.LookAtPosition);
    }

    void ResetWeights(bool val)
    {
        StartCoroutine(ResetWeightInternal(val));
    }

    private void PlayerDied()
    {
        IsPlayerActive = false;
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        //characterController.enabled = false;
        DeadWeight();
        StartCoroutine(ResetLevel());

        //anim.Play("death");
        //gameObject.SetActive(false);
    }

    IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DeadWeight()
    {
        playerIK.RighthandWeight = playerIK.LefthandWeight = playerIK.BodyWeight = playerIK.HeadWeight = playerIK.EyeWeight = playerIK.MainWeight = 0.0f;
    }

    IEnumerator ResetWeightInternal(bool val)
    {
        yield return new WaitForSeconds(0.3568f);
        if (!val)
        {
            playerIK.RighthandWeight = playerIK.LefthandWeight = 1.0f;
        }
        else
        {
            playerIK.RighthandWeight = playerIK.LefthandWeight = 0.0f;
        }
    }

    //void HandleIKAnimationState()
    //{
    //    if(hX == 0 && hZ == 0)
    //    {
    //        RightHandRef.position = Idle.RightHandRef;
    //        LeftHandRef.position = Idle.LeftHandRef;
    //        RightElbowRef.position = Idle.RightElbowRef;
    //        LeftElbowRef.position = Idle.LeftElbowRef;
    //    }
    //    else
    //    {
    //        RightHandRef.position = Run.RightHandRef;
    //        LeftHandRef.position = Run.LeftHandRef;
    //        RightElbowRef.position = Run.RightElbowRef;
    //        LeftElbowRef.position = Run.LeftElbowRef;
    //    }
    //}

    void SolveIK()
    {
        anim.SetIKPosition(AvatarIKGoal.RightHand, playerIK.RightHandRef.position);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, playerIK.LeftHandRef.position);

        anim.SetIKRotation(AvatarIKGoal.RightHand, playerIK.RightHandRef.rotation);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, playerIK.LeftHandRef.rotation);

        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, playerIK.RighthandWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, playerIK.LefthandWeight);

        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, playerIK.RighthandWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, playerIK.LefthandWeight);

        anim.SetIKHintPosition(AvatarIKHint.RightElbow, playerIK.RightElbowRef.position);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, playerIK.LeftElbowRef.position);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, playerIK.RighthandWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, playerIK.LefthandWeight);

        anim.SetLookAtPosition(playerIK.LookAtPosition.position);
        anim.SetLookAtWeight(playerIK.MainWeight, playerIK.BodyWeight, playerIK.HeadWeight, playerIK.EyeWeight);
    }

    private void ApplyWeaponIK(WeaponController wp_controller, BaseWeapon wp_config)
    {
        playerIK.RightHandRef = wp_controller.rightHandIK;
        playerIK.LeftHandRef = wp_controller.leftHandIK;

        playerIK.RightHandRef.parent = ArmTransform;
        playerIK.LeftHandRef.parent = ArmTransform;

        playerIK.RightHandRef.localPosition = wp_config.rightArmPos.pos;
        playerIK.RightHandRef.localRotation = Quaternion.Euler(wp_config.rightArmPos.rot);

        playerIK.LeftHandRef.localPosition = wp_config.leftArmPos.pos;
        playerIK.LeftHandRef.localRotation = Quaternion.Euler(wp_config.leftArmPos.rot);
        //RightHandRef.position = rightHand.position;
        //RightElbowRef.position = rightElbow.position;

        //RightHandRef.rotation = Quaternion.Inverse(rightHand.rotation);
        //RightElbowRef.rotation = rightElbow.rotation;

        //LeftHandRef.position = leftHand.position;
        //LeftElbowRef.position = leftElbow.position;

        //LeftHandRef.rotation = leftHand.rotation;
        //LeftElbowRef.rotation = leftElbow.rotation;
    }
}

public interface IPlayerController { }
