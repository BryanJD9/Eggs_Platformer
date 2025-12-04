using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerControls controls;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private Vector2 attackInput;
    public bool isGrounded;

    [Header("Player Attributes")]
    public int maxHealth = 3;
    public int currentHealth;
    public static Action<int> OnHealthChanged;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 15f;
    public float bounceForce = 7.5f;

    [Header("Jump Settings")]
    public int maxJumps = 2;
    public int jumpsRemaining;

    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public float attackOffset = 1f;
    private Vector2 facingDirection = Vector2.right;

    [Header("Damage & Knockback")]
    public float knockbackForce = 10f;
    public float knockbackUpwardForce = 4f;
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Movement.Jump.performed += ctx => Jump();
        jumpsRemaining = maxJumps;

        controls.Movement.AttackDirection.performed += ctx => attackInput = ctx.ReadValue<Vector2>();
        controls.Movement.AttackDirection.canceled += ctx => attackInput = Vector2.zero;

        controls.Movement.Attack.performed += ctx => Attack();

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
            GameOverManager.Instance?.GameOver();
        }
        else
        {
            StartCoroutine(InvincibilityFlash());
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void OnEnable() => controls.Movement.Enable();
    private void OnDisable() => controls.Movement.Disable();

    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (moveInput.x > 0.01f) spriteRenderer.flipX = false;
        if (moveInput.x < -0.01f) spriteRenderer.flipX = true;

        if (moveInput.x > 0.1f)
            facingDirection = Vector2.right;
        else if (moveInput.x < -0.1f)
            facingDirection = Vector2.left;
    }

    #region Movement
    private void Jump()
    {
        bool holdingDown = attackInput.y < -0.1f;

        if (holdingDown && isGrounded)
        {
            StartCoroutine(FallThroughPlatform());
            return;
        }

        if (jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }
    }

    private IEnumerator FallThroughPlatform()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                                       LayerMask.NameToLayer("Platform"), true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),
                                       LayerMask.NameToLayer("Platform"), false);
    }

    private void resetJumps()
    {
        isGrounded = true;
        jumpsRemaining = maxJumps;
    }
    #endregion

    #region Attack
    private void Attack()
    {
        Vector2 attackDir = facingDirection;

        if (attackInput.y > 0.1f) attackDir = Vector2.up;
        else if (attackInput.y < -0.1f) attackDir = Vector2.down;

        Vector2 spawnPos = (Vector2)transform.position + attackDir * attackOffset;

        GameObject hitbox = Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        hitbox.transform.SetParent(transform);
        hitbox.transform.right = attackDir;

        var hitboxScript = hitbox.GetComponent<AttackHitbox>();
        if (hitboxScript != null)
            hitboxScript.attackDir = attackDir;
    }

    public void BounceFromDownAttack()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsRemaining = maxJumps;
        isGrounded = false;
    }
    #endregion

    #region Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            resetJumps();
        }

        if ((collision.gameObject.CompareTag("Enemy") ||
             collision.gameObject.CompareTag("EnemyBullet") ||
             collision.gameObject.CompareTag("Spikes")) && !isInvincible)
        {
            TakeDamage(1);

            float knockDir = transform.position.x > collision.transform.position.x ? 1 : -1;
            rb.linearVelocity = Vector2.zero;
            Vector2 knockback = new Vector2(knockDir * knockbackForce, knockbackUpwardForce);
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore attack hitboxes
        if (other.GetComponent<AttackHitbox>() != null) return;

        if (other.CompareTag("EnemyBullet"))
        {
            EnemyBulletMovement bullet = other.GetComponent<EnemyBulletMovement>();
            if (bullet != null && !bullet.ignoredByPlayer)
            {
                TakeDamage(1);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Spikes") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            if (Mathf.Abs(rb.linearVelocity.y) > 0.01f)
                isGrounded = false;
        }
    }
    #endregion

    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}
