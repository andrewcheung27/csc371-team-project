using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MiniBossDeath : MonoBehaviour
{
    public Transform deathTransform; // The place where the boss dies
    public Animator animator;
    public NavMeshAgent agent;
    public MiniBossMovement movement;
    public MiniBossShooting shooter;
    public EnemyHealth healthScript;

    private bool isDying = false;
    private bool hasPlayedDeathAnimation = false;
    public float deathSpeed = 2f; // Slow walking speed to death spot

    void Update()
    {
        // If the boss is dying, force it to walk towards the death transform
        if (isDying && !hasPlayedDeathAnimation)
        {
            agent.SetDestination(deathTransform.position);
            agent.speed = deathSpeed;

            // Check if the boss has reached the death transform
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                PlayDeathAnimation();
            }
        }
    }

    public void TriggerDeath()
    {
        if (isDying) return;
        isDying = true;

        // Stop any movement/shooting logic
        movement.enabled = false;
        shooter.enabled = false;
        agent.isStopped = false;
        agent.speed = deathSpeed;

        // Start walking towards the death transform
        agent.SetDestination(deathTransform.position);
    }

    void PlayDeathAnimation()
    {
        if (hasPlayedDeathAnimation) return;
        hasPlayedDeathAnimation = true;

        // Trigger the death animation
        animator.SetTrigger("Death");

        // Completely stop the NavMeshAgent
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.enabled = false;

        // Keep the body in the final frame of the death animation
        StartCoroutine(FreezeBodyOnDeath());
    }

    IEnumerator FreezeBodyOnDeath()
    {
        // Wait for the death animation to complete (adjust to your animation length)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Set the Animator to not transition anymore and freeze the last frame
        animator.enabled = false;
    }
}

