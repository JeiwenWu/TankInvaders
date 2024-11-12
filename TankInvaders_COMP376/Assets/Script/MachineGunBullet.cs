using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunBullet : MonoBehaviour
{
    public float life = 3;
    public float maxRange = 50f; // Maximum range the bullet can travel

    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
        Destroy(gameObject, life);
    }

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > maxRange)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Plane" && collision.gameObject.tag != "Player")
        {
            //Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}


