using UnityEngine;
using UnityEngine.AI;

public class MiniBossMovement : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 30f;
    public float stoppingDistance = 1.5f;
    public float normalSpeed = 3.5f;
    float normalSpeedOrig;
    public float zBoundaryMin = 0f;
    public float zBoundaryMax = 1000f;
    public Animator animator;
    public NavMeshAgent agent;
    public MiniBossShooting shooter;

    private bool isHit = false;
    bool isAttacking = false;

    void Start()
    {
        normalSpeedOrig = normalSpeed;

        agent = GetComponent<NavMeshAgent>();
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
                Vector3 pos = new Vector3(player.position.x, player.position.y, Mathf.Clamp(player.position.z, zBoundaryMin, zBoundaryMax));
                agent.SetDestination(pos);
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

    public void MultiplyOriginalNormalSpeed(float s)
    {
        normalSpeed = normalSpeedOrig * s;
        agent.speed = normalSpeed;
    }

    // this function was previously used to make the enemy sometimes play an animation when hit. 
        // now, we've changed it to flinch when it reaches a new phase.
    public void TakeDamage()
    {
        return;
    }

    public void Flinch()
    {
        isHit = true;
        agent.isStopped = true;  // Stop movement immediately
        shooter.StopShooting();

        animator.SetTrigger("Hit");  // Always play the same hit animation

        // Resume movement after 1 second
        Invoke("ResetHit", 1f);
    }

    void ResetHit()
    {
        isHit = false;
        agent.isStopped = false;  // Allow movement again after hit animation
    }

    public bool ReachedDestination()
    {
        // https://discussions.unity.com/t/how-can-i-tell-when-a-navmeshagent-has-reached-its-destination/52403/5
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
