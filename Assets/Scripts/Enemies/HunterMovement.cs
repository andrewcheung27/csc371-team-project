using UnityEngine;
using System.Collections;

public class HunterMovement : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float moveSpeed = 2f; // Normal movement speed
    public float dashSpeed = 20f; // Speed during dash
    public float dashDuration = .5f; // How long the dash lasts
    public float dashCooldown = 2f; // Time between dashes

    private bool isDashing = false;

    void Start()
    {
        // Find the player in the scene (assuming the player has the tag "Player")
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
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
        Vector3 direction = (player.position - transform.position).normalized;
        float currentSpeed = isDashing ? dashSpeed : moveSpeed;
        transform.position += direction * currentSpeed * Time.deltaTime;

        // Rotate to face the player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
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
