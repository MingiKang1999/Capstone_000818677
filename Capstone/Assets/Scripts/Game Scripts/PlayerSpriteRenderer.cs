using UnityEngine;

public class PlayerSpriteRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Sprites")]
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite jumpSprite;
    public Sprite fallSprite;

    private bool isGrounded;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateSprite();
        HandleFlip();
    }

    private void UpdateSprite()
    {
        // Check ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Jumping
        if (!isGrounded && rb.linearVelocity.y > 0.1f)
        {
            spriteRenderer.sprite = jumpSprite;
            return;
        }

        // Falling
        if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            spriteRenderer.sprite = fallSprite;
            return;
        }

        // Walking
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            spriteRenderer.sprite = walkSprite;
            return;
        }

        // Idle
        spriteRenderer.sprite = idleSprite;
    }

    private void HandleFlip()
    {
        if (rb.linearVelocity.x > 0.1f)
            spriteRenderer.flipX = true;

        else if (rb.linearVelocity.x < -0.1f)
            spriteRenderer.flipX = false;
    }
}