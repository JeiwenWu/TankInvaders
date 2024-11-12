using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoinPoints(); // Gain 500 points for picking up a coin
            Destroy(gameObject); // Destroy the coin GameObject
        }
    }
}
