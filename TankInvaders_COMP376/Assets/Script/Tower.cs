using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public float buffRadius = 10f; // Radius within which enemies are buffed
    public float damageBuff = 5f; // Amount of damage buff
    public Color gizmoColor = Color.red; // Color of the gizmo
    public LineRenderer lineRenderer; // Reference to the LineRenderer component
    public Slider healthBar; // Reference to the health bar UI element
    public Image healthBarFill; // Reference to the health bar fill UI element
    public float maxHealth = 100f;
    public GameObject impactParticleEffect; // Reference to the particle effect prefab

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 50;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = gizmoColor;
        lineRenderer.endColor = gizmoColor;
        DrawCircle();
    }

    void Update()
    {
        BuffNearbyEnemies();
        healthBar.transform.LookAt(Camera.main.transform);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Missile"))
        {
            TakeDamage(50f); // Adjust the damage value as needed
            InstantiateImpactEffect(collision.contacts[0].point);
        }
        else if (collision.gameObject.CompareTag("Shell"))
        {
            TakeDamage(100f); // Adjust the damage value as needed
            InstantiateImpactEffect(collision.contacts[0].point);
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(5f); // Adjust the damage value as needed
            InstantiateImpactEffect(collision.contacts[0].point);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(20f); // Adjust the damage value as needed for player collision
            InstantiateImpactEffect(collision.contacts[0].point);
        }
    }

    void BuffNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyCannon enemyCannon = hitCollider.GetComponent<EnemyCannon>();
                if (enemyCannon != null)
                {
                    enemyCannon.shellDamage += damageBuff;
                }

                MortarShell mortarShell = hitCollider.GetComponent<MortarShell>();
                if (mortarShell != null)
                {
                    mortarShell.damage += damageBuff;
                }
            }
        }
    }

    void DrawCircle()
    {
        float angle = 20f;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * buffRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * buffRadius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / lineRenderer.positionCount);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, buffRadius);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.AddEnemyPoints(2000); // Add 2000 points when the tower is destroyed
            Destroy(gameObject); // Destroy the tower GameObject
        }
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;

            // Change color based on health percentage
            if (healthBarFill != null)
            {
                float healthPercentage = currentHealth / maxHealth;
                if (healthPercentage > 0.75f)
                {
                    healthBarFill.color = new Color(0.0f, 1.0f, 0.0f); // Custom Green
                }
                else if (healthPercentage > 0.5f)
                {
                    healthBarFill.color = new Color(1.0f, 0.65f, 0.0f); // Custom Orange
                }
                else if (healthPercentage > 0.25f)
                {
                    healthBarFill.color = new Color(1.0f, 1.0f, 0.0f); // Custom Yellow
                }
                else
                {
                    healthBarFill.color = new Color(1.0f, 0.0f, 0.0f); // Custom Red
                }
            }
        }
    }

    void InstantiateImpactEffect(Vector3 position)
    {
        if (impactParticleEffect != null)
        {
            GameObject effect = Instantiate(impactParticleEffect, position, Quaternion.identity);
        }
    }
}
