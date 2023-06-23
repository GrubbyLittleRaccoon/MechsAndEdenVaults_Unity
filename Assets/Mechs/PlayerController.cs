using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller; // Set in inspector

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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // This line will set the camera rotation to the "default" upright position
        cam.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        //Section 1: Mouse/Camera input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the entire player on horizontal axis
        transform.Rotate(Vector3.up * mouseX); // Horizontal rotation

        // Rotate only the camera on vertical axis
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Section 2: Movement input
        // Gather input for horizontal (side/forward) movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x) + (transform.forward * z);

        // Jump
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply movement and gravity
        controller.Move(move * speed * Time.deltaTime + velocity * Time.deltaTime);
    }
}
