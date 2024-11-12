using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineGun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 40;
    public float bulletMaxRange = 10f; // Maximum range the bullet can travel
    public float cooldownTime = 0.1f; // Cooldown time in seconds
    public int bulletDamage = 5; // Damage stat
    public float overheatTime = 5f; // Time in seconds before the gun overheats
    public float cooldownDuration = 3f; // Time in seconds to cooldown
    public Slider overheatSlider; // Reference to the overheat slider UI element

    private List<GameObject> activeBullets = new List<GameObject>();
    private float lastShotTime;
    private bool isShooting = false;
    private float shootingTime = 0f;
    private bool isOverheated = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        overheatSlider.maxValue = overheatTime;
        overheatSlider.value = 0;
    }

    void Update()
    {
        if (isOverheated)
        {
            cooldownTimer += Time.deltaTime;
            overheatSlider.value = overheatTime - (cooldownTimer / cooldownDuration) * overheatTime;

            if (cooldownTimer >= cooldownDuration)
            {
                isOverheated = false;
                cooldownTimer = 0f;
                overheatSlider.value = 0;
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire3"))
            {
                isShooting = !isShooting;
            }

            if (isShooting)
            {
                shootingTime += Time.deltaTime;
                overheatSlider.value = shootingTime;

                if (shootingTime >= overheatTime)
                {
                    isOverheated = true;
                    isShooting = false;
                    shootingTime = 0f;
                }
                else if (Time.time >= lastShotTime + cooldownTime)
                {
                    Shoot();
                    lastShotTime = Time.time; // Update the last shot time
                }
            }
            else
            {
                if (overheatSlider.value > 0)
                {
                    overheatSlider.value -= Time.deltaTime;
                }
                if (shootingTime > 0)
                {
                    shootingTime -= Time.deltaTime;
                }
            }
        }

        // Check the distance traveled by each bullet
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            if (activeBullets[i] == null || Vector3.Distance(bulletSpawnPoint.position, activeBullets[i].transform.position) > bulletMaxRange)
            {
                if (activeBullets[i] != null)
                {
                    Destroy(activeBullets[i]);
                }
                activeBullets.RemoveAt(i);
            }
        }
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.transform.Rotate(90, 0, 0); // Rotate the bullet on the x-axis by 90 degrees
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
        activeBullets.Add(bullet);

        // Optionally, you can pass the damage stat to the bullet if needed
        // bullet.GetComponent<Bullet>().damage = bulletDamage;
    }
}
