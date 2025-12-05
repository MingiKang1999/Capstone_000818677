/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb; // Render player as body
    private float inputAxis; // Player location
    private float currentSpeed; // Current player speed
    private bool isGrounded; // Check if player is on the ground

    [Header("Movement Settings")]
    public float moveSpeed = 250f; // Max horizontal speed
    public float acceleration = 400f; // How fast to accelerate
    public float deceleration = 500f; // How fast to stop
    public float jumpForce = 230f; // Jump strength
    public float gravityScale = 20f; // Custom gravity scale

    [Header("Ground Check Settings")]
    public Transform groundCheck; // Empty GameObject under player
    public float groundRadius = 0.2f; // Circle radius for detection
    public LayerMask groundLayer; // Assign your Ground layer here

    [Header("Level Boundaries")]
    public float minX = 0f; // Left end side of the screen
    public float maxX = 2600f; // Right end side of the screen

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // prevent tipping over
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        inputAxis = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Jump if they are on the ground
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Vector2 v = rb.linearVelocity;
            v.y = jumpForce; // How high you can jump
            rb.linearVelocity = v; // How fast you can jump
        }

        // Better jump feel (stronger fall, short hops)
        if (rb.linearVelocity.y < 0)
        {
            // Adds extra downward velocity by applying stronger gravity over time to make falling faster
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (gravityScale * 1.5f - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Short hop when releasing jump early
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (gravityScale - 1) * Time.deltaTime;
        }

        // NEW: keep Rigidbody gravityScale in sync with the field
        rb.gravityScale = gravityScale;
    }

    private void FixedUpdate()
    {
        // Smooth horizontal acceleration and deceleration
        float targetSpeed = inputAxis * moveSpeed;

        if (Mathf.Abs(inputAxis) > 0.01f)
            // Gradually changes currentSpeed toward targetSpeed at a fixed acceleration rate, ensuring smooth acceleration over time
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        else
            // Gradually reduces currentSpeed toward 0 using the deceleration rate, creating smooth stopping motion
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);

        Vector2 velocity = rb.linearVelocity; // Get the current velocity of the Rigidbody2D
        velocity.x = currentSpeed; // Replace the horizontal (x) velocity with the calculated movement speed
        rb.linearVelocity = velocity; // Apply the updated velocity back to the Rigidbody2D

        // Clamp position to level bounds
        Vector2 pos = rb.position; // Get the current position of the Rigidbody2D
        pos.x = Mathf.Clamp(pos.x, minX, maxX); // Limit the x position so it stays within the level boundaries (minX to maxX)
        rb.position = pos; // Apply the clamped position back to the Rigidbody2D
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
           
        }
    }
}
