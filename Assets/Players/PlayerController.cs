using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller; // Set in inspector, TODO see if can add programmatically

    public float speed = 12f;
    public float gravity = -9.81f; // Note we're using negative gravity
    public float jumpHeight = 0.5f;
    public float groundStick = -2f;

    // Attach to player camera in the inspector
    public Transform cam;

    // Mouselogic
    public float mouseSensitivity = 300f;
    public float xRotation = 0f;

    // Ground collision logic
    public Transform groundCheck; // Check if we hit the ground
    public float groundDistance = 0.4f; // Radius of ground checking sphere
    public LayerMask groundMask; // Control what objects to check for
    bool isGrounded;
    Vector3 velocity; // Currently only really used for vertical component, but keeping separate from movement input for future mobility equipment

    // Input handling
    PlayerInput playerInput;
    Vector2 move;
    Vector2 look;

    // As the game starts, before Start()
    void Awake()
    {
        // Link actions to methods
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Movement"].performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        playerInput.actions["Movement"].canceled += ctx => OnMove(Vector2.zero);
        playerInput.actions["Camera"].performed += ctx => OnLook(ctx.ReadValue<Vector2>());
        playerInput.actions["Camera"].canceled += ctx => OnLook(Vector2.zero);
        playerInput.actions["Jump"].performed += ctx => OnJump();

        Cursor.lockState = CursorLockMode.Locked;
        cam.localRotation = Quaternion.identity; //Zero initial rotation
    }

    private void OnMove(Vector2 moveValue)
    {
        move = moveValue;
    }

    private void OnLook(Vector2 lookValue)
    {
        look = lookValue;
    }

    private void OnJump()
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Section 1: Camera input
        float lookDeltaX = look.x * mouseSensitivity * Time.deltaTime;
        float lookDeltaY = look.y * mouseSensitivity * Time.deltaTime;
        // Rotate the entire player on horizontal axis
        transform.Rotate(Vector3.up * lookDeltaX);
        // Rotate only the camera on vertical axis
        xRotation -= lookDeltaY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Section 2: Movement input
        Vector3 moveVector = new Vector3(move.x, 0, move.y);
        // Transform the moveVector to the camera's direction instead of the player body's direction (tank-style movement)
        moveVector = transform.TransformDirection(moveVector);

        // Section 3: Gravity
        velocity.y += gravity * Time.deltaTime; //General gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Avoid gliding/sinking from calculation issues
        }

        // Apply movement
        controller.Move(moveVector * speed * Time.deltaTime + velocity * Time.deltaTime);
    }
}
