using UnityEngine;

public class HunterAttack : MonoBehaviour
{
    public int damage = 1; // Damage per hit
    public float attackCooldown = 1.5f; // Time between attacks
    public float movePauseAfterAttack = 1.0f;

    private float lastAttackTime = 0f;

    private void OnCollisionEnter(Collision collision) // Attacks when first colliding with the player
    {
        if (collision.gameObject.CompareTag("Player")  && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    private void OnCollisionStay(Collision collision) // Continues attacking every attackCooldown seconds
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // Apply damage using GameManager
        if (GameManager.instance != null)
        {
            // Reduce player health
            GameManager.instance.AddToHealth(-damage);

            // pause movement after attacking
            if (TryGetComponent(out HunterMovement hunterMovement)) {
                hunterMovement.PauseMovement(movePauseAfterAttack);
            }
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
        AudioManager.instance.HunterAttack();
    }
}


// Make sure the enemy has a Collider with "Is Trigger" **disabled** (solid Collider)
// Ensure the player has a Collider with "Is Trigger" **disabled**
// The player must have the tag "Player" and a PlayerHealth script with the TakeDamage() function
// At least one object (enemy or player) must have a Rigidbody for collisions to work
