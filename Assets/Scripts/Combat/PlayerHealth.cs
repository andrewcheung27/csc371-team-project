using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10; // Maximum health
    private int currentHealth; // Current health

    // public HealthBar healthBar; // UI Health Bar (add later)
    public GameObject deathEffect;

    void Start()
    {
        currentHealth = maxHealth;
        // if (healthBar != null)
        // {
        //     healthBar.SetMaxHealth(maxHealth);
        // }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Health: " + currentHealth);

        // if (healthBar != null)
        // {
        //     healthBar.SetHealth(currentHealth);
        // }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Don't exceed max health
        }

        Debug.Log("Player healed by " + amount + ". Current Health: " + currentHealth);

        // if (healthBar != null)
        // {
        //     healthBar.SetHealth(currentHealth);
        // }
    }

    void Die()
    {
        Debug.Log("Player died!");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false); // Disable player object
    }
}
