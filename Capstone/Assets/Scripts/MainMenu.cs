/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button registerButton;
    public Button loginButton;
    public Button startButton;
    public Button gameButton;
    public Button profileButton;
    public Button storeButton;

    public Text playerNameDisplay;

    // Display Buttons as interactable depending on player status
    private void Start()
    {
        if (DBManager.LoggedIn)
        {
            playerNameDisplay.text = "Player: " + DBManager.username;
        }
        registerButton.interactable = !DBManager.LoggedIn;
        loginButton.interactable = !DBManager.LoggedIn;
        profileButton.interactable = DBManager.LoggedIn;
        gameButton.interactable = DBManager.LoggedIn;
        storeButton.interactable = DBManager.LoggedIn;
    }

    public void GoToRegister()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene(2);
    }

    // Used for Database connection test
    //public void GoToGame()
    //{
        //SceneManager.LoadScene(3);
    //}

    public void GoToProfile()
    {
        SceneManager.LoadScene(4);
    }

    public void GoToLeaderboard()
    {
        SceneManager.LoadScene(5);
    }

    public void GoToStore()
    {
        SceneManager.LoadScene(6);
    }

    public void GoToHowToPlay()
    {
        SceneManager.LoadScene(7);
    }

    public void GoToRealGame()
    {
        SceneManager.LoadScene(8);
    }
}
