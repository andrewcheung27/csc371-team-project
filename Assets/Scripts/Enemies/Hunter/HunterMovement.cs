
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HunterMovement : MonoBehaviour
{
    public float raycastDistance = 20f;
    public float followSpeed = 3.5f;
    public float normalAcceleration = 8f;
    public float dashSpeed = 10f;
    public float dashAcceleration = 50f; 
    public float dashChargeTime = 2f;

    public float dashMinRange = 5f;
    public float dashMaxRange = 10f;
    public float dashCooldown = 5f;
    public float howFarToDashPastPlayer = 3f;
    public float stuckCheckTime = 1f;
    public float stuckThreshold = 0.1f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isPaused = false;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 dashTargetPosition;
    private Renderer hunterRenderer;
    private Color normalColor;
    private Color darkColor;
    public bool disableMovement = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = followSpeed;
        agent.acceleration = normalAcceleration; // Set default acceleration
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hunterRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        normalColor = hunterRenderer.material.color;
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
        if (!isPaused && !isDashing)
        {
            FollowPlayerWithSphereCast();

            float playerDistance = Vector3.Distance(transform.position, player.position);

            if (canDash && playerDistance >= dashMinRange && playerDistance <= dashMaxRange)
            {
                StartCoroutine(DashAtPlayer());
            }
        }
    
    }

    void FollowPlayerWithSphereCast()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 sphereCastOrigin = transform.position + Vector3.up * 1.0f;

        bool playerDetected = Physics.SphereCast(sphereCastOrigin, 1f, directionToPlayer, out hit, raycastDistance);

        if (playerDetected && hit.collider.CompareTag("Player"))
        {
            agent.SetDestination(player.position);
            return;
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

    IEnumerator DashAtPlayer()
    {
        canDash = false;
        isDashing = true;
        agent.isStopped = true;

        darkColor = new Color(0.5f, 0f, 0f);

        // Gradually darken the hunter while charging
        yield return StartCoroutine(ChangeColorOverTime(normalColor, darkColor, dashChargeTime));

        // try to dash a bit past the player
        Vector3 pastPlayerPosition = player.position + new Vector3(0f, 0f, transform.position.z > player.position.z ? -howFarToDashPastPlayer : howFarToDashPastPlayer);
        if (NavMesh.SamplePosition(pastPlayerPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
            dashTargetPosition = hit.position;
        }
        // if we can't dash past the player, get a valid NavMesh position where the player currently is
        else if (NavMesh.SamplePosition(player.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            dashTargetPosition = hit.position;
        }
        else
        {
            isDashing = false;
            canDash = true;
            agent.isStopped = false;
            yield return StartCoroutine(ChangeColorOverTime(hunterRenderer.material.color, normalColor, 0.5f));
            yield break;
        }

        // Start dashing
        agent.speed = dashSpeed;
        agent.acceleration = dashAcceleration; // Increase acceleration for quick dash start
        agent.isStopped = false;
        agent.SetDestination(dashTargetPosition);

        // Check if the agent gets stuck
        Vector3 startPosition = transform.position;
        yield return new WaitForSeconds(stuckCheckTime);
        if (Vector3.Distance(transform.position, startPosition) < stuckThreshold)
        {
            isDashing = false;
            agent.speed = followSpeed;
            agent.acceleration = normalAcceleration; // Reset acceleration
            agent.isStopped = false;
            yield return StartCoroutine(ChangeColorOverTime(hunterRenderer.material.color, normalColor, 0.5f));

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;

            yield break;
        }

        // Wait until reaching the target or getting stuck
        while (!agent.pathPending && agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        // Reset after dash
        agent.speed = followSpeed;
        agent.acceleration = normalAcceleration; // Reset acceleration
        isDashing = false;

        // Gradually return to normal color
        yield return StartCoroutine(ChangeColorOverTime(hunterRenderer.material.color, normalColor, 0.5f));

        // Cooldown before next dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator ChangeColorOverTime(Color fromColor, Color toColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            hunterRenderer.material.color = Color.Lerp(fromColor, toColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hunterRenderer.material.color = toColor;
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
