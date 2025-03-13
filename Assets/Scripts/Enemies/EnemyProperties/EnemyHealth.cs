using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public string enemyName = "";  // used to look up death sound

    [Header("Health Settings")]
    public int health = 10;
    private int minHealth = 0;
    private int maxHealth; // Stores max health

    [Header("Score Settings")]
    public int score = 100; // Points for killing the enemy

    [Header("Health Bar UI")]
    [SerializeField] private Healthbar healthbar; // Assign the Healthbar script
    [SerializeField] private Transform healthBarCanvas; // Assign the Health Bar Canvas

    [Header("Effects")]
    [SerializeField] private GameObject bloodEffectPrefab; // Assign the blood effect prefab
    public float bloodEffectDuration = 2f;
    [SerializeField] private GameObject headGameObject; // Assign the enemy's head GameObject

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

        if (bloodEffectPrefab == null)
        {
            Debug.LogError("EnemyHealth: bloodEffectPrefab not assigned in Inspector!");
        }

        if (headGameObject == null)
        {
            Debug.LogError("EnemyHealth: headGameObject not assigned in Inspector!");
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

        // show change on healthbar
        healthbar?.UpdateHealthBar(maxHealth, health);

        // Spawn blood effect when taking damage
        if (n < 0)
        {
            if (bloodEffectPrefab != null && headGameObject != null)
            {
                GameObject blood = Instantiate(bloodEffectPrefab, headGameObject.transform.position, Quaternion.identity);

                if (blood == null)
                {
                    Debug.LogError("EnemyHealth: Failed to instantiate blood effect.");
                }
                else
                {
                    // Set the blood effect to shoot upwards
                    blood.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    Destroy(blood, bloodEffectDuration);
                }
            }
        }

        // Ensure enemy dies when health reaches zero
        if (health <= minHealth)
        {
            Die();
        }
    }

    void PlayDeathSound()
    {
        // play death sound based on enemy name
        switch (enemyName) {
            case "Spitter":
                AudioManager.instance.SpitterDeath();
                break;
            case "Hunter":
                AudioManager.instance.HunterDeath();
                break;
            case "Slasher":
                AudioManager.instance.SlasherDeath();
                break;
            default:
                break;
        }
    }

    void Die()

  {
        if (!enabled) return; // Don't run this function if the script is disabled
        GameManager.instance.AddToScore(score);
        GameManager.instance.ShowScorePopup(transform.position, score);

        // Destroy health bar UI when enemy dies
        if (healthBarCanvas != null)
        {
            Destroy(healthBarCanvas.gameObject);
        }

        // play death sound
        PlayDeathSound();

        Destroy(gameObject);
    }
}