using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; // Singleton instance

    [Header("Health Pack Settings")]
    public GameObject healthPackPrefab; // Assign health pack prefab in Inspector
    [Range(0f, 1f)]
    public float healthPackDropChance = 0.3f; // 30% drop chance by default

    private List<GameObject> activeEnemies = new List<GameObject>(); // Track active enemies

    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this when an enemy is created to add it to the active list
    public void RegisterEnemy(GameObject enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    // Call this when an enemy is defeated
    public void EnemyDefeated(GameObject enemy)
    {
        activeEnemies.Remove(enemy);

        if (enemy.name == "Boss")
        {
            // Trigger the MiniBossDeath routine instead of destroying it
            MiniBossDeath bossDeath = enemy.GetComponent<MiniBossDeath>();
            if (bossDeath != null)
            {
                bossDeath.TriggerDeath();
                return; // Prevent the enemy from being destroyed immediately
            }
        }

        // Try dropping a health pack at enemy's position
        if (enemy.name == "Slasher") {
            TryDropHealthPack(enemy.transform.position + new Vector3(0f, 1f, 0f));
        }
        else {
            TryDropHealthPack(enemy.transform.position);
        }

        // Destroy the enemy GameObject
        Destroy(enemy);
    }

    public void TryDropHealthPack(Vector3 position, bool doRandomDropChance=true, int healAmount=-1)
    {
        if (healthPackPrefab == null) return; // Safety check

        if (!doRandomDropChance || Random.value <= healthPackDropChance)
        {
            GameObject healthPack = Instantiate(healthPackPrefab, position, Quaternion.identity);
            // set heal amount if one is provided. must be > 0
            if (healAmount > 0 && healthPack.TryGetComponent(out HealthPack hp)) {
                hp.SetHealAmount(healAmount);
            }
        }
    }

    // Optional: Get list of active enemies (for other systems, like wave manager)
    public List<GameObject> GetActiveEnemies()
    {
        return activeEnemies;
    }

    public void SetHealthPackDropChance(float f)
    {
        healthPackDropChance = f;
    }
}
