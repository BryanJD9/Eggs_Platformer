using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class WalkingEnemy : Enemy
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f; // How fast it walks toward the player

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Prevent physics rotation
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (!isActive || player == null)
        {
            // Stop moving if off-screen
            if (rb != null)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        Vector2 directionToPlayer = player.position - transform.position;

        // Move only along X-axis toward player
        float moveDir = Mathf.Sign(directionToPlayer.x);

        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on direction
        if (spriteRenderer != null)
            spriteRenderer.flipX = moveDir < 0;
    }
}
