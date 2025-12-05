/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;

public class PetFollower : MonoBehaviour
{
    private void Start()
    {
        // Flip once (faces left)
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    [Header("Follow Settings")]
    public Transform target;                   // Player
    public Vector2 offset = new Vector2(-0.5f, 0.5f);
    public float followSpeed = 8f;             // can tweak

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + (Vector3)offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
    }
}