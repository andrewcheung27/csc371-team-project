using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int healAmount = 1; // Amount of health restored (you can set this in the prefab)
    public float lifetime = 30f;  // how long before it disappears
    public float warningTime = 10f;  // how long before disappearance to start blinking
    float spawnTime = 0f;
    MeshRenderer rendChild;  // meshrenderer of child (Heart asset)

    void Awake()
    {
        spawnTime = Time.time;

        rendChild = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        HandleBlink();
    }

    // adapted ChatGPT code for blinking to show that it will disappear
    void HandleBlink()
    {
        float elapsedTime = Time.time - spawnTime;
        float remainingTime = lifetime - elapsedTime;

        // Start blinking when the remaining time is less than the warning time
        if (remainingTime <= warningTime)
        {
            float t = remainingTime / warningTime;

            // Make it blink faster as time runs out
            float blinkSpeed = Mathf.Lerp(3f, 6f, 1f - t);
            bool isVisible = Mathf.PingPong(Time.time * blinkSpeed, 1f) > 0.5f;

            // Toggle renderer visibility
            rendChild.enabled = isVisible;
        }

        // Destroy the pickup when time runs out
        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

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

    public void SetHealAmount(int n)
    {
        healAmount = n;
    }
}
