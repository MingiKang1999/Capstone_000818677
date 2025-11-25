using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathDelay = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerMovement movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // IMPORTANT: player sprite is on a child object
        sr = GetComponentInChildren<SpriteRenderer>();

        movement = GetComponent<PlayerMovement>();
    }

    public void Die()
    {
        if (movement != null)
            movement.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.Sleep();

        // turn player black
        if (sr != null)
            sr.color = Color.black;

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        GameManager.Instance.ResetLevel(deathDelay);
    }
}