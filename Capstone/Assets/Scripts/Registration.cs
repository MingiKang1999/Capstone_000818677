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

public class Registration : MonoBehaviour
{
    public InputField nameField;
    public InputField passwordField;

    public Button submitButton;

    public Text ErrorDisplay;

    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", nameField.text);
        form.AddField("password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.ServerBaseUrl + "register.php", form))
        {
            yield return www.SendWebRequest();
            string response = www.downloadHandler.text;

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network Connection Error: " + www.error);
            }
            else
            {
                if (response == "1")
                {
                    Debug.Log("Account created successfully");
                    SceneManager.LoadScene(0);
                }
                else
                {
                    Debug.Log("Account creation failed. Error: " + response);
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
