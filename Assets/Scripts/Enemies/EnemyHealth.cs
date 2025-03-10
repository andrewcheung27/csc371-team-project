using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 10;  // current health
    public int maxHealth = 10;  // max health
    private int minHealth = 0;

    [Header("Score Settings")]
    public int score = 100; // Points for killing the enemy
    public float scorePopupHeight = 0f;  // how high above the enemy to show score popup when it dies

    [Header("Health Bar UI")]
    [SerializeField] private Healthbar healthbar; // Assign the Healthbar script
    [SerializeField] private Transform healthBarCanvas; // Assign the Health Bar Canvas

    private bool isHealthBarVisible = false; // Track if health bar is visible

    void Start()
    {
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
            // show health bar if enemy starts damaged
            if (health < maxHealth) {
                healthBarCanvas.gameObject.SetActive(true);
                healthbar.UpdateHealthBar(maxHealth, health);
            }
            // otherwise, hide health bar initially
            else {
                healthBarCanvas.gameObject.SetActive(false);
            }
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

        // Ensure enemy dies when health reaches zero
        if (health <= minHealth)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.instance.AddToScore(score);
        GameManager.instance.ShowScorePopup(transform.position + new Vector3(0f, scorePopupHeight, 0f), score);

        // Destroy health bar UI when enemy dies
        if (healthBarCanvas != null)
        {
            Destroy(healthBarCanvas.gameObject);
        }
        if (gameObject.name == "Hunter") // Replace "Hunter" with your actual Hunter enemy's GameObject name
        {
            AudioManager.instance.HunterDamage(); // Play the Hunter's death sound
        }
        else
        {
            // Add any additional sounds for other enemies here
        }


        Destroy(gameObject);
    }
}