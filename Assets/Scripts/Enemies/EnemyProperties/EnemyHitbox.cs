using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public enum HitboxType { Head, Body }
    public HitboxType hitboxType;

    public int baseDamage = 10;
    public int headshotMultiplier = 2;
    MiniBossMovement miniBoss;

    void Start()
    {
        miniBoss = GetComponentInParent<MiniBossMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            // Log the hitbox type and damage
            int damage = (hitboxType == HitboxType.Head) ? baseDamage * headshotMultiplier : baseDamage;

            // Trigger damage to MiniBoss (assuming it has the TakeDamage method)
            if (hitboxType == HitboxType.Head)
            {
                if (miniBoss != null)
                {
                    miniBoss.TakeDamage();  // Call TakeDamage when hit in the head
                }
            }
        }
    }
}
