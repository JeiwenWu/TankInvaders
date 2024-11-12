using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float maxSpeed = 40f;
    public float acceleration = 10f;
    public float steeringRotationSpeed = 75f; // Speed of steering rotation
    public float topPartRotationSpeed = 3000f; // Speed of top part rotation
    public Transform topPartTransform; // Reference to the top part transform
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Setup the camera behind the player
        GameObject playerCamera = Camera.main.gameObject;
        playerCamera.transform.SetParent(topPartTransform);
        playerCamera.transform.localPosition = new Vector3(0, 4f, 10f);
        playerCamera.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");
        float rotateHorizontal = Input.GetAxis("Mouse X");

        Vector3 forwardMovement = transform.forward * moveVertical * acceleration;
        Vector3 movement = forwardMovement.normalized * acceleration;

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movement, ForceMode.Acceleration);
        }

        // Steer the player when moving forward or backward
        if (moveVertical != 0)
        {
            transform.Rotate(0, moveHorizontal * steeringRotationSpeed * Time.deltaTime, 0);
        }

        // Rotate the topPart based on mouse movement
        if (topPartTransform != null)
        {
            topPartTransform.Rotate(0, rotateHorizontal * topPartRotationSpeed * Time.deltaTime, 0);
        }
    }
}

