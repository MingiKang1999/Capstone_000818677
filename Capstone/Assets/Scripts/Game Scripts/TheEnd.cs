/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheEnd : MonoBehaviour
{
    public void GoToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            // Save results to DB
            GameManager.Instance.CallSaveFinalResults();

            // Reset stage/time/tokens for the next run
            GameManager.Instance.ResetRunState();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }

        StartCoroutine(ReturnToMenuAfterSave());
    }

    private IEnumerator ReturnToMenuAfterSave()
    {
        // Small delay so the web request can be fired
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(0);
    }
}
