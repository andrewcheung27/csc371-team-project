// using UnityEngine;
// using System.Collections;

// public class MiniBossShooting : MonoBehaviour
// {
//     public Transform shootPoint; // Where the projectiles spawn
//     public GameObject projectilePrefab;
//     public float shootCooldown = 10f;
//     public float shootingSpeed = 2f; // Slowed movement speed when shooting
//     public Animator animator;
    
//     private float lastShootTime = 0f;
//     private Vector3 lastKnownPlayerPos;
//     private bool isShooting = false;
//     private MiniBossMovement movement;

//     public bool IsShooting => isShooting;

//     void Start()
//     {
//         movement = GetComponent<MiniBossMovement>();
//     }

//     public bool CanShoot(float distanceToPlayer)
//     {
//         return Time.time - lastShootTime >= shootCooldown && distanceToPlayer >= 10f && distanceToPlayer <= 20f;
//     }

//     public void StartShooting(Vector3 playerPosition)
//     {
//         if (isShooting) return;
//         StartCoroutine(ShootSequence(playerPosition));
//     }

//     IEnumerator ShootSequence(Vector3 playerPosition)
//     {
//         isShooting = true;
//         movement.agent.speed = shootingSpeed; // Slow down when shooting
//         lastShootTime = Time.time;
//         lastKnownPlayerPos = playerPosition; // Store the last known position

//         for (int i = 0; i < 3; i++)
//         {
//             float delay = Random.Range(0.5f, 2f);
//             yield return new WaitForSeconds(delay);

//             float animSpeed = (delay < 1f) ? 1.5f : 1f;
//             animator.SetFloat("ShootSpeed", animSpeed);
//             animator.SetTrigger("Shoot");

//             FireProjectile();
//         }

//         yield return new WaitForSeconds(1.66f); // Wait for shooting animation to end
//         movement.agent.speed = movement.normalSpeed; // Resume normal speed
//         isShooting = false;
//     }

//     void FireProjectile()
//     {
//         GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
//         Vector3 direction = (lastKnownPlayerPos - shootPoint.position).normalized;
//         projectile.GetComponent<Rigidbody>().linearVelocity = direction * 10f; // Adjust speed as needed
//     }
// }

using UnityEngine;
using System.Collections;

public class MiniBossShooting : MonoBehaviour
{
    public Transform shootPoint; // Where the projectiles spawn
    public GameObject projectilePrefab;
    public float shootCooldown = 10f;
    public float shootingSpeed = 2f; // Slowed movement speed when shooting
    public Animator animator;
    
    private float lastShootTime = 0f;
    private Vector3 lastKnownPlayerPos;
    private bool isShooting = false;
    private MiniBossMovement movement;
    
    // Reference to player to track their movement
    public Transform player;  // Add a public reference to the player

    public bool IsShooting => isShooting;

    void Start()
    {
        movement = GetComponent<MiniBossMovement>();
    }

    public bool CanShoot(float distanceToPlayer)
    {
        return Time.time - lastShootTime >= shootCooldown && distanceToPlayer >= 10f && distanceToPlayer <= 20f;
    }

    public void StartShooting(Vector3 playerPosition)
    {
        if (isShooting) return;
        StartCoroutine(ShootSequence(playerPosition));
    }

    IEnumerator ShootSequence(Vector3 playerPosition)
    {
        isShooting = true;
        movement.agent.speed = shootingSpeed; // Slow down when shooting
        lastShootTime = Time.time;
        lastKnownPlayerPos = playerPosition; // Store the last known position

        for (int i = 0; i < 3; i++)
        {
            float delay = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(delay);

            float animSpeed = (delay < 1f) ? 1.5f : 1f;
            animator.SetFloat("ShootSpeed", animSpeed);
            animator.SetTrigger("Shoot");

            FireProjectile(); // Fire a projectile
        }

        yield return new WaitForSeconds(1.66f); // Wait for shooting animation to end
        movement.agent.speed = movement.normalSpeed; // Resume normal speed
        isShooting = false;
    }

    void FireProjectile()
    {
        // Get the player's velocity to predict their movement
        if (player == null) return; // Ensure player reference exists

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // Predict where the player will be in 0.5 seconds (tweak this time as needed)
            Vector3 predictedPosition = player.position + playerRb.linearVelocity * 0.5f;

            // Instantiate projectile at the shooting point
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calculate direction to predicted position
                Vector3 direction = (predictedPosition - shootPoint.position).normalized;
                rb.linearVelocity = direction * 10f; // Adjust speed as needed
            }
        }
    }
}

