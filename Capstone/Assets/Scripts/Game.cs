using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text playerNameDisplay;
    public Text scoreDisplay;

    private void Awake()
    {
        if (DBManager.username == null)
        {
            //SceneManager.LoadScene(0);
        }
        //playerNameDisplay.text = "Player: " + DBManager.username;
        //scoreDisplay.text = "Score: " + DBManager.score;
    }

    public void CallSaveData()
    {
        StartCoroutine(SavePlayerData());
    }

    IEnumerator SavePlayerData()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("score", DBManager.score);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/scorecount.php", form))
        {
            yield return www.SendWebRequest();
            string response = www.downloadHandler.text;
            
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Nework Connection Error: " + www.error);
            }
            else
            {
                if (response == "1")
                {
                    Debug.Log("Game saved");

                    DBManager.LogOut();
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
                else
                {
                    Debug.Log("Save Failed: " + response);
                }
            }
        }
    }

    public void IncreaseScore()
    {
        DBManager.score++;
        scoreDisplay.text = "Player Score: " + DBManager.score;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
