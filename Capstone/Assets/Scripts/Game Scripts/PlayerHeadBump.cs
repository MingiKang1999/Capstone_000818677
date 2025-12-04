using UnityEngine;

public class PlayerHeadBump : MonoBehaviour
{
    [Header("Tags to detect")]
    public string speedTag = "SpeedItem";
    public string slowFallTag = "SlowFallItem";
    public string highJumpTag = "HighJumpItem";

    [Header("Speed boost effect")]
    public Color speedColor = Color.red;
    public float boostedMoveSpeed = 400f;
    public float speedDuration = 10f;

    [Header("Slow fall effect")]
    public Color slowFallColor = Color.green;
    public float slowFallGravityScale = 5f;   // lower than normal gravityScale
    public float slowFallDuration = 10f;

    [Header("High jump effect")]
    public Color highJumpColor = Color.blue;
    public float boostedJumpForce = 300f;     // higher than your normal jumpForce
    public float highJumpDuration = 10f;     // how long the buff lasts

    private PlayerMovement playerMovement;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;

    private Coroutine activeEffect;

    // Base stats (true defaults)
    private Color baseColor;
    private float baseMoveSpeed;
    private float baseGravityScale;
    private float baseJumpForce;

    private void Awake()
    {
        Transform root = transform.root;

        playerMovement = root.GetComponent<PlayerMovement>();
        playerRb = root.GetComponent<Rigidbody2D>();
        playerSprite = root.GetComponentInChildren<SpriteRenderer>();

        if (playerSprite != null)
            baseColor = playerSprite.color;
        else
            baseColor = Color.white;

        if (playerMovement != null)
        {
            baseMoveSpeed = playerMovement.moveSpeed;
            baseGravityScale = playerMovement.gravityScale;
            baseJumpForce = playerMovement.jumpForce;
        }
    }

    private enum HeadEffectType
    {
        None,
        Speed,
        SlowFall,
        HighJump
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerRb == null || playerMovement == null)
            return;

        // Only trigger when moving upward (head bump)
        if (playerRb.linearVelocity.y <= 0f)
            return;

        HeadEffectType effect = HeadEffectType.None;

        if (collision.CompareTag(speedTag))
        {
            effect = HeadEffectType.Speed;
        }
        else if (collision.CompareTag(slowFallTag))
        {
            effect = HeadEffectType.SlowFall;
        }
        else if (collision.CompareTag(highJumpTag))
        {
            effect = HeadEffectType.HighJump;
        }

        if (effect == HeadEffectType.None)
            return;

        // Tell the block to handle cooldown / grey color
        PowerUpBlock block = collision.GetComponent<PowerUpBlock>();
        if (block != null)
            block.DeactivateBlock();

        // Stop previous effect and reset to base BEFORE applying new one
        if (activeEffect != null)
        {
            StopCoroutine(activeEffect);
            activeEffect = null;
        }
        ResetToBaseStats();

        activeEffect = StartCoroutine(ApplyHeadBumpEffect(effect));
    }

    private System.Collections.IEnumerator ApplyHeadBumpEffect(HeadEffectType effectType)
    {
        if (playerRb == null || playerMovement == null)
            yield break;

        float duration = 0f;
        bool useSlowFallLogic = false;

        // Apply effect on top of base values
        switch (effectType)
        {
            case HeadEffectType.Speed:
                duration = speedDuration;
                if (playerSprite != null)
                    playerSprite.color = speedColor;

                playerMovement.moveSpeed = boostedMoveSpeed;
                break;

            case HeadEffectType.SlowFall:
                duration = slowFallDuration;
                if (playerSprite != null)
                    playerSprite.color = slowFallColor;

                useSlowFallLogic = true;
                break;

            case HeadEffectType.HighJump:
                duration = highJumpDuration;
                if (playerSprite != null)
                    playerSprite.color = highJumpColor;

                // Buff future jumps (not current frame)
                playerMovement.jumpForce = boostedJumpForce;
                break;
        }

        if (useSlowFallLogic)
        {
            // Only change gravity while falling, for duration
            float timer = 0f;
            while (timer < duration)
            {
                if (playerRb.linearVelocity.y < 0f)
                {
                    playerMovement.gravityScale = slowFallGravityScale;
                }
                else
                {
                    playerMovement.gravityScale = baseGravityScale;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            if (duration > 0f)
                yield return new WaitForSeconds(duration);
        }

        // Effect finished: go back to base stats
        ResetToBaseStats();
        activeEffect = null;
    }

    private void ResetToBaseStats()
    {
        if (playerSprite != null)
            playerSprite.color = baseColor;

        if (playerMovement != null)
        {
            playerMovement.moveSpeed = baseMoveSpeed;
            playerMovement.gravityScale = baseGravityScale;
            playerMovement.jumpForce = baseJumpForce;
        }
    }
}