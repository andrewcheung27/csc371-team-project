using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header ("Health")]
    public int health = 10;
    private int minHealth = 0;

    [Header ("Score")]
    public int score = 100;  // how many points the player gets for killing this enemy

    public void AddToHealth(int n) {
        health += n;

        if (health <= minHealth) {
            Die();
        }
    }

    void Die()
    {
        GameManager.instance.AddToScore(score);
        GameManager.instance.ShowScorePopup(transform.position, score);

        Destroy(gameObject);
    }
}
