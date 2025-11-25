using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button registerButton;
    public Button loginButton;
    public Button startButton;

    public Text playerNameDisplay;

    private void Start()
    {
        SceneManager.LoadScene(4);
        if (DBManager.LoggedIn)
        {
            playerNameDisplay.text = "Player: " + DBManager.username;
        }
        registerButton.interactable = !DBManager.LoggedIn;
        loginButton.interactable = !DBManager.LoggedIn;
        startButton.interactable = DBManager.LoggedIn;
    }

    public void GoToRegister()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene(2);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(3);
    }

    public void GoToRealGame()
    {
        SceneManager.LoadScene(4);
    }
}
