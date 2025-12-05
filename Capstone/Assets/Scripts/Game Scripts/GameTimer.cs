/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text timerText;
    public bool countUp = true;
    public float timeElapsed = 0f;
    private bool isRunning = true;

    private void Start()
    {
        // If GameManager exists, restore saved time
        if (GameManager.Instance != null)
        {
            timeElapsed = GameManager.Instance.savedRunTime;
        }
    }

    private void Update()
    {
        if (!isRunning)
            return;

        timeElapsed += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        if (timerText != null)
            timerText.text = string.Format("TIME: {0:00}:{1:00}", minutes, seconds);

        // Stop increasing time after final stage
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.world == 4 && GameManager.Instance.stage == 1)
            {
                isRunning = false;
            }
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        isRunning = true;
        timeElapsed = 0f;
    }
}