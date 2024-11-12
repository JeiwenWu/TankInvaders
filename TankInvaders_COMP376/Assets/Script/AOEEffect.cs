using UnityEngine;

public class AOEEffect : MonoBehaviour
{
    private float damage;
    private float duration;

    public void Initialize(float damage, float duration)
    {
        this.damage = damage;
        this.duration = duration;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
