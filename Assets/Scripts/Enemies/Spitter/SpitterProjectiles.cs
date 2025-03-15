using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    public int damage = 1; // Adjusted damage per hit to 1
    public float destroyAfterSeconds = 5f; // Destroy projectile after time

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds); // Destroy if it doesn't hit anything
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hit an object tagged "Player"
        if (other.CompareTag("Player"))
        {
            // player is immune to projectiles while dashing
            if (other.TryGetComponent(out PlayerMovement playerMovement) && playerMovement.IsDashing()) {
                Destroy(gameObject);
                return;
            }

            // Apply damage using GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.AddToHealth(-damage); // Reduce health
            }

            // Destroy the projectile immediately
            Destroy(gameObject);
        }

        // Destroy on impact with a wall, platform, or ground
        else if (other.gameObject.CompareTag("Wall") 
            || other.gameObject.CompareTag("MovingPlatform")
            || other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
