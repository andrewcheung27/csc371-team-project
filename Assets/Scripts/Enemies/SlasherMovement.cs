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
    private GameObject player;
    private bool isPaused = false;
    public bool disableMovement = false;
    private SlasherAnimator slasherAnimator;
    public float stopMovingHeightDiff = 10f;  // stop moving if the y-coord difference between slasher and the player is greater than this
    LayerMask layerMask;

    void Awake()
    {
        slasherAnimator = GetComponent<SlasherAnimator>();

        // only look for raycast hits on these layers
        layerMask = LayerMask.GetMask("Player", "NavigationLayer");
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = followSpeed;
        agent.acceleration = normalAcceleration; // Set default acceleration
        player = GameObject.FindGameObjectWithTag("Player");
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
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        float playerHeightDiff = Mathf.Abs(player.transform.position.y - transform.position.y);
        if (playerHeightDiff > stopMovingHeightDiff) {
            return;
        }

        Vector3 rayCastOrigin = transform.position + new Vector3(0, 6f, 0);
        Vector3 directionToPlayer = (player.transform.position - rayCastOrigin).normalized;

        if (Physics.Raycast(rayCastOrigin, directionToPlayer, out RaycastHit hit, raycastDistance, layerMask)) {
            // Debug.Log("hit: " + hit.collider.gameObject.name);
            // Debug.DrawLine(rayCastOrigin, hit.point, Color.red, 1f);
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(player.transform.position);
                slasherAnimator.Walk();
                return;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= raycastDistance)
        {
            if (playerHeightDiff < 5f)
            {
                agent.SetDestination(player.transform.position);
                slasherAnimator.Walk();
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
