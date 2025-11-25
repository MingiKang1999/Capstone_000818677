using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 100f;
    public Vector2 direction = Vector2.right;

    [Header("Movement Boundaries")]
    public float minX = 600f;   // Left boundary
    public float maxX = 900f;   // Right boundary

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enabled = false;
    }

    private void OnBecameVisible() => enabled = true;
    private void OnBecameInvisible() => enabled = false;

    private void OnEnable() => rb.WakeUp();
    private void OnDisable() => rb.Sleep();

    private void FixedUpdate()
    {
        // Only horizontal movement
        float moveX = direction.x * speed * Time.fixedDeltaTime;
        Vector2 newPos = rb.position + new Vector2(moveX, 0);

        // Boundary flip
        if (newPos.x <= minX)
        {
            direction = Vector2.right;
            spriteRenderer.flipX = false;
        }
        else if (newPos.x >= maxX)
        {
            direction = Vector2.left;
            spriteRenderer.flipX = true;
        }

        rb.MovePosition(newPos);
    }

    // SIDE DAMAGE
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var playerRb = collision.collider.GetComponent<Rigidbody2D>();
            var death = collision.collider.GetComponent<PlayerDeath>();

            if (death == null) return;

            // NOT a stomp player dies
            if (playerRb.linearVelocity.y >= 0)
            {
                death.Die();
            }
        }
    }
}