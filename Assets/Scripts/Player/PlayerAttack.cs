using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackPrefab;   // Prefab with Collider2D + Trigger
    public float attackDuration = 0.2f;

    private Animator animator;
    private Vector2 lastMoveDir = Vector2.down; // Default facing down

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Track movement direction (so we know which way to attack)
        Vector2 moveInput = new Vector2(
            Keyboard.current.aKey.isPressed ? -1 :
            Keyboard.current.dKey.isPressed ? 1 : 0,

            Keyboard.current.wKey.isPressed ? 1 :
            Keyboard.current.sKey.isPressed ? -1 : 0
        );

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput.normalized;

        // Attack on key press
        if (Keyboard.current.jKey.wasPressedThisFrame)
            Attack();
    }

    private void Attack()
    {
        // Play attack animation in the correct direction
        animator.SetFloat("AttackX", lastMoveDir.x);
        animator.SetFloat("AttackY", lastMoveDir.y);
        animator.SetTrigger("Attack");

        // Spawn attack collider
        Vector3 spawnPos = transform.position + (Vector3)lastMoveDir;
        GameObject atk = Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        atk.transform.up = lastMoveDir; // rotate collider to face direction
        Destroy(atk, attackDuration);
    }
}
