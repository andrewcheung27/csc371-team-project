using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 10;
    private int minHealth = 0;

    public void AddToHealth(int n) {
        health += n;

        if (n < minHealth) {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
