using UnityEngine;

public class HunterAttack : MonoBehaviour
{
    public int damage = 1; // Damage per hit
    public float attackCooldown = 1.5f; // Time between attacks

    private float lastAttackTime = 0f;

    private void OnCollisionEnter(Collision collision) // Attacks when first colliding with the player
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Attack(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision) // Continues attacking every attackCooldown seconds
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack(collision.gameObject);
        }
    }

    void Attack(GameObject player)
    {
        lastAttackTime = Time.time;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        
        }
        else
        {
            Debug.LogError("No PlayerHealth script found on player!");
        }
    }

}

// Make sure the enemy has a Collider with "Is Trigger" **disabled** (solid Collider)
// Ensure the player has a Collider with "Is Trigger" **disabled**
// The player must have the tag "Player" and a PlayerHealth script with the TakeDamage() function
// At least one object (enemy or player) must have a Rigidbody for collisions to work
