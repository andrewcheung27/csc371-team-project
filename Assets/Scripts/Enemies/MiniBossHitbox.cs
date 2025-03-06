using UnityEngine;

public class MinibossHitbox : MonoBehaviour
{
    public int damage = 3; // Damage per hit

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected for Miniboss!");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            GameManager.instance.AddToHealth(-damage); // Apply damage
        }
    }
}
