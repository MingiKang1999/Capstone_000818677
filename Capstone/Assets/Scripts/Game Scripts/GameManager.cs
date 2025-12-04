using UnityEngine;
using UnityEngine.SceneManagement;

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
        GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            timer.ResetTimer();
        }

        LoadLevel(startingWorld, startingStage);  // this is your "1-1"
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

        // Check if we are about to enter FINAL STAGE (4-1)
        if (stage == 1 && world == 4)
        {
            Debug.Log("Final stage reached! Stopping timer...");

            if (timer != null)
            {
                timer.StopTimer();
                runTime = timer.timeElapsed; // save final time
            }

            return; // Do NOT load a new stage anymore
        }

        if (stage == 4)
        {
            LoadLevel(world + 1, 1);
            lives = startingLives;
        }
        else
        {
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
}