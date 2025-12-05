/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    private SpriteRenderer sr;
    private Color originalColor;
    public float flashDuration = 0.15f;

    public GameObject portal; // assign in Inspector

    private void Awake()
    {
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    public bool TakeHit()
    {
        currentHealth = Mathf.Max(0, currentHealth - 1);

        if (sr != null)
            StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            // SHOW PORTAL
            if (portal != null)
                portal.SetActive(true);

            return true; // boss is dead
        }

        return false; // still alive
    }

    private System.Collections.IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
}