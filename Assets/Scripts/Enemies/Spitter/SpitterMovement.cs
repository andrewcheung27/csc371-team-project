using UnityEngine;

public class SpitterMovement : MonoBehaviour
{
    public Transform player; // Reference to the player
    public bool canMove = false;
    public float moveSpeed = 3f; // Movement speed
    public float stopDistance = 5f; // Distance at which it stops moving

    void Start()
{
    if (player == null)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("SpitterMovement: No GameObject with tag 'Player' found in scene!");
        }
    }
}


    void Update()
    {
        if (player == null) return;

        if (!canMove) return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Move towards player if not within stop distance
        if (distanceToPlayer > stopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        // Rotate to face the player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }
}
