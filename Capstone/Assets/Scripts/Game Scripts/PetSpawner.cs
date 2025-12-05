/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;

[RequireComponent(typeof(SideScrolling))]
public class PetSpawner : MonoBehaviour
{
    public GameObject petPrefab;
    public Vector2 spawnOffset = new Vector2(0f, 0f);

    private void Start()
    {
        if (!DBManager.LoggedIn)
            return;

        if (DBManager.pet <= 0)
        {
            Debug.Log("No pet equipped, not spawning pet.");
            return;
        }

        SideScrolling camFollow = GetComponent<SideScrolling>();
        Transform player = camFollow.Player;

        if (player == null || petPrefab == null)
        {
            Debug.LogWarning("PetSpawner: missing player or petPrefab.");
            return;
        }

        Vector3 spawnPos = player.position + (Vector3)spawnOffset;
        GameObject pet = Instantiate(petPrefab, spawnPos, Quaternion.identity);

        // Assign follow target
        PetFollower follower = pet.GetComponent<PetFollower>();
        if (follower != null)
        {
            follower.target = player;
        }
    }
}