using UnityEngine;

public class ElectricHazardProjectile : MonoBehaviour
{
    public int damage = 1;
    public float maxLifespan = 30f;
    Vector3 projectileVelocity;

    void Start()
    {
        if (projectileVelocity == null) {
            projectileVelocity = new Vector3(0, 0, 1);
        }

        Destroy(gameObject, maxLifespan);
    }

    void Update()
    {
        transform.Translate(projectileVelocity * Time.deltaTime, Space.World);
    }

    public void SetVelocity(Vector3 v)
    {
        projectileVelocity = v;
    }

    void OnTriggerEnter(Collider other)
    {
        // damage and destroy if it hits player
        if (other.gameObject.CompareTag("Player")) {
            GameManager.instance.AddToHealth(-damage);
            AudioManager.instance.ElectricHazardDamage();
            Destroy(gameObject);
        }
        // destroy this if it hits a solid object
        else if (other.gameObject.CompareTag("Wall") 
            || other.gameObject.CompareTag("MovingPlatform")
            || other.gameObject.CompareTag("Ground")) {
                Destroy(gameObject);
        }
    }
}
