
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerWeaponHandler : MonoBehaviour
// {
//     public Transform weaponHolder; // Empty GameObject where weapon will be attached
//     public float shootingCooldown = 0.5f; // Time between shots
//     private Weapon currentWeapon;
//     private bool hasWeapon = false;
//     private bool isAiming = false;
//     private Vector3 lastMoveDirection = Vector3.forward; // Direction player last moved in
//     private PlayerMovement playerMovement;
//     private float shootingTimer = 0f;
//     private float moveDirection;

//     public bool alwaysAim = true; // For testing purposes


//     void Start()
//     {
//         playerMovement = GetComponent<PlayerMovement>();
//     }

//     void Update()
//     {
//         if (hasWeapon)
//         {
//             HandleAiming();
//             HandleShooting();
//             MoveGunAroundPlayer();
//         }

//         CheckAimingInput();
//         if (shootingTimer > 0) shootingTimer -= Time.deltaTime;

//         moveDirection = Input.GetAxis("Horizontal");

//         if (Mathf.Abs(moveDirection) > 0.1f)
//         {
//             lastMoveDirection = new Vector3(0, 0, Mathf.Sign(moveDirection));
//         }

//         if (Input.GetKeyDown(KeyCode.P))
//         {
//             alwaysAim = !alwaysAim;
//         }
//     }

//     public void PickUpWeapon(GameObject weaponPrefab)
//     {
//         // Remove previous weapon if applicable
//         if (currentWeapon != null)
//         {
//             Destroy(currentWeapon.gameObject);
//         }

//         // Instantiate new weapon and assign it to the weapon holder
//         GameObject weaponInstance = Instantiate(weaponPrefab, weaponHolder.position, Quaternion.identity, weaponHolder);
//         currentWeapon = weaponInstance.GetComponent<Weapon>();
//         hasWeapon = currentWeapon != null;

//         // Disable rotation and floating if weapon pickup script exists
//         WeaponPickup weaponPickupScript = weaponInstance.GetComponent<WeaponPickup>();
//         if (weaponPickupScript != null)
//         {
//             weaponPickupScript.enabled = false;
//         }
//     }

//     void HandleAiming()
//     {
//         if (isAiming || alwaysAim)
//         {
//             RotateGunToMouse();
//         }
//     }

//     void HandleShooting()
//     {
//         if (shootingTimer <= 0f && Input.GetMouseButtonDown(0)) // Fire when attack button is pressed
//         {
//             Vector3 shootDirection = GetShootDirection();
//             currentWeapon.Shoot(shootDirection);
//             shootingTimer = shootingCooldown;
//         }
//     }

//     void RotateGunToMouse()
//     {
//         // Get mouse position in world space and aim along Z and Y axes
//         Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
//         Plane plane = new Plane(Vector3.up, transform.position);
        
//         if (plane.Raycast(ray, out float distance))
//         {
//             Vector3 worldMousePosition = ray.GetPoint(distance);
//             Vector3 direction = worldMousePosition - transform.position;

//             // Smooth rotation of the weapon holder towards the mouse
//             Quaternion targetRotation = Quaternion.LookRotation(direction);
//             weaponHolder.rotation = Quaternion.Slerp(weaponHolder.rotation, targetRotation, Time.deltaTime * 10f);
//         }
//     }

//     Vector3 GetShootDirection()
//     {
//         Vector3 direction = Vector3.zero;

//         // If aiming, fire towards mouse direction
//         if (isAiming || alwaysAim)
//         {
//             Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
//             Plane plane = new Plane(Vector3.right, transform.position);

//             if (plane.Raycast(ray, out float distance))
//             {
//                 Vector3 worldMousePosition = ray.GetPoint(distance);
//                 direction = (new Vector3(transform.position.x, worldMousePosition.y, worldMousePosition.z) - transform.position).normalized;
//             }
//         }
//         else
//         {
//             // When stationary, use the last move direction
//             direction = lastMoveDirection;
//         }

//         return direction;
//     }

//     void MoveGunAroundPlayer()
//     {

//         // If player is moving, force the gun to stay in front or behind
//         if (Mathf.Abs(moveDirection) > 0.1f && !alwaysAim) // Threshold to prevent jittering
//         {
//             float weaponDistance = 1.5f; // Distance from player
//             Vector3 forcedWeaponPosition = transform.position + new Vector3(0, 0, Mathf.Sign(moveDirection) * weaponDistance);
//             weaponHolder.position = forcedWeaponPosition;

//             // Make weapon face directly forward along Z-axis
//             weaponHolder.rotation = Quaternion.LookRotation(Vector3.forward * Mathf.Sign(moveDirection));
//         }
//         else if (isAiming || alwaysAim) // If aiming, move gun around player in a circle
//         {
//             // Continue normal rotation around player if stationary
//             Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
//             Plane plane = new Plane(Vector3.right, transform.position); // Plane along X-axis for vertical movement

//             if (plane.Raycast(ray, out float distance))
//             {
//                 Vector3 worldMousePosition = ray.GetPoint(distance);
//                 Vector3 direction = (worldMousePosition - transform.position).normalized;

//                 // Convert direction to an angle using atan2 for full 360-degree motion
//                 float angle = Mathf.Atan2(direction.y, direction.z);

//                 // Calculate circular position for the weapon holder
//                 float yOffset = Mathf.Sin(angle) * 1.5f;
//                 float zOffset = Mathf.Cos(angle) * 1.5f;

//                 // Set weaponHolder position along the vertical circle
//                 weaponHolder.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z + zOffset);

//                 // Make the weapon always point outward, perpendicular to the circle
//                 Vector3 aimDirection = (weaponHolder.position - transform.position).normalized;
//                 weaponHolder.rotation = Quaternion.LookRotation(aimDirection);
//             }
//         }
//     }

//     void CheckAimingInput()
//     {
//         if (!alwaysAim)
//         {
//             if (Input.GetKeyDown(KeyCode.Q))
//             {
//                 isAiming = true;
//                 if (playerMovement != null) playerMovement.enabled = false; // Disable movement while aiming
//             }
//             else if (Input.GetKeyUp(KeyCode.Q))
//             {
//                 isAiming = false;
//                 if (playerMovement != null) playerMovement.enabled = true; // Re-enable movement after aiming

//                 Vector3 weaponForward = weaponHolder.forward;
//                 float zComponent = weaponForward.z;
//                 Vector3 snapDirection = zComponent > 0 ? Vector3.forward : Vector3.back;

//                 // Snap weapon to the closest Z direction
//                 weaponHolder.rotation = Quaternion.LookRotation(snapDirection);
//                 Vector3 forcedWeaponPosition = transform.position + new Vector3(0, 0, snapDirection.z * 1.5f);
//                 weaponHolder.position = forcedWeaponPosition;

//                 lastMoveDirection = snapDirection;
//             }
//         }
//     }
// }
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    public Transform weaponHolder;
    public float shootingCooldown = 0.5f;
    private Weapon currentWeapon;
    private bool hasWeapon = false;
    private bool isAiming = false;
    private Vector3 lastMoveDirection = Vector3.forward;
    private PlayerMovement playerMovement;
    private float shootingTimer = 0f;
    private float moveDirection;

    public bool alwaysAim = true;

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
        if (shootingTimer > 0) shootingTimer -= Time.deltaTime;

        moveDirection = Input.GetAxis("Horizontal");

        if (Mathf.Abs(moveDirection) > 0.1f)
        {
            lastMoveDirection = new Vector3(0, 0, Mathf.Sign(moveDirection));
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            alwaysAim = !alwaysAim;
        }
    }

    public void PickUpWeapon(GameObject weaponPrefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        GameObject weaponInstance = Instantiate(weaponPrefab, weaponHolder.position, Quaternion.identity, weaponHolder);
        currentWeapon = weaponInstance.GetComponent<Weapon>();
        hasWeapon = currentWeapon != null;

        WeaponPickup weaponPickupScript = weaponInstance.GetComponent<WeaponPickup>();
        if (weaponPickupScript != null)
        {
            weaponPickupScript.enabled = false;
        }
    }

    void HandleAiming()
    {
        if (isAiming || alwaysAim)
        {
            RotateGunToMouse();
        }
    }

    void HandleShooting()
    {
        if (shootingTimer <= 0f && Input.GetMouseButtonDown(0))
        {
            Vector3 shootDirection = GetShootDirection();
            currentWeapon.Shoot(shootDirection);
            shootingTimer = shootingCooldown;
        }
    }

    void RotateGunToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldMousePosition = ray.GetPoint(distance);
            Vector3 direction = worldMousePosition - transform.position;

            // ✅ Correct orientation: Barrel now faces outward, not at the player
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.down);

            // ✅ Apply 90-degree X-axis + 180-degree Z-axis rotation to face outward
            weaponHolder.rotation = targetRotation;
        }
    }


    Vector3 GetShootDirection()
    {
        Vector3 direction = Vector3.zero;

        if (isAiming || alwaysAim)
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
            direction = lastMoveDirection;
        }

        return direction;
    }

    void MoveGunAroundPlayer()
    {
        if (Mathf.Abs(moveDirection) > 0.1f && !alwaysAim)
        {
            float weaponDistance = 1.5f;
            Vector3 forcedWeaponPosition = transform.position + new Vector3(0, 0, Mathf.Sign(moveDirection) * weaponDistance);
            weaponHolder.position = forcedWeaponPosition;

            // ✅ Fix the gun's forward direction using negative Y-axis
            weaponHolder.rotation = Quaternion.LookRotation(Vector3.forward * Mathf.Sign(moveDirection)) * Quaternion.Euler(0f, 0f, 0f);
        }
        else if (isAiming || alwaysAim)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.right, transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldMousePosition = ray.GetPoint(distance);
                Vector3 direction = (worldMousePosition - transform.position).normalized;

                float angle = Mathf.Atan2(direction.y, direction.z);

                float yOffset = Mathf.Sin(angle) * 1.5f;
                float zOffset = Mathf.Cos(angle) * 1.5f;
                weaponHolder.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z + zOffset);

                // ✅ Rotate gun barrel outward, ensuring -Y is forward
                weaponHolder.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90f, 0f, 0f);
            }
        }
    }

    void CheckAimingInput()
    {
        if (!alwaysAim)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                isAiming = true;
                if (playerMovement != null) playerMovement.enabled = false;
            }
            else if (Input.GetKeyUp(KeyCode.Q))
            {
                isAiming = false;
                if (playerMovement != null) playerMovement.enabled = true;

                Vector3 weaponForward = weaponHolder.forward;
                float zComponent = weaponForward.z;
                Vector3 snapDirection = zComponent > 0 ? Vector3.forward : Vector3.back;

                weaponHolder.rotation = Quaternion.LookRotation(snapDirection) * Quaternion.Euler(90f, 0f, 0f);
                Vector3 forcedWeaponPosition = transform.position + new Vector3(0, 0, snapDirection.z * 1.5f);
                weaponHolder.position = forcedWeaponPosition;

                lastMoveDirection = snapDirection;
            }
        }
    }
}
