
using UnityEngine;
using System.Collections;

public class MiniBossShooting : MonoBehaviour
{
    public Transform shootPoint; // Where the projectiles spawn
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;
    public int shotsPerLoad = 3;
    public float shootCooldown = 10f;
    public float shootCooldownWhenDestinationReached = 4f;
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
        float cooldown = movement.ReachedDestination() ? shootCooldownWhenDestinationReached : shootCooldown;
        return Time.time - lastShootTime >= cooldown && distanceToPlayer >= 10f && distanceToPlayer <= 20f;
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

        float shootInterval = 1.0f; // Fixed time between each shot

        for (int i = 0; i < shotsPerLoad; i++)
        {
            // stop if no longer shooting
            if (!isShooting) {
                yield break;
            }

            // Trigger the shoot animation
            animator.SetFloat("ShootSpeed", 1f);
            animator.SetTrigger("Shoot");

            // Fire the projectile
            FireProjectile();

            // Wait for the interval (accounting for the animation time)
            yield return new WaitForSeconds(shootInterval);
        }

        // Ensure the shooting animation has fully finished before resuming normal speed
        yield return new WaitForSeconds(1.66f - (shootInterval * shotsPerLoad));

        movement.agent.speed = movement.normalSpeed; // Resume normal speed
        isShooting = false;
    }

    void FireProjectile()
    {
        if (!enabled) return; // Skip if the script is disabled
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
                rb.linearVelocity = direction * projectileSpeed; // Adjust speed as needed
            }
        }
    }

    public void AddShotsPerLoad(int n)
    {
        shotsPerLoad += n;
    }

    public void StopShooting()
    {
        isShooting = false;
    }
}
