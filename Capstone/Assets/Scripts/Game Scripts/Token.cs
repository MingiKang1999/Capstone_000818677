/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
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