using UnityEngine;

public class EnemyBulletMovement : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 3f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy bullet after lifetime expires
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Example: damage player
            Debug.Log("Player hit by enemy bullet!");
            Destroy(gameObject);
        }
    }

}