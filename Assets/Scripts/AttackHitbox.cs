using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float lifetime = 0.2f; // how long the hitbox exists

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy: " + other.name);
            // TODO: damage enemy here
        }
    }

}
