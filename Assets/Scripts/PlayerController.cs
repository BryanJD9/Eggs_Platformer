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
    public int health;

    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 15.0f;
    public float bounceForce = 7.5f;

    [Header("Jump Settings")]
    public int maxJumps = 2;   // adjust for more jumps
    public int jumpsRemaining;

    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public float attackOffset = 1.0f;
    private Vector2 facingDirection = Vector2.right; // default facing right for attacks


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.freezeRotation = true; // Keep player upright, can toggle in inspector

        // Movement InputSystem
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Movement.Jump.performed += ctx => Jump();
        jumpsRemaining = maxJumps;

        controls.Movement.AttackDirection.performed += ctx => attackInput = ctx.ReadValue<Vector2>();
        controls.Movement.AttackDirection.canceled += ctx => attackInput = Vector2.zero;

        controls.Movement.Attack.performed += ctx => Attack();

        
    }

    #region InputSystems
    private void OnEnable()
    {
        controls.Movement.Enable();
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
    }
    #endregion

    private void Update()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on direction
        if (moveInput.x > 0.01f) spriteRenderer.flipX = false; // facing right
        if (moveInput.x < -0.01f) spriteRenderer.flipX = true;  // facing left

        // handle facing direction for attacks
        if (moveInput.x > 0.1f)
            facingDirection = Vector2.right;
        else if (moveInput.x < -0.1f)
            facingDirection = Vector2.left;

    }

    #region Movement Functions
    private void Jump()
    {
        // Check if holding down (fall through)
        bool holdingDown = attackInput.y < -0.1f;

        if (holdingDown && isGrounded)
        {
            // fall through platform instead of jumping
            StartCoroutine(FallThroughPlatform());
            return;
        }

        // double jump
        if (jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }

        //if (isGrounded)
        //{
        //    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        //}

    }

    private IEnumerator FallThroughPlatform() // Allow from jumping below platforms
    {
        // disable collisions between player layer and platform layer
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Platform"),
            true
        );

        yield return new WaitForSeconds(0.3f); // enough time to drop down

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Platform"),
            false
        );
    }

    private void resetJumps()
    {
        isGrounded = true;
        jumpsRemaining = maxJumps; // refresh jumps
    }

    #endregion

    #region Attack logic
    private void Attack()
    {
        Vector2 attackDir = facingDirection; // default left/right

        if (attackInput.y > 0.1f)
            attackDir = Vector2.up;
        else if (attackInput.y < -0.1f)
            attackDir = Vector2.down;

        Vector2 spawnPos = (Vector2)transform.position + attackDir * attackOffset;

        GameObject hitbox = Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        hitbox.transform.SetParent(transform);
        hitbox.transform.right = attackDir;

        // Tell hitbox its attack direction
        var hitboxScript = hitbox.GetComponent<AttackHitbox>();
        if (hitboxScript != null)
            hitboxScript.attackDir = attackDir;

        //Vector2 attackDir = facingDirection; // default left/right

        //if (attackInput.y > 0.1f)
        //    attackDir = Vector2.up;
        //else if (attackInput.y < -0.1f)
        //    attackDir = Vector2.down;

        //Vector2 spawnPos = (Vector2)transform.position + attackDir * attackOffset;

        //GameObject hitbox = Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        //hitbox.transform.SetParent(transform);
        //hitbox.transform.right = attackDir;

        
    }

    public void BounceFromDownAttack()
    {
        // Give upward velocity
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // Reset jumps as if grounded
        jumpsRemaining = maxJumps;
        isGrounded = false; // not grounded, but reset so double jump works again

    }


    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            resetJumps();

        }

        //hit by enemy bullet
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            //TODO: Add reaction to being hit by enemy
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            // Only un-ground if we're not basically standing still vertically
            if (Mathf.Abs(rb.linearVelocity.y) > 0.01f)
            {
                isGrounded = false;
            }
        }

    }



}
