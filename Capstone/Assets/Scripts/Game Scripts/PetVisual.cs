/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PetVisual : MonoBehaviour
{
    [Tooltip("Index = petId (0 = none, 1 = dragon, 2 = cat, 3 = silme)")]
    public Sprite[] petSprites;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        int petId = DBManager.pet;   // which pet is equipped

        // No pet or out-of-range hide this object
        if (petId <= 0 || petSprites == null || petId >= petSprites.Length || petSprites[petId] == null)
        {
            gameObject.SetActive(false);
            return;
        }

        sr.sprite = petSprites[petId];
    }
}