using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;

    public void SetDirection(Vector3 direction, float speed)
    {
        this.moveDirection = direction.normalized;
        this.speed = speed;

        Destroy(gameObject, 3f); // Destroy bullet after 2 seconds
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject); // Destroy bullet on collision
    }
}

