using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 2;
    private Vector3 moveDirection;
    private float speed;
    private EnemyHealth enemyHealth;

    public void SetDirection(Vector3 direction, float speed)
    {
        this.moveDirection = direction.normalized;
        this.speed = speed;

        Destroy(gameObject, 3f); // Destroy bullet after 3 seconds
    }

    void Update()
    {
        // move
        transform.position += moveDirection * speed * Time.deltaTime;

        // destroy if game over
        if (!GameManager.instance.GameIsRunning()) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) {
            // the other object or its parent should have an EnemyHealth component
            if (other.gameObject.TryGetComponent(out enemyHealth) || other.gameObject.transform.parent.TryGetComponent(out enemyHealth)) {
                enemyHealth.AddToHealth(-1 * damage);
            }
        }

        // Destroy bullet on impact with an enemy or a wall
        if (other.gameObject.CompareTag("Enemy") 
            || other.gameObject.CompareTag("Wall") 
            || other.gameObject.CompareTag("MovingPlatform")
            || other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
