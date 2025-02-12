
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    public Transform weaponHolder; // Empty GameObject where weapon will be attached
    private Weapon currentWeapon;
    private bool hasWeapon = false;
    private bool isAiming = false;
    private Vector3 lastMoveDirection = Vector3.forward; // Direction player last moved in
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (hasWeapon)
        {
            HandleAiming();
            HandleShooting();
            MoveGunAroundPlayer();
        }

        CheckAimingInput();
    }

    public void PickUpWeapon(GameObject weaponPrefab)
    {
        // Remove previous weapon if applicable
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        // Instantiate new weapon and assign it to the weapon holder
        GameObject weaponInstance = Instantiate(weaponPrefab, weaponHolder.position, Quaternion.identity, weaponHolder);
        currentWeapon = weaponInstance.GetComponent<Weapon>();
        hasWeapon = currentWeapon != null;

        // Disable rotation and floating if weapon pickup script exists
        WeaponPickup weaponPickupScript = weaponInstance.GetComponent<WeaponPickup>();
        if (weaponPickupScript != null)
        {
            weaponPickupScript.enabled = false;
        }
    }

    void HandleAiming()
    {
        if (isAiming)
        {
            RotateGunToMouse();
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0)) // Fire when attack button is pressed
        {
            Vector3 shootDirection = GetShootDirection();
            currentWeapon.Shoot(shootDirection);
        }
    }

    void RotateGunToMouse()
    {
        // Get mouse position in world space and aim along Z and Y axes
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, transform.position);
        
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldMousePosition = ray.GetPoint(distance);
            Vector3 direction = worldMousePosition - transform.position;

            // Smooth rotation of the weapon holder towards the mouse
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            weaponHolder.rotation = Quaternion.Slerp(weaponHolder.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    Vector3 GetShootDirection()
    {
        Vector3 direction = Vector3.zero;

        // Check if the player is moving
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            direction = new Vector3(0, 0, Mathf.Sign(Input.GetAxis("Horizontal"))); // Move along Z-axis
            lastMoveDirection = direction; // Update last move direction
        }

        // If aiming, fire towards mouse direction
        if (isAiming)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.right, transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldMousePosition = ray.GetPoint(distance);
                direction = (new Vector3(transform.position.x, worldMousePosition.y, worldMousePosition.z) - transform.position).normalized;
            }
        }
        else
        {
            // When stationary, use the last move direction
            direction = lastMoveDirection;
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
            Vector3 forcedWeaponPosition = transform.position + new Vector3(0, 0, Mathf.Sign(moveDirection) * weaponDistance);
            weaponHolder.position = forcedWeaponPosition;

            // Make weapon face directly forward along Z-axis
            weaponHolder.rotation = Quaternion.LookRotation(Vector3.forward * Mathf.Sign(moveDirection));
        }
        else if (isAiming) // If aiming, move gun around player in a circle
        {
            // Continue normal rotation around player if stationary
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.right, transform.position); // Plane along X-axis for vertical movement

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldMousePosition = ray.GetPoint(distance);
                Vector3 direction = (worldMousePosition - transform.position).normalized;

                // Convert direction to an angle using atan2 for full 360-degree motion
                float angle = Mathf.Atan2(direction.y, direction.z);

                // Calculate circular position for the weapon holder
                float yOffset = Mathf.Sin(angle) * 1.5f;
                float zOffset = Mathf.Cos(angle) * 1.5f;

                // Set weaponHolder position along the vertical circle
                weaponHolder.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z + zOffset);

                // Make the weapon always point outward, perpendicular to the circle
                Vector3 aimDirection = (weaponHolder.position - transform.position).normalized;
                weaponHolder.rotation = Quaternion.LookRotation(aimDirection);
            }
        }
    }

    void CheckAimingInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isAiming = true;
            if (playerMovement != null) playerMovement.enabled = false; // Disable movement while aiming
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            isAiming = false;
            if (playerMovement != null) playerMovement.enabled = true; // Re-enable movement after aiming
        }
    }
}
