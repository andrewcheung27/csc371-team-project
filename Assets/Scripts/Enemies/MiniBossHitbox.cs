using UnityEngine;

public class MinibossHitbox : MonoBehaviour
{
    public int damage = 3; // Damage per hit

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.AddToHealth(-damage); // Apply damage
        }
    }
}
