using UnityEngine;

public class BasicShooter : Enemy
{
    [Header("References")]
    //public Transform player;               // Reference to player
    public Transform shootPoint;           // Where the bullets come from
    public GameObject bulletPrefab;        // Assign your bullet prefab

    [Header("Shooting Settings")]
    public float shootInterval = 2f;       // Time between shots
    [SerializeField] private bool shootInAnyDirection = false; // Toggle in inspector

    private SpriteRenderer spriteRenderer;
    private float shootTimer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Auto-find ShootPoint if not assigned
        if (shootPoint == null)
        {
            Transform found = transform.Find("ShootPoint");
            if (found != null)
                shootPoint = found;
            else
                Debug.LogWarning("ShootPoint not assigned and not found under Enemy!");
        }


        //// Auto-find shootPoint if not assigned
        //if (shootPoint == null)
        //{
        //    Transform found = transform.Find("ShootPoint");
        //    if (found != null)
        //        shootPoint = found;
        //    else
        //        Debug.LogWarning("ShootPoint not assigned and not found under Enemy!");
        //}

        
    }

    private void Update()
    {
        if (!isActive || player == null || shootPoint == null)
            return;

        // Face player (only left/right flip)
        if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        // Shooting logic
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }

    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        EnemyBulletMovement bulletScript = bullet.GetComponent<EnemyBulletMovement>();

        if (bulletScript != null)
        {
            if (shootInAnyDirection)
            {
                // Calculate normalized direction towards player
                Vector2 dirToPlayer = (player.position - shootPoint.position).normalized;
                bulletScript.SetDirection(dirToPlayer);
            }
            else
            {
                // Left/right shooting only
                if (spriteRenderer.flipX)
                    bulletScript.SetDirection(Vector2.left);
                else
                    bulletScript.SetDirection(Vector2.right);
            }
        }


    }


}
