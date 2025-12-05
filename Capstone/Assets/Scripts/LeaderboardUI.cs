/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform rowsParent;      // Scroll View
    public GameObject rowPrefab;      // LeaderboardRow prefab
    public Text statusText;           // Error message
    public InputField searchInput;    // InputField where user types the name to search

    private const string LEADERBOARD_URL = "https://antiquewhite-hippopotamus-744274.hostingersite.com/sqlconnect/leaderboard.php";

    [System.Serializable]
    public class PlayerEntry
    {
        public string username;
        public string time;  // format "HH:MM:SS"
    }

    [System.Serializable]
    public class PlayerEntryArray
    {
        public PlayerEntry[] players;
    }

    // All players loaded from PHP (master list)
    private List<PlayerEntry> allPlayers = new List<PlayerEntry>();
    // Players currently displayed (after sort/search)
    private List<PlayerEntry> displayedPlayers = new List<PlayerEntry>();

    // Sort direction toggles
    private bool usernameAscending = true;
    private bool timeAscending = true;

    private void Start()
    {
        if (statusText != null)
            statusText.text = "Loading leaderboard...";

        StartCoroutine(LoadLeaderboard());
    }

    private IEnumerator LoadLeaderboard()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(LEADERBOARD_URL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Leaderboard error: " + www.error);
                if (statusText != null)
                    statusText.text = "Error loading leaderboard";
                yield break;
            }

            string json = www.downloadHandler.text;
            Debug.Log("Raw leaderboard JSON: " + json);

            // Wrap raw array so JsonUtility can parse it
            string wrappedJson = "{\"players\":" + json + "}";
            PlayerEntryArray data = JsonUtility.FromJson<PlayerEntryArray>(wrappedJson);

            if (data == null || data.players == null)
            {
                Debug.LogError("Failed to parse leaderboard JSON");
                if (statusText != null)
                    statusText.text = "Failed to parse leaderboard data";
                yield break;
            }

            // Fill master list and displayed list
            allPlayers.Clear();
            allPlayers.AddRange(data.players);

            displayedPlayers.Clear();
            displayedPlayers.AddRange(allPlayers);

            // Default sort (e.g., username ascending)
            usernameAscending = true;
            SortByUsername();  // will sort & RefreshUI()

            if (statusText != null)
                statusText.text = "";
        }
    }

    // -------------------------
    // Time helper
    // -------------------------

    // Convert "HH:MM:SS" to total seconds for comparison
    private int TimeToSeconds(string time)
    {
        if (string.IsNullOrEmpty(time))
            return int.MaxValue; // treat invalid as very slow

        string[] parts = time.Split(':');
        if (parts.Length != 3)
            return int.MaxValue;

        int h, m, s;
        if (!int.TryParse(parts[0], out h)) h = 0;
        if (!int.TryParse(parts[1], out m)) m = 0;
        if (!int.TryParse(parts[2], out s)) s = 0;

        return h * 3600 + m * 60 + s;
    }

    // -------------------------
    // Public sorting methods (hook these to buttons)
    // -------------------------

    public void SortByUsername()
    {
        if (displayedPlayers == null || displayedPlayers.Count == 0)
            return;

        if (usernameAscending)
        {
            // A -> Z
            displayedPlayers.Sort((a, b) => a.username.CompareTo(b.username));
        }
        else
        {
            // Z -> A
            displayedPlayers.Sort((a, b) => b.username.CompareTo(a.username));
        }

        usernameAscending = !usernameAscending; // toggle for next click
        RefreshUI();
    }

    public void SortByTime()
    {
        if (displayedPlayers == null || displayedPlayers.Count == 0)
            return;

        if (timeAscending)
        {
            // fastest first (smallest seconds)
            displayedPlayers.Sort((a, b) =>
                TimeToSeconds(a.time).CompareTo(TimeToSeconds(b.time))
            );
        }
        else
        {
            // slowest first (largest seconds)
            displayedPlayers.Sort((a, b) =>
                TimeToSeconds(b.time).CompareTo(TimeToSeconds(a.time))
            );
        }

        timeAscending = !timeAscending; // toggle for next click
        RefreshUI();
    }

    // -------------------------
    // Search by name
    // -------------------------

    public void SearchByName()
    {
        if (allPlayers == null || allPlayers.Count == 0)
            return;

        string term = searchInput != null ? searchInput.text.Trim() : "";

        if (string.IsNullOrEmpty(term))
        {
            // Empty search -> show all players again
            displayedPlayers.Clear();
            displayedPlayers.AddRange(allPlayers);

            if (statusText != null)
                statusText.text = "Showing all players";

            RefreshUI();
            return;
        }

        // Case-insensitive "contains" search
        List<PlayerEntry> results = new List<PlayerEntry>();
        string lowerTerm = term.ToLower();

        foreach (var p in allPlayers)
        {
            if (!string.IsNullOrEmpty(p.username) &&
                p.username.ToLower().Contains(lowerTerm))
            {
                results.Add(p);
            }
        }

        if (results.Count == 0)
        {
            // No matches
            displayedPlayers.Clear();
            RefreshUI(); // clears the list visually

            if (statusText != null)
                statusText.text = "No user found";
        }
        else
        {
            displayedPlayers = results;

            if (statusText != null)
                statusText.text = "Found " + results.Count + " result(s)";

            // Optionally, keep last sort mode? For now, just refresh as-is
            RefreshUI();
        }
    }

    // -------------------------
    // UI Rebuild
    // -------------------------
    private void RefreshUI()
    {
        // Clear old rows
        foreach (Transform child in rowsParent)
        {
            Destroy(child.gameObject);
        }

        // Spawn a row for each displayed player
        for (int i = 0; i < displayedPlayers.Count; i++)
        {
            PlayerEntry p = displayedPlayers[i];

            GameObject rowObj = Instantiate(rowPrefab, rowsParent);
            LeaderboardRow row = rowObj.GetComponent<LeaderboardRow>();

            if (row != null)
            {
                row.SetData(i + 1, p.username, p.time);
            }
            else
            {
                Debug.LogError("LeaderboardRow component missing on row prefab!");
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}