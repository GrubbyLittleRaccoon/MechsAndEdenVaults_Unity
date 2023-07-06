using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject dome;
    private DomeManager domeManager;

    public float minSpawnRadius = 30; // Minimum spawn radius from the centre of the dome - enemies don't spawn too close
    [SerializeField]
    private float maxSpawnRadius;
    private float domeRadius;

    private int enemiesToSpawn;

    public GameObject enemyPrefabToSpawn;

    private int batchSize = 3; // Max to spawn in a single batch

    // Start is called before the first frame update
    void Start()
    {
        domeManager = dome.GetComponent<DomeManager>();
        maxSpawnRadius = domeManager.getGroundRadius();
        domeRadius = domeManager.getGroundRadius();

        StartCoroutine(TriggerRandomly());
        StartCoroutine(InstantiateInBatches());
    }
    IEnumerator TriggerRandomly()
    {
        while (true)
        {
            // Wait for a random amount of seconds
            yield return new WaitForSeconds(Random.Range(0.0f, 2.0f));
            // Call your method here
            enemiesToSpawn += Random.Range(1, 10);
        }
    }

    // Animating on 
    IEnumerator InstantiateInBatches()
    {
        float spawnInterval = 0.1f; // Time in seconds between spawns
        float timeSinceLastSpawn = 0f;
        while (true)
        {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= spawnInterval && enemiesToSpawn > 0)
            {
                for (int i = 0; i < batchSize; i++)
                {
                    float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
                    float theta = Random.Range(0, 360);
                    float spawnHeight = Mathf.Sqrt((domeRadius * domeRadius) - (spawnRadius * spawnRadius));
                    Vector3 spawnPosition = dome.transform.position + new Vector3(
                        spawnRadius * Mathf.Cos(theta * Mathf.Deg2Rad),
                        spawnHeight,
                        spawnRadius * Mathf.Sin(theta * Mathf.Deg2Rad)
                    );
                    Instantiate(enemyPrefabToSpawn, spawnPosition, Quaternion.identity);
                    enemiesToSpawn--;
                    if (enemiesToSpawn <= 0)
                        break;
                }
                timeSinceLastSpawn = 0f; // Reset the timer
            }
            yield return null; // Wait for the next frame
        }
    }



}
