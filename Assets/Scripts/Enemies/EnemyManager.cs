using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; // Singleton instance

    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs; // Assign different enemy prefabs in the Inspector
    public int numberOfEnemies = 5; // Number of enemies to spawn
    public float spawnDelay = 2f; // Delay between enemy spawns
    public bool respawnEnemies = false; // Should enemies respawn after being defeated?

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>(); // Assign spawn points in Inspector

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void SpawnEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogError("EnemyManager: No spawn points or enemy prefabs assigned!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)]; // Pick a random spawn point
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]; // Pick a random enemy type

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    public void EnemyDefeated(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        Destroy(enemy);

        if (respawnEnemies)
        {
            StartCoroutine(RespawnEnemy());
        }
    }

    IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnEnemy();
    }
}
