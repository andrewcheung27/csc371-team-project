using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float distanceFromPlayerX = 20f;  // Fixed distance on the X-axis
    public float heightAbovePlayer = 10f;  // Height above the player (Y-axis)
    public float smoothing = 0f;  // Smoothness of the camera movement
    public float angle = 15f;  // Downward angle in degrees

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        if (player == null) {
            return;
        }

        // Calculate the target position to follow the player along the Z-axis, fixed on X and Y
        float targetX = player.position.x + distanceFromPlayerX;
        float targetY = player.position.y + heightAbovePlayer;  // Slightly above the player

        // The Z position is the same as the player's position (no offset)
        Vector3 targetPosition = new Vector3(targetX, targetY, player.position.z);

        // Apply a slight downward rotation (15 degrees)
        Quaternion targetRotation = Quaternion.Euler(angle, -90, 0f); // 15-degree downward angle
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothing);

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);
    }
}
