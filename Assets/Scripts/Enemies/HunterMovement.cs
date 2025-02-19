using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class HunterMovement : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;

    [Header("Movement")]
    public float moveSpeed;
    public float dashSpeed = 20f;
    public float dashDuration = .5f;
    public float dashCooldown = 2f;
    private bool isDashing = false;
    private bool canMove = true;

    [Header("Other Options")]
    public float visionRange = 20f;
    public float zBoundaryMin = Mathf.NegativeInfinity;
    public float zBoundaryMax = Mathf.Infinity;
    public float yBoundaryMin = Mathf.NegativeInfinity;
    public float yBoundaryMax = Mathf.Infinity;

    void Awake()
    {
        if (zBoundaryMin > zBoundaryMax || yBoundaryMin > yBoundaryMax)
        {
            throw new Exception("Boundary min values must not be greater than max values");
        }
    }

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;

        StartCoroutine(DashRoutine());
    }

    void Update()
    {
        if (player == null || !canMove) return;

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (!(zBoundaryMin <= player.transform.position.z && player.transform.position.z <= zBoundaryMax
            && yBoundaryMin <= player.transform.position.y && player.transform.position.y <= yBoundaryMax))
        {
            navMeshAgent.ResetPath();
            return;
        }

        Vector3 hunterToPlayer = player.transform.position - transform.position;
        if (!Physics.Raycast(transform.position, hunterToPlayer.normalized, out RaycastHit hit, visionRange))
        {
            navMeshAgent.ResetPath();
            return;
        }

        if (hit.collider.gameObject != player)
        {
            navMeshAgent.ResetPath();
            return;
        }

        navMeshAgent.SetDestination(player.transform.position);

        if (isDashing)
        {
            navMeshAgent.speed = dashSpeed;
        }
        else
        {
            navMeshAgent.speed = moveSpeed;
        }
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

    IEnumerator PauseMovementRoutine(float seconds)
    {
        canMove = false;
        navMeshAgent.ResetPath();
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    public void PauseMovement(float seconds)
    {
        StartCoroutine(PauseMovementRoutine(seconds));
    }
}