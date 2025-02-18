using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HunterMovement : MonoBehaviour
{
    public GameObject player; // Reference to the player
    public float moveSpeed = 3.5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;
    public float visionRange = 20f;

    private bool isDashing = false;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = 0f;

        StartCoroutine(DashRoutine());
    }

    void Update()
    {
        if (player == null) return;

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check if player is within vision range
        if (distanceToPlayer > visionRange)
        {
            navMeshAgent.ResetPath();
            return;
        }

        // Perform raycast to check line of sight
        if (Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer.normalized, out RaycastHit hit, visionRange))
        {
            // Check if the ray hits the player
            if (hit.collider.gameObject != player)
            {
                // Something is blocking vision (e.g., a wall), so stop pursuing
                navMeshAgent.ResetPath();
                return;
            }
        }
        else
        {
            // If nothing was hit, treat it as a blocked path (optional safeguard)
            navMeshAgent.ResetPath();
            return;
        }

        // If vision is clear, pursue the player
        navMeshAgent.SetDestination(player.transform.position);

        // Adjust speed if dashing
        navMeshAgent.speed = isDashing ? dashSpeed : moveSpeed;
    }

    IEnumerator DashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashCooldown);
            isDashing = true;
            yield return new WaitForSeconds(dashDuration);
            isDashing = false;
        }
    }
}
