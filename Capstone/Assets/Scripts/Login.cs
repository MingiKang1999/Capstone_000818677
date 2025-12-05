/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField nameField;
    public InputField passwordField;

    public Button submitButton;
    public Text ErrorDisplay;

    public void CallLogin()
    {
        StartCoroutine(LoginPlayer());
    }

    IEnumerator LoginPlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", nameField.text);
        form.AddField("password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.ServerBaseUrl + "login.php", form))
        {
            yield return www.SendWebRequest();
            string response = www.downloadHandler.text;
            Debug.Log("Login response: " + response);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network Connection Error: " + www.error);
            }
            else
            {
                if (!string.IsNullOrEmpty(response) && response[0] == '1')
                {
                    Debug.Log("Account logged in successfully");

                    // Expected format: "1\t<score>\t<pet>"
                    string[] data = response.Split('\t');

                    DBManager.username = nameField.text;

                    if (data.Length >= 2)
                        DBManager.score = int.Parse(data[1]);
                    else
                        DBManager.score = 0;

                    if (data.Length >= 3)
                        DBManager.pet = int.Parse(data[2]);
                    else
                        DBManager.pet = 0;

                    SceneManager.LoadScene(0);
                }
                else
                {
                    Debug.Log("User Login Failed. Error: " + response);
                    ErrorDisplay.text = response;
                }
            }
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}