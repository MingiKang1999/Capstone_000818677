using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public Text worldStageText;
    public Text livesText;
    public Text tokenText;

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        int world = GameManager.Instance.world;
        int stage = GameManager.Instance.stage;
        int lives = GameManager.Instance.lives;
        int tokens = GameManager.Instance.tokensCollected;

        if (worldStageText != null)
            worldStageText.text = "STAGE " + world + "-" + stage;

        if (livesText != null)
            livesText.text = "LIVES   x" + lives;

        if (tokenText != null)
            tokenText.text = "TOKENS COLLECTED: " + tokens;
    }
}