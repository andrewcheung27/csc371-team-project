using UnityEngine;

public class SpitterAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign the projectile prefab in the Inspector
    public Transform firePoint; // Where the projectile will be spawned
    public float attackRange = 5f; // Maximum attack distance
    public float attackCooldown = 2f; // Time between attacks
    public float projectileSpeed = 20f; // Speed of the projectile

    private Transform player;
    private float lastAttackTime = 0f;

    void Start()
    {
        // Find the player in the scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Check if player is within attack range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Get direction to the player
        Vector3 direction = (player.position - firePoint.position).normalized;

        // Assign velocity to projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

    }
}
