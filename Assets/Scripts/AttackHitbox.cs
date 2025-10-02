using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float lifetime = 0.2f;
    [HideInInspector] public Vector2 attackDir;

    private PlayerController player;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("BounceObject"))
        {
            Debug.Log("Hit enemy: " + other.name);
            // TODO: damage enemy here

            // Bounce if attacking downward
            if (attackDir.y < -0.5f && player != null)
            {
                player.BounceFromDownAttack();
                Debug.Log("Downward hit");
            }

        }

        // Destroy enemy bullets if hit by attack
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Debug.Log("Destroyed enemy bullet!");
        }

    }



}
