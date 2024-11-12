using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShell : MonoBehaviour
{
    public float life = 99;
    public float maxRange = 50f; // Maximum range the shell can travel

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
        if (collision.gameObject.tag != "Shell" || collision.gameObject.tag != "Plane" || collision.gameObject.tag != "Player")
        {
            //Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}

