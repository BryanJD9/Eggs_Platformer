using UnityEngine;
using System.Collections;

public class BossTest : Enemy
{
    [Header("Movement Points")]
    [SerializeField] private Transform groundPoint1;
    [SerializeField] private Transform groundPoint2;
    [SerializeField] private Transform platform1;
    [SerializeField] private Transform platform2;

    [Header("Shooting")]
    [SerializeField] private Transform shootingPosition;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int shotsPerBurst = 3;
    [SerializeField] private float timeBetweenShots = 0.4f;

    [Header("Charge Settings")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float pauseTime = 1f;
    [SerializeField] private float minActionInterval = 3f;
    [SerializeField] private float maxActionInterval = 6f;

    private bool isPerformingAction = false;
    //private SpriteRenderer spriteRenderer;
    //private Transform player;

    private void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found! Make sure player is tagged 'Player'.");

        StartCoroutine(AutoActionRoutine());
    }

    private IEnumerator AutoActionRoutine()
    {
        yield return new WaitForSeconds(2f); // small delay before starting

        while (true)
        {
            float waitTime = Random.Range(minActionInterval, maxActionInterval);
            yield return new WaitForSeconds(waitTime);

            int action = Random.Range(0, 3); // 0=Ground, 1=Platform, 2=Shoot

            switch (action)
            {
                case 0:
                    yield return StartCoroutine(DoChargeAttack(false));
                    break;
                case 1:
                    yield return StartCoroutine(DoChargeAttack(true));
                    break;
                case 2:
                    yield return StartCoroutine(DoTeleportAndShoot());
                    break;
            }
        }
    }

    private IEnumerator DoChargeAttack(bool usePlatform)
    {
        if (isPerformingAction)
            yield break;
        isPerformingAction = true;

        Transform startPoint = usePlatform ? platform1 : groundPoint1;
        Transform endPoint = usePlatform ? platform2 : groundPoint2;

        yield return StartCoroutine(MoveBetweenPoints(startPoint, endPoint));
        yield return new WaitForSeconds(pauseTime);
        yield return StartCoroutine(MoveBetweenPoints(endPoint, startPoint));

        isPerformingAction = false;
    }

    private IEnumerator MoveBetweenPoints(Transform from, Transform to)
    {
        if (from == null || to == null)
            yield break;

        transform.position = from.position;

        while (Vector2.Distance(transform.position, to.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                to.position,
                chargeSpeed * Time.deltaTime
            );

            if (spriteRenderer != null)
                spriteRenderer.flipX = (to.position.x < transform.position.x);

            yield return null;
        }

        transform.position = to.position;
    }

    private IEnumerator DoTeleportAndShoot()
    {
        if (isPerformingAction)
            yield break;
        isPerformingAction = true;

        if (shootingPosition == null)
        {
            Debug.LogWarning("Shooting position not assigned!");
            yield break;
        }

        // Teleport effect placeholder (you could add VFX later)
        transform.position = shootingPosition.position;
        Debug.Log("Boss teleported to shooting position.");

        yield return new WaitForSeconds(0.5f); // short delay before shooting

        for (int i = 0; i < shotsPerBurst; i++)
        {
            ShootAtPlayer();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        isPerformingAction = false;
    }

    private void ShootAtPlayer()
    {
        if (player == null || projectilePrefab == null)
            return;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }

        // Flip sprite toward player
        if (spriteRenderer != null)
            spriteRenderer.flipX = (player.position.x < transform.position.x);
    }

    // Override Die to trigger Win condition
    public override void Die()
    {
        base.Die(); // destroys the boss object

        if (GameOverManager.Instance != null)
            GameOverManager.Instance.WinGame();
        else
            Debug.LogWarning("GameOverManager not found in scene!");
    }



}
