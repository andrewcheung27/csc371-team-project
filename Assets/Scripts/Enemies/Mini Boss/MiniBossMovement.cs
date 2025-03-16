using UnityEngine;
using UnityEngine.AI;

public class MiniBossMovement : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 30f;
    public float stoppingDistance = 1.5f;
    public float normalSpeed = 3.5f;
    public Animator animator;
    public UnityEngine.AI.NavMeshAgent agent;
    public MiniBossShooting shooter;

    private bool isHit = false;
    bool isAttacking = false;
    private float hitCooldownTime = 3f;  // Cooldown time in seconds (adjust as needed)
    private float lastHitTime = 0f;  // Keeps track of the last time the animation was triggered

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = normalSpeed;
        shooter = GetComponent<MiniBossShooting>();
    }

    void Update()
    {
        // Don't allow movement if the hit animation is playing or during cooldown
        if (isHit) return;  

        // don't move while attacking
        if (isAttacking) {
            agent.isStopped = true;
            return;
        }
        else {
            agent.isStopped = false;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (!shooter.IsShooting)
        {
            if (distanceToPlayer <= detectionRange)
            {
                agent.SetDestination(player.position);
            }
        }

        if(!shooter.IsShooting && shooter.CanShoot(distanceToPlayer))
        {
            shooter.StartShooting(player.position);
        }




        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);  // Set the walking animation based on speed
    }

    public void SetIsAttacking(bool b)
    {
        isAttacking = b;
    }

    // public void TakeDamage()
    // {
    //     // Ensure cooldown is respected
    //     if (Time.time - lastHitTime < hitCooldownTime) return; // Don't trigger animation again until cooldown

    //     isHit = true;
    //     agent.isStopped = true;  // Stop movement during hit animation

    //     // Trigger appropriate hit animation based on whether the boss is moving or idle
    //     if (animator.GetFloat("Speed") > 0.1f)  // If moving
    //     {
    //         animator.SetTrigger("WalkHit");  // Trigger walking hit animation
    //     }
    //     else  // If idle
    //     {
    //         animator.SetTrigger("Hit");  // Trigger idle hit animation
    //     }

    //     lastHitTime = Time.time;  // Update the last hit time

    //     // Start cooldown to allow movement and prevent further hits from triggering animation
    //     Invoke("ResetHit", 1f); // 1 second before the boss can move again after animation
    // }

    public void TakeDamage()
    {
        // Ensure cooldown is respected
        if (Time.time - lastHitTime < hitCooldownTime) return; 

        isHit = true;
        agent.isStopped = true;  // Stop movement immediately

        animator.SetTrigger("Hit");  // Always play the same hit animation

        lastHitTime = Time.time;  // Update cooldown time

        // Resume movement after 1 second
        Invoke("ResetHit", 1f);
    }

    void ResetHit()
    {
        isHit = false;
        agent.isStopped = false;  // Allow movement again after hit animation
    }
}
