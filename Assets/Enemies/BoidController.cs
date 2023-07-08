using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    // Overall movement
    private Rigidbody boidRigidbody;
    public float maxSpeed = 12f;
    public float jumpHeight = 0.5f;
    public float terminalVelocity = -20f; // Terminal fall velocity, experiment from 10-20


    // Boids algorithm
    public float neighborRadius = 5f;
    public float separationAmount = 1.5f;
    public LayerMask boidLayer;

    // Ground check
    private float groundDistance = 1.0f; // Radius for checking grounded
    private bool isGrounded = false;
    public LayerMask groundLayer;

    // Need to avoid the sides of the dome
    private float domeGroundRadius;
    private Vector3 domeCentre;

    public void setDomeProperties(float inputRadius, Vector3 inputCentre)
    {
        domeGroundRadius = inputRadius;
        domeCentre = inputCentre;
    }

    void Start()
    {
        boidRigidbody = GetComponent<Rigidbody>();
        // Initialize with random forces
        boidRigidbody.velocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        boidRigidbody.maxLinearVelocity = maxSpeed;
    }

    // Physics based code better in fixedupdate
    void FixedUpdate()
    {
        // Ground collision detection
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundLayer);
        /*
        Debug.Log("Debugging isGrounded");
        Debug.Log(transform.position);
        Debug.Log(groundDistance);
        Debug.Log(groundLayer);*/

        if (isGrounded)
        {
            Debug.Log("ground collision detected");
            // Boid forces
            Vector3 alignment = Vector3.zero;
            Vector3 cohesion = Vector3.zero;
            Vector3 separation = Vector3.zero;

            Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius, boidLayer);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.gameObject != this.gameObject)
                {
                    alignment += neighbor.transform.forward;
                    cohesion += neighbor.transform.position;
                    Vector3 directionToNeighbor = transform.position - neighbor.transform.position;
                    separation += directionToNeighbor / directionToNeighbor.sqrMagnitude;
                }
            }

            if (neighbors.Length > 1)
            {
                alignment /= neighbors.Length - 1;
                cohesion = cohesion / (neighbors.Length - 1) - transform.position;
                separation /= neighbors.Length - 1;
            }

            Vector3 targetVelocity = alignment + cohesion + separation * separationAmount;

            // Steer towards the target velocity
            Vector3 deltaVelocity = Vector3.Lerp(boidRigidbody.velocity, targetVelocity, Time.deltaTime);

            // Check if currentVelocity is not a zero vector
            if (deltaVelocity != Vector3.zero)
            {
                transform.forward = deltaVelocity.normalized; // Set forward
            }

            // Apply the force to the Rigidbody //TODO cap speed
            boidRigidbody.AddForce(deltaVelocity);
        }
        else
        {
            //Trying to stop clipping
            if (boidRigidbody.velocity.y < terminalVelocity) // Cap fall speed
            {
                Debug.Log("capped");
                boidRigidbody.velocity = new Vector3(boidRigidbody.velocity.x, terminalVelocity, boidRigidbody.velocity.z); // Avoid gliding/sinking from calculation issues
            }
        }
    }
}