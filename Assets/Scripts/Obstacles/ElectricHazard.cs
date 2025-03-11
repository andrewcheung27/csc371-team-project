using UnityEngine;

public class ElectricHazard : MonoBehaviour
{
    public float yCoord1;
    public float yCoord2;
    public float speed = 1f;
    public int damage = 1;

    private Vector3 velocity;

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed, 1);
        Vector3 newPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, yCoord1, transform.localPosition.z), new Vector3(transform.localPosition.x, yCoord2, transform.localPosition.z), t);
        velocity = (newPosition - transform.position) / Time.deltaTime;
        transform.localPosition = newPosition;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            GameManager.instance.AddToHealth(-damage);
        }
    }
}
