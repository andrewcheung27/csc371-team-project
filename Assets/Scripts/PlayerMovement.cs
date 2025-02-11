using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Control")]
    public float speed = 15f;
    public float maxSpeed = 15f;
    private Rigidbody rb;
    private Vector3 offset;
    private Vector3 velocity;

    [Header("Jump Settings")]
    public float maxJumpHeight = 8f;          // Max height a player can jump
     public float minJumpHeight = 4f;          // Min height a player can jump
    public float apexFactor = 0.8f;   // How much the upward velocity is cut when releasing the jump key
    public float maxFallSpeed = -15f;      // Max downward velocity (terminal velocity)
    public bool isJumping = false;
    private Vector3 jumpMovementSlowdown = new Vector3 (0f, 0f, 0.5f);
    private float jumpY;                    // The y value from which the player jumped
    public bool jumpReleased = false;

    [Header("Midair Movement")]
    public float airAcceleration = 7.5f;
    public float rotationSpeed = 360f;            // How fast (degrees per second) the horizontal velocity rotates toward input.
    public float airDeceleration = 0.75f;           // How quickly the horizontal speed decreases when turning.

    [Header("Gravity Control")]
    public float gravityScale = 1f;        // Default gravity multiplier
    public float apexGravityScale = 0.75f;  // Lower gravity near the apex to increase air time
    public float fallGravityScale = 2f;    // Increase gravity when falling
    
    [Header("Wall slide/Jump Control")]
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 15f;
    public float wallJumpHorizontalBoost = 10f;
    public LayerMask wallLayer;
    public bool isWallSliding = false;
    private Transform lastWall;  // Store the last wall touched
    private bool isWallJumping = false;
    private float wallJumpDuration = 0.4f; // Duration in seconds to preserve wall jump velocity
    private float wallJumpTimer = 0f;
    private Vector3 lastWallNormal;

    [Header("Ground Check")]
    public Transform groundCheck;  // Assign this in the Inspector (e.g. an empty GameObject at your feet)
    public float groundCheckRadius = 0.2f;  // How far below the groundCheck to look
    public LayerMask groundMask;   // Which layers count as ground
    public bool isGrounded;
    public float slopeLimit = 50f; // Maximum angle (in degrees) that is considered walkable
    private List<Collision> collisions = new List<Collision>();

    [Header("Object Interaction")]
    public bool doObjectInteraction = false;  // toggle object interaction because it can be expensive
    public InputAction interactAction;
    public float interactionRadius = 0.5f;  // radius of sphere for interactions
    public float interactionOffset = 0.5f;  // sphere offset from origin

    void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // transform.rotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    void Update()
    {
        HandleJump();

        if (doObjectInteraction)
        {
            HandleInteractions();
        }
    }

    void FixedUpdate()
    {
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

        float moveZ = Input.GetAxis("Horizontal");

        Vector3 currentVelocity = rb.linearVelocity;

        // Get the current velocity from the rigidbody
        Vector3 newVelocity = new Vector3(0, rb.linearVelocity.y, moveZ * speed);

        if (isGrounded && !isJumping)
        {
            // When on a slope that is walkable, zero out any vertical component.
            // This prevents sliding down or the residual vertical velocity from accumulating.
            currentVelocity.y = 0f;
        }

        // If wall jumping, preserve the current horizontal velocity (or blend it slowly)
        if (isWallJumping)
        {
            newVelocity = new Vector3(0, currentVelocity.y, currentVelocity.z);
        }
        // If wall sliding, no horizontal movement allowed
        else if (isWallSliding)
        {
            newVelocity = new Vector3(0, currentVelocity.y, currentVelocity.z);
        }

        // Handle gravity adjustments
        AdjustGravity();

        // Check for midair movement and adjust horizontal velocity accordingly.
        if (!isGrounded && !isWallSliding)
        {
            AdjustMidAirVelocity();

        }
        
        rb.linearVelocity = newVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = CheckGrounded();
        if (collision.gameObject.CompareTag("MovingPlatform") && isGrounded)
        {
            isJumping = false;
            transform.parent = collision.transform;     // Parent the player to the platform
        }
        else if (isJumping && rb.linearVelocity.y <= 0 && isGrounded) {
            isJumping = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform") && isGrounded)
        {
            isJumping = false;
            transform.parent = collision.transform;     // Parent the player to the platform
        }
        else if (isJumping && rb.linearVelocity.y <= 0 && isGrounded) {
            isJumping = false;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            // Check if the normal is mostly horizontal (wall) and not vertical (ground)
            if (Mathf.Abs(normal.y) < 0.1f) 
            {
                // Convert player input to world space
                Vector3 moveInput = new Vector3(0, 0, Input.GetAxisRaw("Horizontal"));
                Vector3 worldMoveDirection = transform.TransformDirection(moveInput).normalized;

                // Check if movement is pushing against the wall
                float pushStrength = Vector3.Dot(worldMoveDirection, -normal);
                bool pushingAgainstWall = pushStrength > 0.5f; // Small threshold to prevent false positives

                if (!isGrounded && rb.linearVelocity.y < 0 && pushingAgainstWall && collision.gameObject.CompareTag("Wall"))
                {
                    isWallSliding = true;
                    lastWall = collision.transform;
                    lastWallNormal = normal;
                    return;
                }
                 else if (pushStrength <= 0.1f && isWallSliding)
                {
                    // Stop wall sliding when key is no longer pushing against the wall
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

    void HandleJump()
    {
        // Debug.Log($"Y Velocity: {rb.linearVelocity.y}");
        if (Input.GetButtonDown("Jump"))
        {
            // Normal Jump
            if (isGrounded)
            {
                isJumping = true;
                isGrounded = false;
                jumpY = rb.position.y;
                jumpReleased = false;

                float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * maxJumpHeight);
                rb.linearVelocity = new Vector3(0, jumpVelocity, rb.linearVelocity.z);
                // Debug.Log($"Jump Velocity: {rb.linearVelocity}");
            }
            // Wall Jump
            else if (isWallSliding) {
                isWallSliding = false;
                isJumping = true;
                isWallJumping = true;
                wallJumpTimer = wallJumpDuration;

                // Calculate the jump components for a 45° jump:
                float componentForce = wallJumpForce / Mathf.Sqrt(2f);
                // The horizontal component should be the wall's normal (which is perpendicular to the wall)
                Vector3 horizontalJump = lastWallNormal * componentForce;
                // Vertical component is straight up.
                Vector3 verticalJump = Vector3.up * componentForce;

                // Combine to get a jump at 45° relative to the wall
                rb.linearVelocity = horizontalJump + verticalJump;
            }
        }
        
        if (Input.GetButtonUp("Jump"))
        {
            jumpReleased = true;
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
                if (jumpReleased && rb.position.y >= minJumpHeight + jumpY && rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector3(0, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z);
                }

                // Stop upward movement if max height is reached
                if (rb.position.y >= jumpY + maxJumpHeight && rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector3(0, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z);
                }
            }
        }
        // When the player is falling (and not wall sliding)
        else if ((rb.linearVelocity.y < 0 && !isWallSliding)) // Falling
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallGravityScale * Time.deltaTime;
        }
        else if (isWallSliding && rb.linearVelocity.y <= 0) {
            rb.linearVelocity = new Vector3(0, -wallSlideSpeed, rb.linearVelocity.z);
        }

        // Cap max fall speed
        if (rb.linearVelocity.y < maxFallSpeed)
        {
            // Debug.Log("Hit Max Speed.");
            rb.linearVelocity = new Vector3(0, maxFallSpeed, rb.linearVelocity.z);
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
            rb.linearVelocity -= horizontalVelocity * airDeceleration * Time.deltaTime;
        }

        // Clamp horizontal speed
        Vector3 horizontalVelocityOnly = new Vector3(0, 0, rb.linearVelocity.z);
        if (horizontalVelocityOnly.magnitude > maxSpeed)
        {
            horizontalVelocityOnly = horizontalVelocityOnly.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, horizontalVelocityOnly.z);
        }
    }


    bool CheckGrounded()
    {
        RaycastHit hit;

        // Get the radius of the player's capsule collider
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        float sphereCastRadius = capsuleCollider.radius;  // Use the capsule's radius
        
        // The length of the spherecast (how far down to check)
        float castLength = 0.55f;

        // Perform the spherecast from the player's position downward
        if (Physics.SphereCast(transform.position, sphereCastRadius, Vector3.down, out hit, castLength, groundMask))
        {
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
            lastWall = detectedWall;
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
                if (interactAction.WasPressedThisFrame())
                {
                    buttonController.Activate();
                }
            }
        }
    }
}
