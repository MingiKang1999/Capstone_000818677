using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathDelay = 1f;

    // Kill the player if they fall below this Y
    public float killHeight = -10f;

    private bool hasDied = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerMovement movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Fall death check
        if (!hasDied && transform.position.y < killHeight)
        {
            Die();
        }
    }

    public void Die()
    {
        if (hasDied)
            return;

        hasDied = true;

        if (movement != null)
            movement.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.Sleep();
        }

        // Turn player black
        if (sr != null)
            sr.color = Color.black;

        // disable all player colliders
        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        // use your GameManager death pipeline
        GameManager.Instance.ResetLevel(deathDelay);
    }
}