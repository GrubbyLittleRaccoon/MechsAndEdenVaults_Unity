using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Dome/radius
    public GameObject dome;
    private DomeManager domeManager;
    public float minSpawnRadius = 30; // Minimum spawn radius from the centre of the dome - enemies don't spawn too close
    [SerializeField]
    private float maxSpawnRadius;
    private float domeRadius;

    // Instantiation parameters
    private int enemiesToSpawn;
    private int totalSpawned = 0; // Keep track of how many enemies have been spawned so far (TODO: need to add decrement later on death)
    public int upperSpawnLimit = 100; // Max enemies to spawn in total
    public float spawnPeriod = 2.0f; //In seconds
    public int spawnQuantity = 3; //Has to be > 0
    private int batchSize = 3; // Limit instantiation to batches to avoid frame lag
    public GameObject enemyPrefabToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        domeManager = dome.GetComponent<DomeManager>();
        maxSpawnRadius = domeManager.getGroundRadius();
        domeRadius = domeManager.getGroundRadius();

        StartCoroutine(RandomlyAddEnemiesToSpawn());
        StartCoroutine(InstantiateInBatches());
    }

    // This runs continuously
    IEnumerator RandomlyAddEnemiesToSpawn()
    {
        while (true)
        {
            // Wait for a random amount of seconds
            yield return new WaitForSeconds(Random.Range(0.0f, spawnPeriod));
            // Call your method here
            enemiesToSpawn += Random.Range(1, spawnQuantity);
        }
    }

    // Contually run batch instantiation.
    // Helps performance by limiting instantiation rate. 
    // Not sure if I should be concerned about frame rates though - batch sizes occur on a per-frame basis.
    // Can always add that to "yield return null" consideration later on.
    IEnumerator InstantiateInBatches()
    {
        float spawnInterval = 0.125f; // Time in seconds between batching - animating on threes.
        float timeSinceLastSpawn = 0f;
        while (true)
        {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= spawnInterval && enemiesToSpawn > 0)
            {
                for (int i = 0; i < batchSize; i++)
                {
                    if (totalSpawned >= upperSpawnLimit)
                    {
                        break;
                    }
                    float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
                    float theta = Random.Range(0, 360);
                    float spawnHeight = Mathf.Sqrt((domeRadius * domeRadius) - (spawnRadius * spawnRadius));
                    Vector3 spawnPosition = dome.transform.position + new Vector3(
                        spawnRadius * Mathf.Cos(theta * Mathf.Deg2Rad),
                        spawnHeight,
                        spawnRadius * Mathf.Sin(theta * Mathf.Deg2Rad)
                    );
                    GameObject newBoid = Instantiate(enemyPrefabToSpawn, spawnPosition, Quaternion.identity);
                    BoidController boidController = newBoid.GetComponent<BoidController>();
                    boidController.setDomeProperties(domeRadius, new Vector3(0, 0, 0)); // Assume dome is already at origin TODO: check this

                    enemiesToSpawn--;
                    totalSpawned++;
                    if (enemiesToSpawn <= 0)
                    {
                        break;
                    }
                }
                timeSinceLastSpawn = 0f; // Reset the timer
            }
            yield return null; // Wait for the next frame
        }
    }

}
