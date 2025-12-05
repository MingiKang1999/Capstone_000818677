/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PetStore : MonoBehaviour
{
    [System.Serializable]
    public class PetOption
    {
        public string displayName;   // e.g. "Slime"
        public int petId;            // matches DB 'pet' value
        public int cost;             // how many tokens
        public Button buyButton;     // UI button
        public Text buttonText;      // text on the button
    }

    public Text scoreText;          // "Tokens: X"
    public Text statusText;         // info / errors
    public PetOption[] pets;        // list of pets

    private void Start()
    {
        if (!DBManager.LoggedIn)
        {
            // back to login if somehow not logged in
            SceneManager.LoadScene(0);
            return;
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = "Tokens: " + DBManager.score;

        foreach (var pet in pets)
        {
            if (pet == null) continue;

            // Set button label
            if (pet.buttonText != null)
            {
                pet.buttonText.text = $"{pet.displayName}\nCost: {pet.cost}";
            }

            if (pet.buyButton != null)
            {
                int id = pet.petId;
                int cost = pet.cost;

                pet.buyButton.onClick.RemoveAllListeners();
                pet.buyButton.onClick.AddListener(() => OnBuyPet(id, cost));

                // Disable button if not enough tokens
                pet.buyButton.interactable = (DBManager.score >= cost);
            }
        }

        if (statusText != null)
            statusText.text = "Current pet ID: " + DBManager.pet;
    }

    private void OnBuyPet(int petId, int cost)
    {
        if (DBManager.score < cost)
        {
            if (statusText != null)
                statusText.text = "Not enough tokens!";
            return;
        }

        StartCoroutine(BuyPetCoroutine(petId, cost));
    }

    private IEnumerator BuyPetCoroutine(int petId, int cost)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("petId", petId);
        form.AddField("cost", cost);

        using (UnityWebRequest www = UnityWebRequest.Post(DBManager.ServerBaseUrl + "petstore.php", form))
        {
            yield return www.SendWebRequest();
            string response = www.downloadHandler.text;
            Debug.Log("Pet store response: " + response);

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (statusText != null)
                    statusText.text = "Network error: " + www.error;
            }
            else
            {
                // success format: "1\tnewScore\tpetId"
                if (!string.IsNullOrEmpty(response) && response[0] == '1')
                {
                    string[] parts = response.Split('\t');
                    if (parts.Length >= 3)
                    {
                        int newScore;
                        int newPetId;
                        int.TryParse(parts[1], out newScore);
                        int.TryParse(parts[2], out newPetId);

                        DBManager.score = newScore;
                        DBManager.pet = newPetId;

                        if (statusText != null)
                            statusText.text = "Purchased pet ID " + newPetId + "!";

                        RefreshUI();
                    }
                }
                else
                {
                    if (statusText != null)
                        statusText.text = "Purchase failed: " + response;
                }
            }
        }
    }

    // Optional: button to go back to main menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}