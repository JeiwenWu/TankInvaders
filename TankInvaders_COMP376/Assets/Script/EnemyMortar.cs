using System.Collections;
using UnityEngine;

public class EnemyMortar : MonoBehaviour
{
    public float range = 50f; // Long range
    public float attackInterval = 5f; // Long attack intervals
    public GameObject mortarShellPrefab; // Prefab for the mortar shell
    public Color chargeColor = Color.red; // Color during charge-up
    public Renderer enemyRenderer; // Reference to the Renderer component
    public Transform spawnPoint; // Spawn point for the mortar shell
    public int shotsPerBurst = 1; // Number of shots per burst
    public float chargeTime = 2f; // Charge-up time

    private float attackTimer;
    private bool isCharging = false;

    void Start()
    {
        attackTimer = attackInterval;
        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponent<Renderer>();
        }
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= chargeTime && !isCharging)
        {
            StartCharging();
        }
        if (attackTimer <= 0f)
        {
            StartCoroutine(ShootBurst());
            attackTimer = attackInterval;
        }
    }

    void StartCharging()
    {
        isCharging = true;
        enemyRenderer.material.color = chargeColor;
    }

    private IEnumerator ShootBurst()
    {
        isCharging = false;
        enemyRenderer.material.color = Color.white; // Reset color after attack

        Vector3? targetPosition = GetTargetPosition();
        if (targetPosition == null)
        {
            yield break; // Exit the coroutine if no target is in range
        }

        for (int i = 0; i < shotsPerBurst; i++)
        {
            GameObject mortarShell = Instantiate(mortarShellPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = mortarShell.GetComponent<Rigidbody>();
            Vector3 velocity = CalculateLaunchVelocity(targetPosition.Value);
            rb.velocity = velocity;

            yield return null; // Wait for the next frame
        }
    }

    Vector3? GetTargetPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= range)
            {
                return player.transform.position;
            }
        }
        // Return null if the player is not in range
        return null;
    }

    Vector3 CalculateLaunchVelocity(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - spawnPoint.position;
        float distance = new Vector2(direction.x, direction.z).magnitude; // Horizontal distance
        float heightDifference = targetPosition.y - spawnPoint.position.y;

        // Calculate the initial velocity required to hit the target
        float gravity = Physics.gravity.magnitude;
        float angle = Mathf.Deg2Rad * 45f; // Launch angle in radians

        float speed = Mathf.Sqrt(gravity * distance / Mathf.Sin(2 * angle));
        Vector3 velocity = new Vector3(direction.x, 0, direction.z).normalized * speed;
        velocity.y = speed * Mathf.Sin(angle) + heightDifference;

        return velocity;
    }
}
