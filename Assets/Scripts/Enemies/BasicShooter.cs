using UnityEngine;

public class BasicShooter : Enemy
{


    public Transform shootPoint;           // Where the bullets come from
    public GameObject bulletPrefab;        // Assign your bullet prefab
    public float shootInterval = 2f;       // Time between shots

    private SpriteRenderer spriteRenderer;
    private float shootTimer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Auto-find shootPoint if not assigned
        if (shootPoint == null)
        {
            Transform found = transform.Find("ShootPoint");
            if (found != null)
                shootPoint = found;
            else
                Debug.LogWarning("ShootPoint not assigned and not found under Enemy!");
        }

        //if (player == null)
        //{
        //    // Automatically find player if not assigned
        //    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        //    if (playerObj != null)
        //    {
        //        player = playerObj.transform;
        //    }
        //}
    }

    private void Update()
    {
        if (player == null) return;

        // Face player
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

        // Tell bullet which direction to go
        EnemyBulletMovement bulletScript = bullet.GetComponent<EnemyBulletMovement>();
        if (bulletScript != null)
        {
            if (spriteRenderer.flipX)
                bulletScript.SetDirection(Vector2.left);
            else
                bulletScript.SetDirection(Vector2.right);
        }
    }


}
