using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public int damageEasyMode = 1000000;
    public float destroyAfterSeconds = 2f;  // destroy Bullet after this many seconds
    private Vector3 moveDirection;
    private float speed;

    void Awake()
    {
        // Force the bullet's X position to 0
        Vector3 position = transform.position;
        position.x = 0f;  // Ensure the bullet always spawns on X = 0
        transform.position = position;
    }

    public void SetDirection(Vector3 direction, float speed)
    {
        this.moveDirection = direction.normalized;
        this.speed = speed;

        Destroy(gameObject, destroyAfterSeconds);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        if (!GameManager.instance.GameIsRunning()) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = null;
        bool doBlood = true;
        int finalDamage = GameManager.instance.EasyMode() ? damageEasyMode : damage;  // adjust damage if game is on easy mode

        // 1️⃣ Check if we hit a special hitbox (head or body)
        EnemyHitbox hitbox = other.GetComponent<EnemyHitbox>();
        if (hitbox != null)
        {
            enemyHealth = other.GetComponentInParent<EnemyHealth>(); // Find health on parent
            if (hitbox.hitboxType == EnemyHitbox.HitboxType.Head)
            {
                finalDamage *= hitbox.headshotMultiplier; // Apply headshot multiplier

                // show dialogue to make sure the player knows about headshots
                DialogueManager.instance.ShowBossHeadshotDialogue();
            }
            else {
                // if we hit the boss and it wasn't the head, don't show effect
                doBlood = false;
            }
        }
        else
        {
            // 2️⃣ Check if the collider is part of an enemy with health (e.g., claws, body)
            enemyHealth = other.GetComponentInParent<EnemyHealth>();
        }

        // Apply damage if we found an EnemyHealth component
        if (enemyHealth != null)
        {
            enemyHealth.AddToHealth(-finalDamage, doBlood);
            Destroy(gameObject); // Destroy bullet on hit
        }

        // Destroy bullet on impact with solid objects
        if (other.gameObject.CompareTag("Wall") 
            || other.gameObject.CompareTag("MovingPlatform")
            || other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
