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

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/login.php", form))
        {
            yield return www.SendWebRequest();
            string response = www.downloadHandler.text;

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network Connection Error: " + www.error);
            }
            else
            {
                if (response[0] == '1')
                {
                    Debug.Log("Account logged in successfully");
                    DBManager.username = nameField.text;
                    DBManager.score = int.Parse(response.Split('\t')[1]);
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
