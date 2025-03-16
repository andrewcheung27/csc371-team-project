using UnityEngine;

public class MiniBossCollision : MonoBehaviour
{
    public int damage = 1;

    // This method is called when the miniboss collides with something
    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return; // Skip if the script is disabled
        // Check if the object the miniboss collided with has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage using the GameManager (assuming GameManager has a method to deal damage)
            if (GameManager.instance != null)
            {
                GameManager.instance.AddToHealth(-damage);
            }

            // // Optionally, you could apply some force to the player or other effects
            // // For example, you can add a small knockback effect if you want:
            // Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            // if (playerRigidbody != null)
            // {
            //     // Apply a knockback force (optional)
            //     Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
            //     playerRigidbody.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
            // }
        }
    }
}
