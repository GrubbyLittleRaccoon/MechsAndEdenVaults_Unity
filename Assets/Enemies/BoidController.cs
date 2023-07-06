using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    // Movement
    public float speed = 12f;
    public float jumpHeight = 0.5f;



    private Vector3 currentVelocity;

    // Boids algorithm
    public float neighborRadius = 5f;
    public float separationAmount = 1.5f;
    public LayerMask boidLayer;

    // Ground check
    private float groundDistance = 0.4f;
    private bool isGrounded;
    public LayerMask groundMask;

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
        // Initialize with random forces
        currentVelocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        // Ground collision detection
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);

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
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime);

        // Check if currentVelocity is not a zero vector
        if (currentVelocity != Vector3.zero)
        {
            transform.forward = currentVelocity.normalized;
        }

        transform.position += currentVelocity * speed * Time.deltaTime;
    }
}