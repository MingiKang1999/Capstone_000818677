/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;

public class PowerUpBlock : MonoBehaviour
{
    public float cooldownTime = 20f;        // how long the block stays inactive
    public float deactivationDelay = 0.15f; // delay before collider turns off
    public Color inactiveColor = Color.gray;

    private SpriteRenderer spriteRenderer;
    private Collider2D blockCollider;
    private Color originalColor;
    private bool isActive = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        blockCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    // Called by PlayerHeadBump when block is triggered
    public void DeactivateBlock()
    {
        if (!isActive)
            return;

        isActive = false;
        StartCoroutine(DeactivateRoutine());
    }

    private System.Collections.IEnumerator DeactivateRoutine()
    {
        // Immediately show grey so the player sees the hit
        if (spriteRenderer != null)
            spriteRenderer.color = inactiveColor;

        // Keep the collider solid for a short moment so the "bonk" happens
        yield return new WaitForSeconds(deactivationDelay);

        if (blockCollider != null)
            blockCollider.enabled = false;

        // Block stays inactive for cooldownTime
        yield return new WaitForSeconds(cooldownTime);

        // Reactivate
        if (blockCollider != null)
            blockCollider.enabled = true;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isActive = true;
    }
}