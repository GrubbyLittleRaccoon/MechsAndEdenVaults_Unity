using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller; // Set in inspector, TODO see if can add programmatically

    public float speed = 12f;
    public float gravity = -9.81f; // Note we're using negative gravity
    public float jumpHeight = 3f;
    public float groundStick = -2f;

    // Assuming this is the main camera, attach to player camera in the inspector
    public Transform cam;

    // Mouselogic
    public float mouseSensitivity = 300f;
    public float xRotation = 0f;

    // Ground collision logic
    public Transform groundCheck; // Check if we hit the ground
    public float groundDistance = 0.4f; // Radius of ground checking sphere
    public LayerMask groundMask; // Control what objects to check for
    bool isGrounded;

    Vector3 velocity;

    // New playerinput stuff
    PlayerControls playerControls; // Generated from player controls asset
    Vector2 move;
    Vector2 rotate;

    // As the game starts, before Start()
    void Awake()
    {
        playerControls = new PlayerControls();

        // Pulling values defined in inputactions component
        playerControls.Player.Movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        playerControls.Player.Movement.canceled += ctx => move = Vector2.zero;
        playerControls.Player.Camera.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        playerControls.Player.Camera.canceled += ctx => rotate = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // This line will set the camera rotation to the "default" upright position
        cam.localRotation = Quaternion.identity;
    }

    //Enable/disable player input spat out by AI, will research whether useful
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Section 1: /Camera input
        float mouseX = rotate.x * mouseSensitivity * Time.deltaTime;
        float mouseY = rotate.y * mouseSensitivity * Time.deltaTime;

        // Rotate the entire player on horizontal axis
        transform.Rotate(Vector3.up * mouseX); // Horizontal rotation

        // Rotate only the camera on vertical axis
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Section 2: Movement input
        Vector3 movement = new Vector3(move.x, 0, move.y);
        movement = transform.TransformDirection(movement); //???

        // Jump
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (playerControls.Player.Jump.triggered)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply movement and gravity
        controller.Move(movement * speed * Time.deltaTime + velocity * Time.deltaTime);
    }
}
