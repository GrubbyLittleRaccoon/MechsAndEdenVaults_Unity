using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    // Overall movement
    private Rigidbody boidRigidbody;
    public float maxSpeed = 12f;
    public float jumpHeight = 0.5f;
    public float terminalVelocity = -30f; // Terminal fall velocity, started from -15f
    public float uprightForce = 5f; // Adjust this value to control how strongly the creatures try to stay upright


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

        // Personal preferences of front-down facing spawning for now
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    // Transform updates
    void Update()
    {
        // Set front face position
        /*
        if (boidRigidbody.velocity != Vector3.zero)
        {
            transform.forward = boidRigidbody.velocity.normalized; // Set forward based on current velocity
        }*/
    }

    // Phyiscs/rigidbody updates
    void FixedUpdate()
    {
        // Ground collision detection
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundLayer);
        if (isGrounded)
        {
            // Boid forces
            Vector3 alignment = Vector3.zero;
            Vector3 cohesion = Vector3.zero;
            Vector3 separation = Vector3.zero;

            Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborRadius, boidLayer);

            //Sum of boid effects
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

            // Averaging boid effects
            if (neighbors.Length > 1)
            {
                alignment /= neighbors.Length - 1;
                cohesion = cohesion / (neighbors.Length - 1) - transform.position;
                separation /= neighbors.Length - 1;
            }

            Vector3 targetVelocity = alignment + cohesion + (separation * separationAmount);

            // Apply boid steering force
            // Steer towards the target velocity
            Vector3 deltaVelocity = Vector3.Lerp(boidRigidbody.velocity, targetVelocity, Time.deltaTime);
            // boidRigidbody.AddForce(deltaVelocity);

            // Apply self-righting force
            Vector3 uprightTorque = Vector3.Cross(transform.up, Vector3.up) * uprightForce; // Assume flat world TODO: Account for tilting terrain
            boidRigidbody.AddTorque(uprightTorque);
        }
        else
        {
            // Capping fall speed
            if (boidRigidbody.velocity.y < terminalVelocity)
            {
                boidRigidbody.velocity = new Vector3(boidRigidbody.velocity.x, terminalVelocity, boidRigidbody.velocity.z);
            }
            // Delete if clipped through ground
            if (boidRigidbody.position.y < 0)
            {
                this.SelfDestruction();
            }
        }
    }

    // Wrapping self destruct since there's likely larger implications that'll come up.
    // TODO: Deal with spawner limit, increase it or something
    void SelfDestruction()
    {
        Destroy(gameObject);
    }
}