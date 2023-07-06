using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public float speed = 12f;
    public float jumpHeight = 0.5f;

    // Ground collision
    private Transform groundCheck; // Check if we hit the ground
    private float groundDistance = 0.4f; // Radius of ground checking sphere
    public LayerMask groundMask; // Control what objects to check for
    bool isGrounded;

    // Dome collision
    public GameObject dome; // Avoid the dome
    private float domeGroundRadius; // So we know what to avoid
    private Vector3 domeCentre; // Gives direction to dome repelling force

    // Start is called before the first frame update
    void Start()
    {
        //Optimise through constructor once we know it works
        domeGroundRadius = dome.GetComponent<DomeManager>().getGroundRadius();
        domeCentre = dome.transform.position;
        Debug.Log("domeCentre: " + domeCentre);
    }

    // Update is called once per frame
    void Update()
    {
        // Perform ground collision detection
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Movement
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // Testing basic movement

        // Calculate forces
        // Standard boid forces
        // Dome forces
        // Obstacle forces
        // Boid+ forces
        // Intelligence from leader
    }
}
