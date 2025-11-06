using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Assigned automatically if not set
    private SpriteRenderer spriteRenderer;

    [Header("Enemy Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Damage Feedback")]
    public float flashDuration = 0.1f;
    public Color damageFlashColor = Color.red;
    private Color originalColor;

    protected virtual void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        currentHealth = maxHealth;

    }


    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (spriteRenderer != null)
            StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
            Die();

    }

    protected virtual void Die()
    {
        // TODO: Add sound on death
        Destroy(gameObject);

    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }

    }
}
