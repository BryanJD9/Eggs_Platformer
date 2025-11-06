using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float lifetime = 0.2f;
    public int damage = 1;

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
            // Try to damage enemy
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

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
            player.BounceFromDownAttack();
            Destroy(other.gameObject);
            Debug.Log("Destroyed enemy bullet!");
        }

    }



}
