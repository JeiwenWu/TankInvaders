using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    public float maxHealth = 200f;
    private float currentHealth;
    public Slider healthBar; // Reference to the health bar UI element
    public Image healthBarFill; // Reference to the health bar fill image
    public float gasCloudDamageRate = 1f; // Damage per second from the gas cloud
    public float flashDuration = 0.5f; // Duration of the flash effect
    public Color flashColor = Color.red; // Color of the flash effect
    public GameObject impactParticleEffect; // Reference to the particle effect prefab

    private float gasCloudDamageTimer = 0f;
    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10f); // Adjust the damage value as needed
            InstantiateImpactEffect(collision.contacts[0].point);
        }
        else if (collision.gameObject.CompareTag("TankTrap"))
        {
            TakeDamage(20f); // Adjust the damage value as needed for tank trap collision
            InstantiateImpactEffect(collision.contacts[0].point);
            // Do not destroy the tank trap GameObject
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoinPoints(); // Gain 500 points for collecting a coin
            Destroy(collision.gameObject); // Destroy the coin GameObject
        }
        else if (collision.gameObject.CompareTag("EnemyShell"))
        {
            EnemyCannon enemyCannon = collision.gameObject.GetComponentInParent<EnemyCannon>();
            if (enemyCannon != null)
            {
                TakeDamage(enemyCannon.shellDamage); // Take damage equal to the shell damage
                InstantiateImpactEffect(collision.contacts[0].point);
            }
        }
        else if (collision.gameObject.CompareTag("MortarShell"))
        {
            MortarShell mortarShell = collision.gameObject.GetComponent<MortarShell>();
            if (mortarShell != null)
            {
                TakeDamage(mortarShell.damage); // Take damage equal to the mortar shell damage
                InstantiateImpactEffect(collision.contacts[0].point);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GasCloud"))
        {
            gasCloudDamageTimer = 0f; // Reset the damage timer when entering the gas cloud
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("GasCloud"))
        {
            gasCloudDamageTimer += Time.deltaTime;
            if (gasCloudDamageTimer >= 1f)
            {
                TakeDamage(gasCloudDamageRate);
                gasCloudDamageTimer = 0f; // Reset the timer after applying damage
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GasCloud"))
        {
            gasCloudDamageTimer = 0f; // Reset the damage timer when exiting the gas cloud
        }
    }

    void Update()
    {
        if (healthBar != null && Camera.main != null)
        {
            healthBar.transform.LookAt(Camera.main.transform);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Handle player death (e.g., restart the game, show game over screen, etc.)
            //Destroy(gameObject); // Destroy the player GameObject
        }
        UpdateHealthBar();

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashHealthBar());
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

    IEnumerator FlashHealthBar()
    {
        Color originalColor = healthBarFill.color;
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            healthBarFill.color = flashColor;
            yield return new WaitForSeconds(flashDuration / 2);
            healthBarFill.color = originalColor;
            yield return new WaitForSeconds(flashDuration / 2);
            elapsedTime += flashDuration;
        }

        healthBarFill.color = originalColor;
    }

    void InstantiateImpactEffect(Vector3 position)
    {
        if (impactParticleEffect != null)
        {
            GameObject effect = Instantiate(impactParticleEffect, position, Quaternion.identity);
        }
    }

    public float GetCurrentHP()
    {
        return currentHealth;
    }
}
