using UnityEngine;

// destroys player in ending scene
public class PlayerDestroyer : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player")) {
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);
        }
    }
}
