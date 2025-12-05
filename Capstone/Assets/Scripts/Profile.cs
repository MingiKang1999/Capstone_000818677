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

public class Profile : MonoBehaviour
{
    [Header("Existing Fields")]
    public InputField passwordField;          // used for DELETE account
    public InputField newNameField;

    public Button nameChangeButton;
    public Button deleteAccountButton;

    public Text nameChangeText;

    [Header("Change Password UI")]
    public InputField oldPasswordField;       // user enters current password
    public InputField newPasswordField;       // user enters new password
    public Button passwordChangeButton;       // "Change Password" button
    public Text passwordChangeText;           // status text for password change

    private void Awake()
    {
        if (DBManager.username == null)
        {
            SceneManager.LoadScene(0);
        }
    }

    //   NAME CHANGE
    public void CallPlayerNameChange()
    {
        StartCoroutine(PlayerNameChange());
    }

    IEnumerator PlayerNameChange()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("newUsername", newNameField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.ServerBaseUrl + "usernamechange.php", form))
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
                    nameChangeText.text = "Name change failed: " + response;
                }
            }
        }
    }

    //   ACCOUNT DELETION
    public void CallAccountDeletion()
    {
        StartCoroutine(AccountDeletion());
    }

    IEnumerator AccountDeletion()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("password", passwordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.ServerBaseUrl + "delete.php", form))
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
                    Debug.Log("Account Deletion Failed. Error: " + response);
                }
            }
        }
    }

    //   PASSWORD CHANGE
    public void CallPasswordChange()
    {
        StartCoroutine(PasswordChange());
    }

    IEnumerator PasswordChange()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("oldPassword", oldPasswordField.text);
        form.AddField("newPassword", newPasswordField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.ServerBaseUrl + "passwordchange.php", form))
        {
            yield return www.SendWebRequest();
            string response = www.downloadHandler.text;

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network Connection Error: " + www.error);
                if (passwordChangeText != null)
                    passwordChangeText.text = "Network error: " + www.error;
            }
            else
            {
                Debug.Log("Password change response: " + response);

                if (response.Length > 0 && response[0] == '1')
                {
                    // convention: "1" = success
                    if (passwordChangeText != null)
                        passwordChangeText.text = "Password changed successfully.";
                }
                else
                {
                    if (passwordChangeText != null)
                        passwordChangeText.text = "Password change failed: " + response;
                }
            }
        }
    }

    //   NAVIGATION
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //   VALIDATION
    public void VerifyInputsName()
    {
        nameChangeButton.interactable = (newNameField.text.Length >= 8);
    }

    public void VerifyInputsPassword()
    {
        deleteAccountButton.interactable = (passwordField.text.Length >= 8);
    }

    // Enable password change button only when both fields are long enough
    public void VerifyInputsPasswordChange()
    {
        bool valid = oldPasswordField.text.Length >= 8 && newPasswordField.text.Length >= 8;
        passwordChangeButton.interactable = valid;
    }
}