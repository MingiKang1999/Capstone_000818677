using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    public InputField passwordField;
    public InputField newNameField;

    public Button nameChangeButton;
    public Button deleteAccountButton;

    public Text nameChangeText;

    private void Awake()
    {
        if (DBManager.username == null)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void CallPlayerNameChange()
    {
        StartCoroutine(PlayerNameChange());
    }

    public void CallAccountDeletion()
    {
        StartCoroutine(AccountDeletion());
    }

    IEnumerator PlayerNameChange()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("newUsername", newNameField.text);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/usernamechange.php", form))
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
                    Debug.Log("Account name changed successfully");
                    DBManager.username = newNameField.text;
                    nameChangeText.text = "Name Changed to " + newNameField.text;
                }
                else
                {
                    Debug.Log("User Name Change Failed. Error: " + response);
                }
            }
        }
    }

    IEnumerator AccountDeletion()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/delete.php", form))
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
                    Debug.Log("Account Deleted");
                    DBManager.LogOut();
                    SceneManager.LoadScene(0);
                }
                else
                {
                    Debug.Log("User Name Change Failed. Error: " + response);
                }
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void VerifyInputsName()
    {
        nameChangeButton.interactable = (newNameField.text.Length >= 8);
    }

    public void VerifyInputsPassword()
    {
        deleteAccountButton.interactable = (passwordField.text.Length >= 8);
    }
}
