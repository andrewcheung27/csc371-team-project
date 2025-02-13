using UnityEngine;
using System.Collections;

public class HunterMovement : MonoBehaviour
{
    public GameObject player; // Reference to the player
    public float moveSpeed = 2f; // Normal movement speed
    public float dashSpeed = 20f; // Speed during dash
    public float dashDuration = .5f; // How long the dash lasts
    public float dashCooldown = 2f; // Time between dashes
    public float visionRange = 20f;  // how close the player must be to move towards the player

    private bool isDashing = false;

    void Start()
    {
        // Find the player in the scene (assuming the player has the tag "Player")
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Start the dash loop
        StartCoroutine(DashRoutine());
    }

    void Update()
    {
        if (player == null) return;

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
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

        Vector3 direction = hunterToPlayer.normalized;
        float currentSpeed = isDashing ? dashSpeed : moveSpeed;
        transform.position += direction * currentSpeed * Time.deltaTime;

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
}
