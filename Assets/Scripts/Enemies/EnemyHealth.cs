using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 10;
    private int minHealth = 0;
    private int maxHealth; // Stores max health

    [Header("Score Settings")]
    public int score = 100; // Points for killing the enemy

    [Header("Health Bar UI")]
    [SerializeField] private Healthbar healthbar; // Assign the Healthbar script
    [SerializeField] private Transform healthBarCanvas; // Assign the Health Bar Canvas

    private Camera cam; // Camera reference
    private bool isHealthBarVisible = false; // Track if health bar is visible

    void Start()
    {
        maxHealth = health; // Store initial health value
        cam = Camera.main; // Get reference to the main camera

        if (healthbar == null)
        {
            Debug.LogError("EnemyHealth: Healthbar not assigned in Inspector!");
        }
        if (healthBarCanvas == null)
        {
            Debug.LogError("EnemyHealth: HealthBarCanvas not assigned in Inspector!");
        }
        else
        {
            healthBarCanvas.gameObject.SetActive(false); // Initially hide the health bar
        }
    }

    void Update()
    {
        if (healthBarCanvas != null && isHealthBarVisible)
        {
            // Ensure the health bar always faces the X-axis
            healthBarCanvas.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    public void AddToHealth(int n)
    {
        if (!isHealthBarVisible && healthBarCanvas != null)
        {
            healthBarCanvas.gameObject.SetActive(true); // Show the health bar when hit
            isHealthBarVisible = true;
        }

        health += n;
        health = Mathf.Clamp(health, minHealth, maxHealth); // Ensure health stays within limits

        // Update health bar
        if (healthbar != null)
        {
            healthbar.UpdateHealthBar(maxHealth, health);
        }

        Debug.Log($"Enemy health updated: {health}/{maxHealth}");

        // Ensure enemy dies when health reaches zero
        if (health <= minHealth)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");

        GameManager.instance.AddToScore(score);
        GameManager.instance.ShowScorePopup(transform.position, score);

        // Destroy health bar UI when enemy dies
        if (healthBarCanvas != null)
        {
            Destroy(healthBarCanvas.gameObject);
        }

        Destroy(gameObject);
    }
}