using UnityEngine;
using System.Collections;
using System;

public class HunterMovement : MonoBehaviour
{
    public GameObject player; // Reference to the player
    private Rigidbody rb;

    [Header ("Movement")]
    public float moveSpeed = 2f; // Normal movement speed
    public float dashSpeed = 20f; // Speed during dash
    public float dashDuration = .5f; // How long the dash lasts
    public float dashCooldown = 2f; // Time between dashes
    private bool isDashing = false;
    private bool canMove = true;

    [Header ("Other Options")]
    public float visionRange = 20f;  // how close the player must be to move towards the player
    // can't move if player is beyond these boundaries on z axis
    public float zBoundaryMin = Mathf.NegativeInfinity;
    public float zBoundaryMax = Mathf.Infinity;
    // can't move if player is beyond these boundaries on y axis
    public float yBoundaryMin = Mathf.NegativeInfinity;
    public float yBoundaryMax = Mathf.Infinity;

    void Awake()
    {
        if (zBoundaryMin > zBoundaryMax || yBoundaryMin > yBoundaryMax) {
            throw new Exception("Boundary min values must not be greater than max values");
        }
    }

    void Start()
    {
        // Find the player in the scene (assuming the player has the tag "Player")
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        rb = GetComponent<Rigidbody>();

        // Start the dash loop
        StartCoroutine(DashRoutine());
    }

    void FixedUpdate()
    {
        if (player == null) return;

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (!canMove) {
            return;
        }

        // must be within movement boundaries
        if (!(zBoundaryMin <= player.transform.position.z 
            && player.transform.position.z <= zBoundaryMax 
            && yBoundaryMin <= player.transform.position.y
            && player.transform.position.y <= yBoundaryMax)) {
                return;
        }

        Vector3 hunterToPlayer = player.transform.position - transform.position;

        // raycast towards player
        if (!Physics.Raycast(transform.position, hunterToPlayer, out RaycastHit hit, visionRange)) {
            return;
        }

        // don't move if raycast hits a wall
        // TODO: maybe need to change this to checking if raycast doesn't hit Player or Gun
        if (hit.collider.gameObject.CompareTag("Wall")) {
            return;
        }

        // move towards player
        Vector3 direction = hunterToPlayer.normalized;
        float yVelocity = rb.linearVelocity.y;  // save y velocity so we don't change it
        rb.linearVelocity = new Vector3(direction.x, 0f, direction.z) * moveSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, yVelocity, rb.linearVelocity.z);

        // do dash ability by adding force
        if (isDashing) {
            rb.AddForce(new Vector3(direction.x, 0f, direction.z) * dashSpeed);
        }

        // Rotate to face the player
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
    }

    IEnumerator DashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashCooldown); // Wait before dashing
            isDashing = true;
            yield return new WaitForSeconds(dashDuration); // Dash duration
            isDashing = false;
        }
    }

    // pause movement for some number of seconds
    IEnumerator PauseMovementRoutine(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    // public method for PauseMovementRoutine()
    public void PauseMovement(float seconds) {
        StartCoroutine(PauseMovementRoutine(seconds));
    }
}
