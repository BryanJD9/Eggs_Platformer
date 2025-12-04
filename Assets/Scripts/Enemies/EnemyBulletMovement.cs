using UnityEngine;

public class EnemyBulletMovement : MonoBehaviour
{
    public float speed = 4f;
    public float lifeTime = 3f;

    private Vector2 direction;

    [HideInInspector] public bool ignoredByPlayer = false;

    public void SetDirection(Vector2 dir) => direction = dir.normalized;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            ignoredByPlayer = true; // mark to avoid hitting player
            Destroy(gameObject);
        }

        // Player damage handled in PlayerController
    }
}
