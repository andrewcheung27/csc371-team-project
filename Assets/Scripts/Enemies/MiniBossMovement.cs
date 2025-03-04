using UnityEngine;
using UnityEngine.AI;

public class MiniBossMovement : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 30f;
    public float stoppingDistance = 1.5f;
    public Animator animator;
    public UnityEngine.AI.NavMeshAgent agent;
    
    private bool isHit = false;
    
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        if (isHit) return; // Prevent movement while hit animation is playing

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);
            
        }

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    
    }

    public void TakeDamage()
    {
        isHit = true;
        agent.isStopped = true;
        
        if (animator.GetBool("isWalking"))
        {
            animator.SetTrigger("hitWhileWalking");
        }
        else
        {
            animator.SetTrigger("hitWhileIdle");
        }
        
        Invoke("ResetHit", 0.5f); // Delay before allowing movement again
    }

    void ResetHit()
    {
        isHit = false;
        agent.isStopped = false;
    }
}
