using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    [SerializeField] private Transform player;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;     // How fast camera catches up
    [SerializeField] private Vector2 followOffset = new Vector2(2f, 1f);  // Dead zone before camera moves
    [SerializeField] private bool followY = true;        // Optionally follow vertical movement

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;      // Toggle bounds on/off
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 currentPos = transform.position;
        Vector3 playerPos = player.position;

        // Horizontal dead zone
        float xDiff = playerPos.x - currentPos.x;
        if (Mathf.Abs(xDiff) > followOffset.x)
        {
            currentPos.x = Mathf.Lerp(currentPos.x, playerPos.x - Mathf.Sign(xDiff) * followOffset.x, Time.deltaTime * smoothSpeed);
        }

        // Vertical dead zone (optional)
        if (followY)
        {
            float yDiff = playerPos.y - currentPos.y;
            if (Mathf.Abs(yDiff) > followOffset.y)
            {
                currentPos.y = Mathf.Lerp(currentPos.y, playerPos.y - Mathf.Sign(yDiff) * followOffset.y, Time.deltaTime * smoothSpeed);
            }
        }

        // Apply bounds if enabled
        if (useBounds)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            // Clamp camera edges instead of center
            currentPos.x = Mathf.Clamp(currentPos.x, minX + camWidth, maxX - camWidth);
            currentPos.y = Mathf.Clamp(currentPos.y, minY + camHeight, maxY - camHeight);
        }

        // Apply position (keep z unchanged)
        transform.position = new Vector3(currentPos.x, currentPos.y, transform.position.z);
    }

    // Optional: draw the bounds in Scene View for convenience
    private void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
            Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
            Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
            Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));
        }
    }
}
