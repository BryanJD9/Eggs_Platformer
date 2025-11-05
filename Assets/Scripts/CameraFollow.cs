using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    [SerializeField] private Transform player;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;     // How fast camera catches up
    [SerializeField] private Vector2 followOffset = new Vector2(2f, 1f);  // Distance before camera starts moving
    [SerializeField] private bool followY = true;        // Optionally follow vertical movement

    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (player == null)
            return;

        // Get current camera position and target offset
        Vector3 currentPos = transform.position;
        Vector3 playerPos = player.position;

        // Horizontal dead zone
        float xDiff = playerPos.x - currentPos.x;
        if (Mathf.Abs(xDiff) > followOffset.x)
        {
            currentPos.x = Mathf.Lerp(currentPos.x, playerPos.x - Mathf.Sign(xDiff) * followOffset.x, Time.deltaTime * smoothSpeed);
        }

        // Vertical dead zone
        if (followY)
        {
            float yDiff = playerPos.y - currentPos.y;
            if (Mathf.Abs(yDiff) > followOffset.y)
            {
                currentPos.y = Mathf.Lerp(currentPos.y, playerPos.y - Mathf.Sign(yDiff) * followOffset.y, Time.deltaTime * smoothSpeed);
            }
        }

        // keep z unchanged
        transform.position = new Vector3(currentPos.x, currentPos.y, transform.position.z);
    }
}
