using UnityEngine;

public class PlayerHeadBump : MonoBehaviour
{
    public Color hitColor = Color.red;
    public float newSpeed = 50f;
    public float effectDuration = 1f;

    private PlayerMovement playerMovement;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        // Always get components from PLAYER root (golem)
        Transform root = transform.root;

        playerMovement = root.GetComponent<PlayerMovement>();
        playerRb = root.GetComponent<Rigidbody2D>();
        playerSprite = root.GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item"))
            return;

        // Only trigger if moving UP
        if (playerRb.linearVelocity.y > 0)
        {
            StartCoroutine(ApplyHeadBumpEffect());
        }
    }

    private System.Collections.IEnumerator ApplyHeadBumpEffect()
    {
        Color originalColor = playerSprite.color;
        float originalSpeed = playerMovement.moveSpeed;

        playerSprite.color = hitColor;
        playerMovement.moveSpeed = newSpeed;

        yield return new WaitForSeconds(effectDuration);

        playerSprite.color = originalColor;
        playerMovement.moveSpeed = originalSpeed;
    }
}