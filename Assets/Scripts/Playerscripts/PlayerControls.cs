using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerControls : NetworkBehaviour
{
    public Rigidbody rb;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] PlayerInputHandler playerInputHandler;

    public float speed = 12f;
    public float jumpForce = 3f;
    public float acceleration = 5f;
    public float deceleration = 5f;
    public float velPower = 0.87f;

    public float GroundDrag = 2;
    public float AirDrag = 0.1f;
    public float ForceMultiplier = 10f;

    public float sprintMultiplier = 1.7f;
    private float speedMultiplier;
    public float airMultiplier = 0.3f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBuffercounter;
    private float jumpBufferTime = 0.2f;

    public Transform playerBody;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    Vector3 moveDirection;
    bool isGrounded;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        if (isLocalPlayer)
        {
            jumpBuffercounter -= Time.deltaTime;

            //ground check
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                rb.drag = GroundDrag;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
                rb.drag = AirDrag;
            }

            SpeedControl();
        }
    }
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (jumpBuffercounter > 0 && coyoteTimeCounter > 0)
            {
                Jump();
            }
            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                coyoteTimeCounter = 0f;
            }

            move();
        }
    }
    private void move()
    {
        moveDirection = playerBody.transform.forward * playerInputHandler.MovementVector.y + playerBody.transform.right * playerInputHandler.MovementVector.x;
        //slow walking
        if (Input.GetButton("Fire3"))
        {
            speedMultiplier = sprintMultiplier;
        }
        else
        {
            speedMultiplier = 1;
        }
        if (isGrounded) rb.AddForce(moveDirection.normalized * playerStats.ActiveWeapon.Mobility * speed * speedMultiplier * ForceMultiplier, ForceMode.Force);
        else if (!isGrounded) rb.AddForce(moveDirection.normalized * playerStats.ActiveWeapon.Mobility * speed * airMultiplier * ForceMultiplier, ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    public void Jump_Performed(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            jumpBuffercounter = jumpBufferTime;
        }
    }
}
