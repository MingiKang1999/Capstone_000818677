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
        if (collision.CompareTag("Enemy"))
        {
            // ONLY stomp if moving downward
            if (playerRb.linearVelocity.y <= 0)
            {
                // Bounce the player upward
                Vector2 v = playerRb.linearVelocity;
                v.y = bounceForce;
                playerRb.linearVelocity = v;

                // Disable enemy movement
                collision.GetComponent<EntityMovement>().enabled = false;

                // Make enemy red before dieing
                collision.GetComponent<SpriteRenderer>().color = Color.red;

                // Disable collider so player won't get stuck
                foreach (var col in collision.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = false;
                }

                // Destroy enemy after short delay
                Destroy(collision.gameObject, 0.5f);
            }
        }
    }
}