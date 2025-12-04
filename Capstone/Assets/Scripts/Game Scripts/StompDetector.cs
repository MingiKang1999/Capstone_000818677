using UnityEngine;

public class StompDetector : MonoBehaviour
{
    public float bounceForce = 200f;  // how high the player bounces after stomp
    private Rigidbody2D playerRb;

    private void Awake()
    {
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ONLY stomp if moving downward
        if (playerRb.linearVelocity.y > 0)
            return;

        // Check if this target is a boss
        BossHealth boss = collision.GetComponent<BossHealth>();
        if (boss != null)
        {
            BouncePlayer();

            bool isDead = boss.TakeHit();   // boss loses 1 HP

            if (isDead)
                KillEnemy(collision.gameObject);

            return;
        }

        // Normal enemy stomp
        if (collision.CompareTag("Enemy"))
        {
            BouncePlayer();
            KillEnemy(collision.gameObject);
        }
    }

    private void BouncePlayer()
    {
        Vector2 v = playerRb.linearVelocity;
        v.y = bounceForce;
        playerRb.linearVelocity = v;
    }

    private void KillEnemy(GameObject enemy)
    {
        // Disable enemy movement
        var move = enemy.GetComponent<EntityMovement>();
        if (move) move.enabled = false;

        // Make enemy red before dying
        var sr = enemy.GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.red;

        // Disable collider so player won't get stuck
        foreach (var col in enemy.GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // Destroy enemy after short delay
        Destroy(enemy, 0.5f);
    }
}