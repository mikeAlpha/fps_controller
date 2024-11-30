using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : BaseNetworkBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 720f;
    public float jumpForce = 40f;

    private Rigidbody rb;
    private bool isGrounded;

    public Animator anim , tps_anim;

    //temporary
    private InputManager inputManager;

    private void Awake()
    {
        inputManager = new InputManager();
        EventHandler.ExecuteEvent<InputManager>(GameEvents.OnInputManagerUpdate, inputManager);
    }

    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        UpdateInput();
        MouseControl();
        Move();
        Jump();
        CheckShoot();
    }
    private void UpdateInput()
    {
        inputManager.UpdateInput();
    }

    private void MouseControl()
    {
        EventHandler.ExecuteEvent(GameEvents.OnMouseUpdate);
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
        float horizontal = inputManager.GetInputAxis().x;
        float vertical = inputManager.GetInputAxis().y;

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        Debug.Log(direction);

        if (direction.magnitude >= 0.1f)
        {
            Vector3 dir = transform.TransformDirection(direction);
            Vector3 move = dir * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + move);
        }
        anim.SetFloat("Speed", direction.magnitude, 0.1f, 0.1f);
        tps_anim.SetFloat("SpeedX", direction.x, 0.1f,0.1f);
        tps_anim.SetFloat("SpeedY", direction.z, 0.1f, 0.1f);
    }

    private void Jump()
    {
        if (inputManager.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
