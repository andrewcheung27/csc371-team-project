using UnityEngine;

public class ElectricHazardProjectile : MonoBehaviour
{
    public int damage = 1;
    Vector3 projectileVelocity;

    void Start()
    {
        if (projectileVelocity == null) {
            projectileVelocity = new Vector3(0, 0, 1);
        }
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
        Debug.Log("EHP hit: " + other.gameObject.name);
        // damage and destroy if it hits player
        if (other.gameObject.CompareTag("Player")) {
            GameManager.instance.AddToHealth(-damage);
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
