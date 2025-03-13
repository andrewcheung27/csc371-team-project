using UnityEngine;

public class SpitterAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign the projectile prefab in the Inspector
    public Transform firePoint; // Where the projectile will be spawned
    public bool shootAtPlayer = false;  // whether to shoot at player or shoot at attackDirection
    public Vector3 attackDirection;
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

        // extremely terrible code to make it face in the attack direction
        if (attackDirection.x > 0.1) {
            transform.LookAt(transform.position + new Vector3(-1f, 0f, 0f));
        }
        else if (attackDirection.x < -0.1) {
            transform.LookAt(transform.position + new Vector3(1f, 0f, 0f));
        }
        else if (attackDirection.z > 0.1) {
            transform.LookAt(transform.position + new Vector3(0f, 0f, -1f));
        }
        else if (attackDirection.z < -0.1) {
            transform.LookAt(transform.position + new Vector3(0f, 0f, 1f));
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

        // Get direction to attack
        Vector3 currentAttackDirection;
        if (shootAtPlayer) {
            currentAttackDirection = (player.position - firePoint.position).normalized;
        }
        else {
            currentAttackDirection = attackDirection;
        }

        // Assign velocity to projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = currentAttackDirection * projectileSpeed;
        }
    }
}
