/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    public Transform Player => player;

    [Header("Camera Settings")]
    [SerializeField] private float followSpeed = 250f;
    [SerializeField] private float movementThreshold = 0.1f; // how far player must move before follow starts

    [Header("Level Boundaries (world units)")]
    [SerializeField] private float minX = 0f; // left edge of level/map
    [SerializeField] private float maxX = 2600f; // right edge of level/map
    [SerializeField] private float minY = 0f;   // bottom edge of level/map
    [SerializeField] private float maxY = 500f; // top edge of level/map (adjust to your level)

    private Camera cam;
    private bool isFollowing;
    private Vector3 startCamPos;
    private float startPlayerX;
    private float startPlayerY;
    private float leftLimit; // min camera center X allowed
    private float rightLimit; // max camera center X allowed
    private float bottomLimit; // min camera center Y allowed
    private float topLimit; // max camera center Y allowed

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        cam = GetComponent<Camera>();

        // Compute half-width of camera in map units
        float halfWidth = cam.orthographicSize * cam.aspect;
        float halfHeight = cam.orthographicSize;

        // Clamp limits to camera CENTER, not map edges
        leftLimit = minX + halfWidth;
        rightLimit = maxX - halfWidth;

        // Vertical limits based on camera height
        bottomLimit = minY + halfHeight;
        topLimit = maxY - halfHeight;

        // If level is narrower than camera width, keep camera centered in the level span
        if (leftLimit > rightLimit)
        {
            float mid = (minX + maxX) * 0.5f;
            leftLimit = rightLimit = mid;
        }

        // If level is shorter than camera height, keep camera centered in the level span
        if (bottomLimit > topLimit)
        {
            float midY = (minY + maxY) * 0.5f;
            bottomLimit = topLimit = midY;
        }

        // Record starting positions
        startCamPos = transform.position;
        if (player != null)
        {
            startPlayerX = player.position.x;
            startPlayerY = player.position.y;
        }

        // Force camera X to start within limits
        startCamPos.x = Mathf.Clamp(startCamPos.x, leftLimit, rightLimit);
        // Force camera Y to start within limits
        startCamPos.y = Mathf.Clamp(startCamPos.y, bottomLimit, topLimit);
        transform.position = startCamPos;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Do not move until the player actually moves a bit
        if (!isFollowing)
        {
            float dx = Mathf.Abs(player.position.x - startPlayerX);
            float dy = Mathf.Abs(player.position.y - startPlayerY);

            if (dx > movementThreshold || dy > movementThreshold)
                isFollowing = true;
        }

        if (!isFollowing)
        {
            // Stay locked at the start
            transform.position = startCamPos;
            return;
        }

        // Desired camera center follows the player X
        float desiredCamX = player.position.x;
        float desiredCamY = player.position.y;

        // Clamp camera center to limits so it never goes past minX/maxX on-screen
        desiredCamX = Mathf.Clamp(desiredCamX, leftLimit, rightLimit);
        desiredCamY = Mathf.Clamp(desiredCamY, bottomLimit, topLimit);

        // Smooth movement
        Vector3 targetPos = new Vector3(desiredCamX, desiredCamY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}