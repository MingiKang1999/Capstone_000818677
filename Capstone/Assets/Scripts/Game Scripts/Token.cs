using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    public int tokenValue = 1; // how many tokens this pickup is worth

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Add to token count via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddToken(tokenValue);
            }

            Destroy(gameObject);
        }
    }
}