using UnityEngine;

public class GasCloud : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float initialSpeed = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 2.5f;

    private float currentSpeed;

    void Start()
    {
        transform.position = new Vector3(0, 1, 33);
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        // Update gas cloud position
        UpdatePosition(player.position);
    }

    private void UpdatePosition(Vector3 playerPosition)
    {
        // Calculate distance to player
        Vector3 direction = playerPosition - transform.position;
        float distance = direction.magnitude;

        // Normalize direction
        direction.Normalize();

        // Adjust speed based on distance to player
        if (distance < 10.0f)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        }
        else
        {
            currentSpeed = initialSpeed;
        }

        // Update position
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
