using UnityEngine;
using UnityEngine.UI;

public class Mortar : MonoBehaviour
{
    public float maxHealth = 100f; // Maximum health of the mortar
    public Slider healthBar; // Reference to the health bar UI element
    public Image healthBarFill; // Reference to the health bar fill UI element
    public GameObject mortarShellPrefab;
    public GameObject mortarAOEPrefab; // Reference to the mortar AOE prefab
    public float mortarSpeed = 15;
    public float mortarMaxRange = 50f; // Maximum range the mortar shell can travel
    public int trajectoryResolution = 30; // Number of points in the trajectory line
    public GameObject impactParticleEffect; // Reference to the particle effect prefab

    private float currentHealth;
    private LineRenderer trajectoryLine;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Create and configure the LineRenderer
        trajectoryLine = gameObject.AddComponent<LineRenderer>();
        trajectoryLine.positionCount = trajectoryResolution;
        trajectoryLine.startWidth = 0.1f;
        trajectoryLine.endWidth = 0.1f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = Color.red;
        trajectoryLine.endColor = Color.red;

        // Spawn the mortar shell at a random position
        SpawnMortarShell();
    }

    void Update()
    {
        if (healthBar != null && Camera.main != null)
        {
            healthBar.transform.LookAt(Camera.main.transform);
        }

        // Update the trajectory line
        UpdateTrajectoryLine();
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

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.AddEnemyPoints(1500); // Add 1500 points when the mortar is destroyed
            Destroy(gameObject); // Destroy the mortar GameObject
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

    private void UpdateTrajectoryLine()
    {
        if (trajectoryLine == null)
        {
            Debug.LogWarning("Trajectory line is not assigned.");
            return;
        }

        Vector3 startPosition = transform.position;
        Vector3 velocity = transform.forward * mortarSpeed;
        float timeStep = mortarMaxRange / (mortarSpeed * trajectoryResolution);
        for (int i = 0; i < trajectoryResolution; i++)
        {
            float time = i * timeStep;
            Vector3 position = startPosition + velocity * time + 0.5f * Physics.gravity * time * time;
            trajectoryLine.SetPosition(i, position);
        }
    }

    void InstantiateImpactEffect(Vector3 position)
    {
        if (impactParticleEffect != null)
        {
            GameObject effect = Instantiate(impactParticleEffect, position, Quaternion.identity);
        }
    }

    void SpawnMortarShell()
    {
        if (mortarShellPrefab == null)
        {
            Debug.LogError("Mortar shell prefab is not assigned.");
            return;
        }

        // Generate a random position within the map boundaries
        Vector3 randomPosition = new Vector3(
            Random.Range(-50f, 50f), // Adjust the range as needed
            1f, // Set the Y-axis to 1
            Random.Range(-50f, 50f) // Adjust the range as needed
        );

        Instantiate(mortarShellPrefab, randomPosition, Quaternion.identity);
    }
}
