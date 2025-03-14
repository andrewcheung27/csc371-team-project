using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int healAmount = 1; // Amount of health restored (you can set this in the prefab)

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Access GameManager instance and add to health
            GameManager.instance.AddToHealth(healAmount);

            AudioManager.instance.HealthDropPickup();

            // Destroy health pack after it's picked up
            Destroy(gameObject);
        }
    }
}
