using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerControls controls;
    private SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;

    private Vector2 moveInput;
    public bool isGrounded;


    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public float attackOffset = 1.0f;

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

        controls.Movement.Attack.performed += ctx => Attack();


    }

    private void OnEnable()
    {
        controls.Movement.Enable();
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
    }

    private void Update()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on direction
        if (moveInput.x > 0.01f) spriteRenderer.flipX = false; // facing right
        if (moveInput.x < -0.01f) spriteRenderer.flipX = true;  // facing left

    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

    }

    private void Attack()
    {
        Vector2 attackDir = Vector2.zero;

        if (moveInput.x < -0.1f) attackDir = Vector2.left;
        else if (moveInput.x > 0.1f) attackDir = Vector2.right;
        else if (moveInput.y > 0.1f) attackDir = Vector2.up;
        else if (moveInput.y < -0.1f) attackDir = Vector2.down;
        else
            attackDir = Vector2.right; // default if standing still

        Vector2 spawnPos = (Vector2)transform.position + attackDir * attackOffset;

        GameObject hitbox = Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        hitbox.transform.SetParent(transform); // follows the player
        hitbox.transform.right = attackDir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Remember to tag scene objects "Ground" as needed
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

}
