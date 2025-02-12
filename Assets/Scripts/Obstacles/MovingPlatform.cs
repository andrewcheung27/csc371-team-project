using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 1f;

    private Vector3 velocity;

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed, 1);
        Vector3 newPosition = Vector3.Lerp(pointA, pointB, t);
        velocity = (newPosition - transform.position) / Time.deltaTime;
        transform.position = newPosition;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
