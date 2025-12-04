using UnityEngine;
using UnityEngine.UI;

public class FinalTokenDisplay : MonoBehaviour
{
    public Text tokenText;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            tokenText.text = "TOKENS: " + GameManager.Instance.tokensCollected;
        }
        else
        {
            tokenText.text = "TOKENS: 0";
        }
    }
}