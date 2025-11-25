using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private float followSpeed = 250f;
    [SerializeField] private float movementThreshold = 0.1f; // how far player must move before follow starts

    [Header("Level Boundaries (world units)")]
    [SerializeField] private float minX = 0f; // left edge of level/map
    [SerializeField] private float maxX = 2600f; // right edge of level/map

    private Camera cam;
    private bool isFollowing;
    private Vector3 startCamPos;
    private float startPlayerX;
    private float leftLimit; // min camera center X allowed
    private float rightLimit; // max camera center X allowed

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        cam = GetComponent<Camera>();

        // Compute half-width of camera in map units
        float halfWidth = cam.orthographicSize * cam.aspect;

        // Clamp limits to camera CENTER, not map edges
        leftLimit = minX + halfWidth;
        rightLimit = maxX - halfWidth;

        // If level is narrower than camera width, keep camera centered in the level span
        if (leftLimit > rightLimit)
        {
            float mid = (minX + maxX) * 0.5f;
            leftLimit = rightLimit = mid;
        }

        // Record starting positions
        startCamPos = transform.position;
        startPlayerX = player != null ? player.position.x : 0f;

        // Force camera X to start within limits
        startCamPos.x = Mathf.Clamp(startCamPos.x, leftLimit, rightLimit);
        transform.position = startCamPos;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Do not move until the player actually moves a bit
        if (!isFollowing && Mathf.Abs(player.position.x - startPlayerX) > movementThreshold)
            isFollowing = true;

        if (!isFollowing)
        {
            // Stay locked at the start
            transform.position = startCamPos;
            return;
        }

        // Desired camera center follows the player X
        float desiredCamX = player.position.x;

        // Clamp camera center to limits so it never goes past minX/maxX on-screen
        desiredCamX = Mathf.Clamp(desiredCamX, leftLimit, rightLimit);

        // Smooth movement
        Vector3 targetPos = new Vector3(desiredCamX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
