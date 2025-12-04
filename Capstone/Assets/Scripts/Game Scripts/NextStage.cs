using UnityEngine;

public class NextStage : MonoBehaviour
{
    public string playerTag = "Player";
    public float loadDelay = 0.5f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        triggered = true;

        // Optional: freeze player movement so they don't walk during transition
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        StartCoroutine(NextStageRoutine());
    }

    private System.Collections.IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(loadDelay);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextStage();
        }
        else
        {
            Debug.LogWarning("NextStage: GameManager.Instance is null.");
        }
    }
}