using UnityEngine;

public class ElectricHazardShooter : MonoBehaviour
{
    public Vector3 attackDirection;  // direction to attack
    public float attackInterval = 2f;  // how many seconds in between each attack
    public float projectileSpeed = 2f;  // speed of projectiles it fires
    public float minAttackDistance = 50f;  // don't attack unless distance to player is closer than this
    public GameObject electricHazardProjectilePrefab;  // prefab for projectile to fire
    float timeSinceAttack = 0f;
    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        timeSinceAttack += Time.deltaTime;

        if (player != null) {
            if ((player.transform.position - transform.position).magnitude > minAttackDistance) {
                return;
            }
        }

        if (timeSinceAttack > attackInterval) {
            Attack();
            timeSinceAttack = 0f;
        }
    }

    void Attack()
    {
        GameObject projectile = Instantiate(electricHazardProjectilePrefab, transform.position, transform.rotation);
        ElectricHazardProjectile electricHazardProjectile = projectile.GetComponent<ElectricHazardProjectile>();
        electricHazardProjectile.SetVelocity(attackDirection.normalized * projectileSpeed);
    }
}
