using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Transform healthbarSprite;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        if (healthbarSprite != null)
        {
            // Calculate the new scale
            float healthPercentage = currentHealth / maxHealth;
            healthbarSprite.localScale = new Vector3(healthPercentage, 1, 1);
        }
        else
        {
            Debug.LogError("Healthbar sprite is not assigned.");
        }
    }
}