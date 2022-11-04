// Character models are from mixamo -> https://mixamo.com

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Initializers")]
    public Transform cam;
    public Transform groundCheck;
    public Animator animator;
    public LayerMask groundMask;
    
    [Header("Player Settings")]
    private float speed = 6f;
    public float turnSmoothTime = 0.1f;
    
    [Header("Y Axis")]
    public float gravity = -18f;
    public float groundCheckSphereRadius = 0.4f;
    public float jumpHeight = 0.25f;
    
    private CharacterController controller;
    private Vector3 direction;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private float horizontalInput;
    private float verticalInput;
    private float turnSmoothVelocity;
    private float targetAngle;
    private float angle;
    private bool isGrounded;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        SetInput();
        HandleInput();
        SetHeadRotation();
        HandleGravity();
    }

    private void SetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;
    }

    private void HandleInput()
    {
        if (direction.magnitude >= 0.1f) {
            animator.SetBool("isWalking", true);
            SetHeadRotation();

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; // Calculates where player is supposed to move
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
        else animator.SetBool("isWalking", false);
    }

    private void SetHeadRotation()
    {
        targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; // Calculates where the player is supposed to look
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); // Smooths it
        transform.rotation = Quaternion.Euler(0f, angle, 0f); // Sets the rotation
    }

    private void HandleGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckSphereRadius, groundMask); // Sets ground value

        if (velocity.y < 0 && isGrounded)
            velocity.y = -0.1f; // Resets velocity
        
        if (Input.GetKeyDown("space") && isGrounded) {
            speed = 0f;
            animator.SetBool("isJumping", true);
            Invoke("Jump", 0.5f); // To sync with animation
        }

        velocity.y += gravity * Time.deltaTime * Time.deltaTime; // Adds gravity
        controller.Move(velocity);
    }

    private void Jump()
    {
        velocity.y = jumpHeight; // Pushes player above
        speed = 6f;
        Invoke("ResetJump", 1);
    }

    private void ResetJump()
    {
        animator.SetBool("isJumping", false);
    }
}
