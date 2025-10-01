using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float lifetime = 0.2f; // how long the hitbox exists
    [HideInInspector] public Vector2 attackDir; // direction set by PlayerController

    private PlayerController player;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        player = GetComponentInParent<PlayerController>(); // player is parent
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("BounceObject"))
        {
            Debug.Log("Hit enemy: " + other.name);
            // TODO: damage enemy here

            // Bounce if this was a downwards attack
            if (attackDir == Vector2.down && player != null)
            {
                player.BounceFromDownAttack();
            }
        }
    }

}
