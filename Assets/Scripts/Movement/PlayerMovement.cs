using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Control")]
    public float speed = 15f;
    public float maxSpeed = 15f;
    private Rigidbody rb;
    private Vector3 offset;
    private Vector3 velocity;
    
    enum Direction
    {
        Left = -1,
        Right = 1
    }

    private float direction = (float)Direction.Right;

    [Header("Jump Settings")]
    // Max height a player can jump
    public float maxJumpHeight = 8f;
    // Min height a player can jump
     public float minJumpHeight = 4f;
    // How much the upward velocity is cut when releasing the jump key
    public float apexFactor = 0.8f;
    // Max downward velocity (terminal velocity)
    public float maxFallSpeed = -15f;
    public bool isJumping = false;
    private Vector3 jumpMovementSlowdown = new Vector3 (0f, 0f, 0.5f);
    // The y value from which the player jumped
    private float jumpY;
    public bool jumpReleased = false;
    public bool hasDoubleJump = true;
    public bool hasDoubleJumped = false;
    private float DOUBLEJUMPCOOLDOWN = 0.5f;
    private float doubleJumpCooldownTimer = 0.0f;

    [Header("Midair Movement")]
    public float airAcceleration = 7.5f;
    // How quickly the horizontal speed decreases when turning.
    public float airDeceleration = 0.75f;

    [Header("Gravity Control")]
    // Default gravity multiplier
    public float gravityScale = 1f;
    // Lower gravity near the apex to increase air time
    public float apexGravityScale = 0.75f;
    // Increase gravity when falling
    public float fallGravityScale = 2f;
    
    [Header("Wall slide/Jump Control")]
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 15f;
    public float wallJumpHorizontalBoost = 10f;
    public LayerMask wallLayer;
    public bool isWallSliding = false;
    // Store the last wall touched
    private Transform lastWall;
    private bool isWallJumping = false;
    // Duration in seconds to preserve wall jump velocity
    private float wallJumpDuration = 0.4f;
    private float wallJumpTimer = 0f;
    private Vector3 lastWallNormal;

    [Header("Ground Check")]
    // Assign this in the Inspector (e.g. an empty GameObject at your feet)
    public Transform groundCheck;
    // How far below the groundCheck to look
    public float groundCheckRadius = 0.2f;
    // Which layers count as ground
    public LayerMask groundMask;
    // Whether the player is touching the ground.
    public bool isGrounded;
    // Maximum angle (in degrees) that is considered walkable
    public float slopeLimit = 50f;
    private List<Collision> collisions = new List<Collision>();

    [Header("Object Interaction")]
    // toggle object interaction because it can be expensive
    public bool doObjectInteraction = false;
    // private InputAction interactAction;
    // radius of sphere for interactions
    public float interactionRadius = 0.5f;
    // sphere offset from origin
    public float interactionOffset = 0.5f;


    [Header("Dash Settings")]
    // Whether the player has unlocked 'dash'
    public bool hasDash = true;
    // Whether the player has unlocked 'chargeDash'
    public bool hasChargeDash = true;
    // Whether the player is currently doing a normal 'dash'.
    public bool isDashing = false;
    // Whether the player is currently charging a 'dash'.
    public bool isDashCharging = false;
    // Whether the player is currently doing a charge dash (Actually moving) 
    public bool isChargeDashing = false;
    // The speed for a normal dash
    public const float DASHSPEED = 20.0f;
    // The speed for a charged dash
    public const float CHARGEDASHSPEED = 40.0f;
    // The total time it takes to charge a charged dash
    private const float CHARGEDASHTIME = 3.0f; // In seconds
    // A timer to keep track of how long the player has charged the dash.
    private float chargeDashTimer = 0.0f;
    // A multiplier to speed up charge times (increases with future powerups)
    public float chargeDashMultiplier = 4.0f;
    // The cooldown between dashes.
    private const float DASHCOOLDOWN = 0.2f; // In seconds
    // The timer to keep track of the dash cooldown.
    public float dashCooldownTimer = 0.0f;
    // The time it takes to complete a 'dash'.
    private const float DASHTIME = 0.2f;
    // The timer keeping track of how long the player has 'dashed'.
    public float dashTimer = 0.0f;

    public int numDashes = 1;
    public int numDashesUsed = 0;

    public PlayerInputHandler inputHandler;


    void Awake()
    {
        // interactAction = InputSystem.actions.FindAction("Interact");
        inputHandler = GameObject.Find("PlayerInputHandler").GetComponent<PlayerInputHandler>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovementInputs();
        if (doObjectInteraction)
        {
            HandleInteractions();
        }
    }

    void FixedUpdate()
    {
        // If the player is currently dashing...
        if (isDashing || isChargeDashing) {
            if (isDashing) {
                rb.linearVelocity = new Vector3(0, 0, DASHSPEED * direction);
            }
            else {
                rb.linearVelocity = new Vector3(0, 0, DASHSPEED * direction * chargeDashMultiplier);
            }


            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer >= 0) {
                return;
            }
            else {
                isDashing = false;
                isChargeDashing = false;
                rb.linearVelocity = new Vector3(0, 0, 0);
                dashCooldownTimer = DASHCOOLDOWN;
                
                if (isGrounded || isWallSliding) {
                    numDashesUsed = 0;
                }
            }
        }
        // If the player is currently charging a dash...
        else if (isDashCharging) {
            chargeDashTimer -= Time.fixedDeltaTime;
            return;
        }
        /* Any other type of movement. */
        else {
            // Reduce dash cooldown
            if (dashCooldownTimer >= 0.0) {
                dashCooldownTimer -= Time.fixedDeltaTime;
            }

            // Reduce double jump cooldown
            if (doubleJumpCooldownTimer >= 0.0) {
                doubleJumpCooldownTimer -= Time.fixedDeltaTime;
            }

            // Check if wallsliding.
            DetectWallSlide();
            // Update wall jump timer if a wall jump is in progress
            if (isWallJumping)
            {
                wallJumpTimer -= Time.fixedDeltaTime;
                if (wallJumpTimer <= 0)
                {
                    isWallJumping = false;
                }
            }

            Vector2 moveInput = inputHandler.MoveInput;
            float moveZ = moveInput.x;

            if (moveZ > 0)
            {
                direction = (float)Direction.Right;
            }
            else if (moveZ < 0) {
                direction = (float)Direction.Left;
            }

            Vector3 newVelocity = new Vector3(0, rb.linearVelocity.y, moveZ * speed);

            if (isGrounded && !isJumping)
            {
                newVelocity.y = 0f;
            }

            if (isWallJumping)
            {
                newVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            }
            else if (isWallSliding)
            {
                newVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            }

            AdjustGravity();

            if (!isGrounded && !isWallSliding)
            {
                AdjustMidAirVelocity();
            }

            rb.linearVelocity = newVelocity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = CheckGrounded();
        if (collision.gameObject.CompareTag("MovingPlatform") && isGrounded)
        {
            isJumping = false;
            hasDoubleJumped = false;
            // Parent the player to the platform
            transform.parent = collision.transform;
        }
        else if (isJumping && rb.linearVelocity.y <= 0 && isGrounded) {
            isJumping = false;
            hasDoubleJumped = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform") && isGrounded)
        {
            isJumping = false;
            hasDoubleJumped = false;
            // Parent the player to the platform
            transform.parent = collision.transform;
        }
        else if (isJumping && rb.linearVelocity.y <= 0 && isGrounded) {
            isJumping = false;
            hasDoubleJumped = false;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            // Check if the normal is mostly horizontal (wall) and not 
            // vertical (ground/roof)
            if (Mathf.Abs(normal.y) < 0.1f) 
            {
                // Convert player input to world space
                Vector3 moveInput = new Vector3(0, 0, Input.GetAxisRaw("Horizontal"));
                Vector3 worldMoveDirection = transform.TransformDirection(moveInput).normalized;

                // Check if movement is pushing against the wall
                float pushStrength = Vector3.Dot(worldMoveDirection, -normal);
                // Small threshold to prevent false positives
                bool pushingAgainstWall = pushStrength > 0.5f;

                if (
                    !isGrounded && rb.linearVelocity.y < 0 &&
                    pushingAgainstWall &&
                    collision.gameObject.CompareTag("Wall")
                ) {
                    isWallSliding = true;
                    lastWall = collision.transform;
                    lastWallNormal = normal;
                    return;
                }
                 else if (pushStrength <= 0.1f && isWallSliding)
                {
                    // Stop wall sliding when no longer pushing the wall
                    isWallSliding = false;
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = CheckGrounded();
        
        if (collision.transform == transform.parent)
        {
            transform.parent = null;       // Un-parent the player
        }
        
        else if (collision.transform == lastWall)
        {
            isWallSliding = false;
            lastWall = null;
        }
    }

    void HandleMovementInputs() {
        /* Handle all the player inputs. */
        // If the player is not already in the dashing movmenet
        if (!isDashing && !isChargeDashing) {
            // Check for 'charge dash'
            if (
                inputHandler.chargeDashTriggered &&
                hasChargeDash &&
                dashCooldownTimer <= 0.0 &&
                numDashesUsed < numDashes
            ) {
                HandleChargingDash();
            }
            // Check for normal 'dash'
            else if (
                inputHandler.dashTriggered &&
                hasDash &&
                dashCooldownTimer <= 0.0 &&
                numDashesUsed < numDashes
            ) {
                Debug.Log("Registered Dash Input");
                isJumping = false;
                hasDoubleJumped = false;
                isWallSliding = false;
                isWallJumping = false;
                HandleDash();
            }
            // Check for Jump
            else if (inputHandler.jumpTriggered)
            {
                HandleJump();
            }
        }

        // Check if the player has released the 'Charge Dash' buttons.
        if (!inputHandler.chargeDashTriggered && isDashCharging)
        {
            isDashCharging = false;
            // If the player has charged long enough, do a 'charged dash'
            if (inputHandler.chargeDashDuration >= CHARGEDASHTIME)
            {
                isChargeDashing = true;
                dashTimer = DASHTIME;
                float dashV = CHARGEDASHSPEED * chargeDashMultiplier * direction;
                rb.linearVelocity = new Vector3(0, dashV, 0);
            }
            // Otherwise, do a normal 'dash'
            else {
                HandleDash();
            }
        }

        // Check if the "Jump" button is released (For a shorter jump)
        if (isJumping && !inputHandler.jumpTriggered)
        {
            jumpReleased = true;
        }
    }

    void HandleChargingDash()
    {
        // Disable any other movement
        isJumping = false;
        hasDoubleJumped = false;
        isWallSliding = false;
        isWallJumping = false;

        // Start the Charging Timer
        isDashCharging = true;
        chargeDashTimer = CHARGEDASHTIME;
        rb.linearVelocity = new Vector3(0, 0, 0);
    }


    void HandleDash()
    {
        // Disable any other movement
        isJumping = false;
        hasDoubleJumped = false;
        isWallSliding = false;
        isWallJumping = false;
        
        // Start the dash movement 
        isDashing = true;
        dashTimer = DASHTIME;
        numDashesUsed += 1;
    }

    void HandleJump()
    {
        // Normal Jump
        if (isGrounded)
        {
            isJumping = true;
            isGrounded = false;
            jumpY = rb.position.y;
            jumpReleased = false;
            doubleJumpCooldownTimer = DOUBLEJUMPCOOLDOWN;

            float jumpVelocity = Mathf.Sqrt(
                2 * Mathf.Abs(Physics.gravity.y) * maxJumpHeight
            );
            rb.linearVelocity = new Vector3(
                0, jumpVelocity, rb.linearVelocity.z
            );
            // Debug.Log($"Jump Velocity: {rb.linearVelocity}");
        }
        // Wall Jump
        else if (isWallSliding) {
            isWallSliding = false;
            isJumping = true;
            isWallJumping = true;
            wallJumpTimer = wallJumpDuration;
            doubleJumpCooldownTimer = DOUBLEJUMPCOOLDOWN;

            // Calculate the jump components for a 45° jump:
            float componentForce = wallJumpForce / Mathf.Sqrt(2f);
            // The horizontal component should be the wall's normal
            // (which is perpendicular to the wall)
            Vector3 horizontalJump = lastWallNormal * componentForce;
            // Vertical component is straight up.
            Vector3 verticalJump = Vector3.up * componentForce;

            // Combine to get a jump at 45° relative to the wall
            rb.linearVelocity = horizontalJump + verticalJump;
        }
        // Double Jump
        else if (hasDoubleJump && !hasDoubleJumped && doubleJumpCooldownTimer <= 0) {
            isJumping = true;
            hasDoubleJumped = true;
            isGrounded = false;
            jumpY = rb.position.y;
            jumpReleased = false;

            float jumpVelocity = Mathf.Sqrt(
                2 * Mathf.Abs(Physics.gravity.y) * maxJumpHeight
            );
            rb.linearVelocity = new Vector3(
                0, jumpVelocity, rb.linearVelocity.z
            );
        }
    }

    void AdjustGravity()
    {

        // When the player is moving upwards (jumping)
        if (rb.linearVelocity.y > 0 && !isGrounded) // Moving Up
        {
            // Handle early jump release or max jump height
            if (isJumping && !isWallJumping)
            {
                // If Jump is released early and min height is reached, reduce velocity
                if (
                    jumpReleased && rb.position.y >= minJumpHeight + jumpY &&
                    rb.linearVelocity.y > 0    
                ) {
                    rb.linearVelocity = new Vector3(0, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z);
                }

                // Stop upward movement if max height is reached
                if (
                    rb.position.y >= jumpY + maxJumpHeight &&
                    rb.linearVelocity.y > 0
                ) {
                    rb.linearVelocity = new Vector3(
                        0, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z
                    );
                }
            }
        }
        // When the player is falling (and not wall sliding)
        else if ((rb.linearVelocity.y < 0 && !isWallSliding)) // Falling
        {
            rb.linearVelocity += (
                Vector3.up * Physics.gravity.y * fallGravityScale *
                Time.deltaTime
            );
        }
        else if (isWallSliding && rb.linearVelocity.y <= 0) {
            rb.linearVelocity = new Vector3(
                0, -wallSlideSpeed, rb.linearVelocity.z
            );
        }

        // Cap max fall speed
        if (rb.linearVelocity.y < maxFallSpeed)
        {
            // Debug.Log("Hit Max Speed.");
            rb.linearVelocity = new Vector3(
                0, maxFallSpeed, rb.linearVelocity.z
            );
        }
    }


    void AdjustMidAirVelocity()
    {
        Vector3 input = new Vector3(0, 0, Input.GetAxisRaw("Horizontal"));

        if (input.magnitude > 0)
        {
            // Apply acceleration in the input direction
            rb.linearVelocity += input * airAcceleration * Time.deltaTime;
        }
        else
        {
            // Apply friction when no input is given
            Vector3 horizontalVelocity = new Vector3(0, 0, rb.linearVelocity.z);
            rb.linearVelocity -= (
                horizontalVelocity * airDeceleration * Time.deltaTime
            );
        }

        // Clamp horizontal speed
        Vector3 horizontalVelocityOnly = new Vector3(0, 0, rb.linearVelocity.z);
        if (horizontalVelocityOnly.magnitude > maxSpeed)
        {
            horizontalVelocityOnly = (
                horizontalVelocityOnly.normalized * maxSpeed
            );
            rb.linearVelocity = new Vector3(
                0, rb.linearVelocity.y, horizontalVelocityOnly.z
            );
        }
    }


    bool CheckGrounded()
    {
        RaycastHit hit;

        // Get the radius of the player's capsule collider
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        float sphereCastRadius = capsuleCollider.radius;
        
        // The length of the spherecast (how far down to check)
        float castLength = 0.55f;

        // Perform the spherecast from the player's position downward
        if (Physics.SphereCast(
                               transform.position,
                               sphereCastRadius,
                               Vector3.down,
                               out hit,
                               castLength,
                               groundMask
                              )
            ) {
                numDashesUsed = 0;
            // Check if the angle of the ground is within the allowed slope limit
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle <= slopeLimit)
            {
                isWallSliding = false;
                return true;
            }
        }
        return false;
    }

    void DetectWallSlide()
    {
        RaycastHit hit;

        // Get the radius of the player's capsule collider
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        float capsuleRadius = capsuleCollider.radius * 0.9f; // Slightly smaller to avoid false positives
        float capsuleHeight = capsuleCollider.height * 0.5f; // Half-height for proper positioning
        
        // The length of the spherecast (how far down to check)
        float castDistance = 0.5f;

        Vector3 start = transform.position + Vector3.up * capsuleRadius;  // Bottom of capsule
        Vector3 end = transform.position + Vector3.up * (capsuleHeight - capsuleRadius); // Top of capsule

        Transform detectedWall = null;

        // Check in both directions
        if (Physics.CapsuleCast(start, end, capsuleRadius, transform.right, out hit, castDistance, wallLayer))
        {
            detectedWall = hit.transform;
        }
        else if (Physics.CapsuleCast(start, end, capsuleRadius, -transform.right, out hit, castDistance, wallLayer))
        {
            detectedWall = hit.transform;
        }

        // If a wall was detected and the player is falling, enable wall sliding
        if (detectedWall != null && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            isJumping = false;
            hasDoubleJumped = false;
            lastWall = detectedWall;
            numDashesUsed = 0;
            return;
        }

        isWallSliding = false;
    }

    void HandleInteractions()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + (transform.forward * interactionOffset), interactionRadius);
        foreach (var hitCollider in hitColliders)
        {
            ButtonController buttonController;
            if (hitCollider.gameObject.TryGetComponent<ButtonController>(out buttonController))
            {
                buttonController.SetActivatable();
                if (inputHandler.chargeDashTriggered)
                {
                    buttonController.Activate();
                }
            }
        }
    }
}
