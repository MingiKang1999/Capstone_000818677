/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Current level
    public int world { get; private set; }
    public int stage { get; private set; }

    // Tokens Collected
    public int tokensCollected { get; private set; }

    // Lives
    public int lives { get; private set; }

    // Configurable starting values
    public int startingWorld = 1;
    public int startingStage = 1;
    public int startingLives = 3;

    // Game Timer
    public float savedRunTime = 0f;

    // saved final time
    public float runTime = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        lives = startingLives;
        savedRunTime = 0f; // New game, new run
        tokensCollected = 0; // New game, reset tokens

        // Player died 3 times -> reset timer
        GameTimer timer = UnityEngine.Object.FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            timer.ResetTimer();
        }

        LoadLevel(startingWorld, startingStage);  // this is your "1-1"
        Debug.Log(DBManager.username);
    }

    private void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;

        // Scene names like "1-1", "1-2"
        string sceneName = world + "-" + stage;
        SceneManager.LoadScene(sceneName);
    }

    public void NextStage()
    {
        // Save current timer before going to next stage
        GameTimer timer = FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            savedRunTime = timer.timeElapsed;
        }

        // NORMAL STAGE PROGRESSION (no final-save logic here anymore)

        if (stage == 4)
        {
            // Move to the next world, reset lives
            LoadLevel(world + 1, 1);
            lives = startingLives;
        }
        else
        {
            // Move to next stage in same world
            LoadLevel(world, stage + 1);
        }
    }

    public void ResetLevel(float delay)
    {
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;

        if (lives > 0)
        {
            // Save current timer before reloading the same stage
            GameTimer timer = FindFirstObjectByType<GameTimer>();
            if (timer != null)
            {
                savedRunTime = timer.timeElapsed;
            }
            // Retry current level
            LoadLevel(world, stage);
        }
        else
        {
            // Out of lives: reset run time and start over
            savedRunTime = 0f;
            // Token reset
            tokensCollected = 0;
            // Out of lives: back to starting world-stage (1-1)
            GameOver();
        }
    }

    private void GameOver()
    {
        NewGame();
    }

    public void AddToken(int amount = 1)
    {
        tokensCollected += amount;
        // Debug.Log("Tokens: " + tokensCollected);
    }

    public void ResetRunState()
    {
        // Reset level progress
        world = startingWorld;
        stage = startingStage;

        // Reset lives for next run
        lives = startingLives;

        // Reset timers
        savedRunTime = 0f;
        runTime = 0f;

        // Reset per-run tokens (DB already has lifetime total)
        tokensCollected = 0;

        Debug.Log("GameManager run state reset for new game.");
    }

    //   SAVE TO DATABASE
    public void CallSaveFinalResults()
    {
        // Assumes you already logged in and DBManager.username is set
        if (string.IsNullOrEmpty(DBManager.username))
        {
            Debug.LogError("No username set in DBManager. Cannot save run.");
            return;
        }

        StartCoroutine(SaveFinalResults());
    }

    private IEnumerator SaveFinalResults()
    {
        // Add to the local token counter
        DBManager.score += tokensCollected;
        // Convert runTime to a hh:mm:ss string for the DB TIME column
        int runTimeSeconds = Mathf.RoundToInt(savedRunTime);
        TimeSpan ts = TimeSpan.FromSeconds(runTimeSeconds);
        string runtimeString = ts.ToString(@"hh\:mm\:ss");

        Debug.Log($"Saving results for {DBManager.username}: time={runtimeString}, tokens={tokensCollected}");

        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("runtime", runtimeString);
        form.AddField("tokens", tokensCollected);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.ServerBaseUrl + "updaterun.php", form))
        {
            yield return www.SendWebRequest();
            string response = www.downloadHandler.text;

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network Connection Error: " + www.error);
            }
            else
            {
                if (response == "1")
                {
                    Debug.Log("Final run saved successfully");
                }
                else
                {
                    Debug.Log("Save Failed: " + response);
                }
            }
        }
    }
}