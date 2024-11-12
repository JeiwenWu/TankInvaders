using UnityEngine;

public class MortarShell : MonoBehaviour
{
    public float speed = 10f; // Speed of the mortar shell
    public float damage = 10f; // Damage dealt by the mortar shell

    private Vector3 targetPosition;

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage); // Use the damage attribute
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Plane") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
