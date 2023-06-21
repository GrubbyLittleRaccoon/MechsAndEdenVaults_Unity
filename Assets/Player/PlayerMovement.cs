using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller; // Have to set in inspector

    public float speed = 12f; // TODO: directional speed
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundStick = -2f;

    // Ground collision logic
    public Transform groundCheck; // Check if we hit the ground
    public float groundDistance = 0.4f; // Radius of ground checking sphere
    public LayerMask groundMask; // Control what objects to check for
    bool isGrounded;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // Not inputs can be grabbed from Edit->Project Settings->Input manager
    void Update()
    {
        // Vertical movement/physics
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) { // <0 check stops clash with jumping
            velocity.y = -2f; // Apparently forcing player to the ground is better
            // TODO: Try playing with velocity.y = 0;
        } else {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

        // Jump equation
        if(Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * groundStick * gravity);
        }

        // Horizontal plane
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x) + (transform.forward * z);

        controller.Move(move * speed * Time.deltaTime);

    }
}
