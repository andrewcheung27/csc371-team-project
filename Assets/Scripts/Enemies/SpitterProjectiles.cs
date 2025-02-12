using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    public int damage = 2; // Damage per hit
    public float destroyAfterSeconds = 5f; // Destroy projectile after time
    private bool hasHit = false; // Prevents multiple hits

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds); // Destroy if it doesn't hit anything
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit && other.CompareTag("Player"))
        {
            hasHit = true; // Prevents multiple trigger calls

            // Disable the Collider so it can't register more triggers
            GetComponent<Collider>().enabled = false;

            // Apply damage
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player hit by SpitterProjectile.");
            }

            // Schedule the projectile to be destroyed very soon
            Invoke(nameof(DestroyProjectile), 0.01f);
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}

// Having issue where player takes 2 damage instead of 1 with projectiles