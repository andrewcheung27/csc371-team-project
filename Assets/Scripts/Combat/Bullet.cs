using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 2;
    private Vector3 moveDirection;
    private float speed;

    public void SetDirection(Vector3 direction, float speed)
    {
        this.moveDirection = direction.normalized;
        this.speed = speed;

        Destroy(gameObject, 3f); // Destroy bullet after 3 seconds
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) {
            if (other.gameObject.TryGetComponent(out EnemyHealth enemyHealth)) {
                enemyHealth.AddToHealth(-1 * damage);
            }
        }

        // Destroy bullet on impact with an enemy or a wall
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
