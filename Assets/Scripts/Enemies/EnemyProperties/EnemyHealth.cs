using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public string enemyName;  // name of this enemy

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

    // play audio based on enemy name
    void PlayDefeatedAudio()
    {
        switch (enemyName) {
            case "Hunter":
                AudioManager.instance.HunterDamage();
                break;
            default:
                break;
        }
    }

    void Die()
{
    if (!enabled) return; // Don't run this function if the script is disabled
    
    GameManager.instance.AddToScore(score);
    GameManager.instance.ShowScorePopup(transform.position + new Vector3(0f, scorePopupHeight, 0f), score);

    // Tell EnemyManager that this enemy died (so it can handle health pack drop and tracking)
    EnemyManager.instance.EnemyDefeated(gameObject);

    // death sound
    PlayDefeatedAudio();

    // Destroy health bar UI when enemy dies
    if (healthBarCanvas != null)
    {
        Destroy(healthBarCanvas.gameObject);
    }

    // EnemyManager will handle destroying the enemy
    // So we DON'T need Destroy(gameObject); here anymore!
}

}