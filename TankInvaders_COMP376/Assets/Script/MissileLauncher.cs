using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileLauncher : MonoBehaviour
{
    public Transform missileSpawnPoint;
    public GameObject missilePrefab;
    public float missileSpeed = 15;
    public float missileMaxRange = 50f; // Maximum range the missile can travel
    public float cooldownTime = 2f; // Cooldown time in seconds
    public int missileDamage = 50; // Damage stat
    public int shotsPerBurst = 4; // Number of shots per burst
    public float intervalBetweenShots = 0.1f; // Interval time between each shot
    public Slider cooldownSlider; // Reference to the cooldown slider UI element
    public float shakeDuration = 0.2f; // Duration of the screen shake
    public float shakeMagnitude = 0.4f; // Magnitude of the screen shake

    private List<GameObject> activeMissiles = new List<GameObject>();
    private float lastShotTime;

    void Start()
    {
        cooldownSlider.maxValue = cooldownTime;
        cooldownSlider.value = cooldownTime;
    }

    void Update()
    {
        if (Input.GetButton("Fire2") && Time.time >= lastShotTime + cooldownTime)
        {
            StartCoroutine(ShootBurst());
            lastShotTime = Time.time; // Update the last shot time
        }

        // Update cooldown slider
        float cooldownRemaining = Mathf.Max(0, (lastShotTime + cooldownTime) - Time.time);
        cooldownSlider.value = cooldownTime - cooldownRemaining;

        // Check the distance traveled by each missile
        for (int i = activeMissiles.Count - 1; i >= 0; i--)
        {
            if (activeMissiles[i] == null || Vector3.Distance(missileSpawnPoint.position, activeMissiles[i].transform.position) > missileMaxRange)
            {
                if (activeMissiles[i] != null)
                {
                    Destroy(activeMissiles[i]);
                }
                activeMissiles.RemoveAt(i);
            }
        }
    }

    private IEnumerator ShootBurst()
    {
        for (int i = 0; i < shotsPerBurst; i++)
        {
            var missile = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation);
            missile.GetComponent<Rigidbody>().velocity = missileSpawnPoint.forward * missileSpeed;
            activeMissiles.Add(missile);

            // Optionally, you can pass the damage stat to the missile if needed
            // missile.GetComponent<Missile>().damage = missileDamage;

            // Trigger screen shake
            if (ScreenShake.instance != null)
            {
                StartCoroutine(ScreenShake.instance.Shake(shakeDuration, shakeMagnitude));
            }

            yield return new WaitForSeconds(intervalBetweenShots);
        }
    }
}
