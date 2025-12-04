using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    public Text rankText;
    public Text usernameText;
    public Text timeText;

    public void SetData(int rank, string username, string time)
    {
        rankText.text = rank.ToString() + ".";
        usernameText.text = username;
        timeText.text = time;
    }
}