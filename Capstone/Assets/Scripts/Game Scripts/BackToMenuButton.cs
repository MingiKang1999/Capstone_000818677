/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    // Hook this to the Button's OnClick
    public void BackToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            // Just reset, do NOT send to database
            GameManager.Instance.ResetRunState();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null when trying to go to main menu.");
        }

        // Load main menu (scene index 0)
        SceneManager.LoadScene(0);
    }
}