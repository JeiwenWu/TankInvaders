using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour
{
    public Transform shellSpawnPoint;
    public GameObject shellPrefab;
    public float shellSpeed = 10;
    public float shellMaxRange = 50f; // Maximum range the shell can travel
    public float shootInterval = 5f; // Interval between shots in seconds
    public float shellDamage = 50; // Damage stat

    private List<GameObject> activeShells = new List<GameObject>();
    private Player player;
    private Coroutine shootingCoroutine;

    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            shootingCoroutine = StartCoroutine(ShootAtInterval());
        }
    }

    void Update()
    {
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

    IEnumerator ShootAtInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);
            Shoot();
        }
    }

    void Shoot()
    {
        if (player != null)
        {
            Vector3 direction = (player.transform.position - shellSpawnPoint.position);
            direction.y = 0; // Ensure the Y axis remains 0 for horizontal shooting
            direction = direction.normalized;
            var shell = Instantiate(shellPrefab, shellSpawnPoint.position, Quaternion.LookRotation(direction));
            Rigidbody shellRigidbody = shell.GetComponent<Rigidbody>();
            shellRigidbody.velocity = direction * shellSpeed;
            shellRigidbody.useGravity = false; // Disable gravity for the shell
            activeShells.Add(shell); // Add the GameObject instance to the list
        }
    }

    void OnDestroy()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
    }
}
