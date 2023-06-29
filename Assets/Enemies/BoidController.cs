using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public float speed = 12f;
    public float jumpHeight = 0.5f;

    // Ground collision logic
    public Transform groundCheck; // Check if we hit the ground
    public float groundDistance = 0.4f; // Radius of ground checking sphere
    public LayerMask groundMask; // Control what objects to check for
    bool isGrounded;

    private Rigidbody rb; // Reference to the Rigidbody component

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Perform ground collision detection
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Implement movement logic here
        // Example: transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
