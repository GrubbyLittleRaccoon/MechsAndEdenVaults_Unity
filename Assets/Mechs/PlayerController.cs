using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller; // Have to set in inspector

    public float speed = 12f; // TODO: directional speed
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundStick = -2f;

    // Mouselogic
    public float mouseSensitivity = 300f;
    public Transform playerBody; // Note this has to be set in the inspector
    public float xRotation = 0f;

    // Ground collision logic
    public Transform groundCheck; // Check if we hit the ground
    public float groundDistance = 0.4f; // Radius of ground checking sphere
    public LayerMask groundMask; // Control what objects to check for
    bool isGrounded;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting");
        // Lock cursor to centre
        Cursor.lockState = CursorLockMode.Locked;
        // Set initial rotation for cam predictability
        transform.Rotate(180, 0, 0); //TODO make it forwards
    }

    // Update is called once per frame
    void Update()
    {
        //Section 1: Mouse/Camera input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX, Space.World); // Handles horizontal rotation
        transform.Rotate(Vector3.left, mouseY, Space.Self); // Handles vertical rotation

        // Section 2: Movement input
        // Note: inputs may need to be configured in (Edit->Project Settings->Input manager)
        // Vertical movement/physics
        // TODO Debug grounding disabling jump, also look into refactor this section:
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            // <0 check stops clash with jumping
            // Apparently forcing player to the ground is better
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Jump equation
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * groundStick * gravity);
        }

        controller.Move(velocity * Time.deltaTime);

        // Horizontal plane
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x) + (transform.forward * z);
        controller.Move(move * speed * Time.deltaTime);
    }
}
