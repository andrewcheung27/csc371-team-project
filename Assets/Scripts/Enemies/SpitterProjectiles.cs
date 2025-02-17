using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    public int damage = 1; // Adjusted damage per hit to 1
    public float destroyAfterSeconds = 5f; // Destroy projectile after time
    private bool hasHit = false; // Prevents multiple hits

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds); // Destroy if it doesn't hit anything
    }

    private void OnTriggerEnter(Collider other)
{
    // Check if the projectile hit an object tagged "Player"
    if (!hasHit && other.CompareTag("Player"))
    {
        hasHit = true; // Prevent multiple trigger calls

        // Apply damage using GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.AddToHealth(-damage); // Reduce health
            Debug.Log($"Player hit by SpitterProjectile. New Health: {GameManager.instance.health}");
        }

        // Destroy the projectile immediately
        Destroy(gameObject);
    }
}



    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}


// Having issue where player takes 2 damage instead of 1 with projectiles