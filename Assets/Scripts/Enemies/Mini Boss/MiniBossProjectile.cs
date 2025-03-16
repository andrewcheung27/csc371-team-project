using UnityEngine;

public class MiniBossProjectile : MonoBehaviour
{
    public int damage = 2;
    public float destroyAfterSeconds = 10f; // Destroy projectile after time

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds); // Destroy if it doesn't hit anything
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hit an object tagged "Player"
        if (other.CompareTag("Player"))
        {
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
