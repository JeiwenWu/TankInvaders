using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
    public Transform shellSpawnPoint;
    public GameObject shellPrefab;
    public float shellSpeed = 20;
    public float shellMaxRange = 50f; // Maximum range the shell can travel
    public float cooldownTime = 3f; // Cooldown time in seconds
    public int shellDamage = 100; // Damage stat
    public Slider cooldownSlider; // Reference to the cooldown slider UI element
    public float shakeDuration = 0.2f; // Duration of the screen shake
    public float shakeMagnitude = 0.4f; // Magnitude of the screen shake

    private List<GameObject> activeShells = new List<GameObject>();
    private float lastShotTime;

    void Start()
    {
        cooldownSlider.maxValue = cooldownTime;
        cooldownSlider.value = cooldownTime;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= lastShotTime + cooldownTime)
        {
            var shell = Instantiate(shellPrefab, shellSpawnPoint.position, shellSpawnPoint.rotation);
            shell.GetComponent<Rigidbody>().velocity = shellSpawnPoint.forward * shellSpeed;
            activeShells.Add(shell);
            lastShotTime = Time.time; // Update the last shot time

            // Trigger screen shake
            if (ScreenShake.instance != null)
            {
                StartCoroutine(ScreenShake.instance.Shake(shakeDuration, shakeMagnitude));
            }
        }

        // Update cooldown slider
        float cooldownRemaining = Mathf.Max(0, (lastShotTime + cooldownTime) - Time.time);
        cooldownSlider.value = cooldownTime - cooldownRemaining;

        // Check the distance traveled by each shell
        for (int i = activeShells.Count - 1; i >= 0; i--)
        {
            if (activeShells[i] == null || Vector3.Distance(shellSpawnPoint.position, activeShells[i].transform.position) > shellMaxRange)
            {
                if (activeShells[i] != null)
                {
                    Destroy(activeShells[i]);
                }
                activeShells.RemoveAt(i);
            }
        }
    }
}
