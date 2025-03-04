using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SlasherMovement : MonoBehaviour
{
    public float raycastDistance = 20f;
    public float followSpeed = 3.5f;
    public float normalAcceleration = 8f;

    public float stuckCheckTime = 1f;
    public float stuckThreshold = 0.1f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isPaused = false;
    public bool disableMovement = false;
    private SlasherAnimator slasherAnimator;

    void Awake()
    {
        slasherAnimator = GetComponent<SlasherAnimator>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = followSpeed;
        agent.acceleration = normalAcceleration; // Set default acceleration
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (disableMovement)
        {
            agent.isStopped = true;
        }
    }

    void Update()
    {
        if (disableMovement)
        {
            agent.isStopped = true;
            return;
        }

        if (slasherAnimator.InAttackAnimation()) {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        if (!isPaused)
        {
            FollowPlayerWithSphereCast();
        }
    }

    void FollowPlayerWithSphereCast()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 sphereCastOrigin = transform.position + Vector3.up * 1.0f;

        RaycastHit[] hits = Physics.SphereCastAll(sphereCastOrigin, 1f, directionToPlayer, raycastDistance);

        foreach (RaycastHit hit in hits) {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(player.position);

                // animate walking
                if (!slasherAnimator.InWalkAnimation()) {
                    if (player.position.z < transform.position.z) {
                        slasherAnimator.WalkLeft();
                    }
                    else {
                        slasherAnimator.WalkRight();
                    }
                }

                return;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= raycastDistance)
        {
            float playerHeightDiff = Mathf.Abs(player.position.y - transform.position.y);
            if (playerHeightDiff < 5f)
            {
                agent.SetDestination(player.position);
                return;
            }
        }

        agent.ResetPath();
    }

    public void PauseMovement(float pauseTime)
    {
        if (!isPaused)
        {
            StartCoroutine(PauseCoroutine(pauseTime));
        }
    }

    private IEnumerator PauseCoroutine(float pauseTime)
    {
        isPaused = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(pauseTime);
        agent.isStopped = false;
        isPaused = false;
    }

}
