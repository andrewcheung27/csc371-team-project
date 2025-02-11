using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    public Transform weaponHolder; // Empty GameObject where weapon will be attached
    private Weapon currentWeapon;
    private bool hasWeapon = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (hasWeapon)
        {
            HandleShooting();
            MoveGunAroundPlayer(); 
        }
    }

    public void PickUpWeapon(GameObject weaponPrefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject); // Remove old weapon if applicable
        }

        GameObject weaponInstance = Instantiate(weaponPrefab, weaponHolder.position, Quaternion.identity, weaponHolder);
        currentWeapon = weaponInstance.GetComponent<Weapon>();
        hasWeapon = currentWeapon != null;

        WeaponPickup weaponPickupScript = weaponInstance.GetComponent<WeaponPickup>();
        if (weaponPickupScript != null)
        {
            weaponPickupScript.enabled = false; // Disable rotation and floating
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0)) // Fire when the attack button is pressed
        {
            Vector3 shootDirection = GetShootDirection();
            currentWeapon.Shoot(shootDirection);
        }
    }

    void RotateGunToMouse()
    {
        // Get mouse position in world space and aim along the Z and Y axes
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldMousePosition = ray.GetPoint(distance);
            Vector3 direction = worldMousePosition - transform.position;

            // Rotate the weapon holder to face the mouse position on the Z and Y axes
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            weaponHolder.rotation = Quaternion.Slerp(weaponHolder.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }

    Vector3 GetShootDirection()
    {
        Vector3 direction = Vector3.zero;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0) // Player is moving
        {
            direction = transform.forward * Mathf.Sign(Input.GetAxis("Horizontal"));
        }
        else
        {
            // Get mouse position in world space and aim only along the Z and Y axes
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.right, transform.position); // Plane along X-axis

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldMousePosition = ray.GetPoint(distance);
                
                // Keep X coordinate constant (use player's X position)
                direction = (new Vector3(transform.position.x, worldMousePosition.y, worldMousePosition.z) - transform.position).normalized;

            }
        }
        return direction;
    }

    void MoveGunAroundPlayer()
    {
    float moveDirection = Input.GetAxis("Horizontal"); // Get player's movement direction

    // If player is moving, force the gun to stay in front or behind
    if (Mathf.Abs(moveDirection) > 0.1f) // Threshold to prevent jittering
    {
        float weaponDistance = 1.5f; // Distance from player

        // Move gun directly in front (positive X movement) or behind (negative X movement)
        Vector3 forcedWeaponPosition = transform.position + new Vector3(0, 0, Mathf.Sign(moveDirection) * weaponDistance);
        weaponHolder.position = forcedWeaponPosition;

        // Make weapon face directly forward along Z-axis
        weaponHolder.rotation = Quaternion.LookRotation(Vector3.forward * Mathf.Sign(moveDirection));
    }
    else
    {
        // If stationary, continue normal rotation around player
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.right, transform.position); // Plane along X-axis for vertical movement

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldMousePosition = ray.GetPoint(distance);

            // Get direction from player to mouse
            Vector3 direction = (worldMousePosition - transform.position).normalized;

            // Convert direction to an angle using atan2
            float angle = Mathf.Atan2(direction.y, direction.z); // Full 360-degree motion

            // Convert angle into a point on the circular path
            float yOffset = Mathf.Sin(angle) * 1.5f;
            float zOffset = Mathf.Cos(angle) * 1.5f;

            // Set weaponHolder position along a vertical circle
            weaponHolder.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z + zOffset);

            // Make the weapon always point outward, perpendicular to the circle
            Vector3 aimDirection = (weaponHolder.position - transform.position).normalized;
            weaponHolder.rotation = Quaternion.LookRotation(aimDirection);
        }
    }
    }

}
