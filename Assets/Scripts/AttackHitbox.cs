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
        // Damage enemies
        if (other.CompareTag("Enemy") || other.CompareTag("BounceObject"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);

            if (attackDir.y < -0.5f && player != null)
                player.BounceFromDownAttack();
        }

        // Destroy enemy bullets
        if (other.CompareTag("EnemyBullet"))
        {
            EnemyBulletMovement bullet = other.GetComponent<EnemyBulletMovement>();
            if (bullet != null)
            {
                bullet.ignoredByPlayer = true; // prevent it from hitting player

                // Bounce if downward attack
                if (attackDir.y < -0.5f && player != null)
                {
                    player.BounceFromDownAttack();
                }

                Destroy(other.gameObject);
            }
        }
    }
}
