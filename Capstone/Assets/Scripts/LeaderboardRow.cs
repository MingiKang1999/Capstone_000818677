/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
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