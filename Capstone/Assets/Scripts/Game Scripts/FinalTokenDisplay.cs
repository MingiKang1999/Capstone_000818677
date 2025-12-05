/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
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