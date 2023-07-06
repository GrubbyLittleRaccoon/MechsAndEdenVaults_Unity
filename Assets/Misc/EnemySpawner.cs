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

    private int batchSize = 5; // Max to spawn in a single batch

    // Start is called before the first frame update
    void Start()
    {
        domeManager = dome.GetComponent<DomeManager>();
        float maxSpawnRadius = domeManager.getGroundRadius();
        Debug.Log("maxRadius: " + maxSpawnRadius);

        StartCoroutine(TriggerRandomly());
        StartCoroutine(InstantiateInBatches());
    }
    IEnumerator TriggerRandomly()
    {
        while (true)
        {
            // Wait for a random amount of seconds
            yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
            // Call your method here
            enemiesToSpawn += Random.Range(1, 10);

            Debug.Log("enemiesToSpawn: " + enemiesToSpawn);
        }
    }

    IEnumerator InstantiateInBatches()
    {
        while (true) // Outer infinite loop
        {
            while (enemiesToSpawn > 0)
            {
                for (int i = 0; i < batchSize; i++)
                {
                    float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
                    float theta = Random.Range(0, 360);
                    float spawnHeight2 = (domeRadius * domeRadius) - (spawnRadius * spawnRadius);
                    Debug.Log("spawnHeight: " + spawnHeight2 + " spawnRadius: " + spawnRadius + " domeRadius: " + domeRadius);
                    Vector3 spawnPosition = dome.transform.position + new Vector3(
                        spawnRadius * Mathf.Cos(theta * Mathf.Deg2Rad),
                        Mathf.Sqrt(spawnHeight2),
                        spawnRadius * Mathf.Sin(theta * Mathf.Deg2Rad)
                    );

                    Instantiate(enemyPrefabToSpawn, spawnPosition, Quaternion.identity); // Use spawnPosition instead of transform.position
                    enemiesToSpawn--;

                    if (enemiesToSpawn <= 0) // Break out of the loop if no more enemies need to be spawned
                        break;
                }

                yield return null; // Wait for the next frame
            }

            yield return new WaitForSeconds(1.0f); // Wait before checking again
        }
    }
}
