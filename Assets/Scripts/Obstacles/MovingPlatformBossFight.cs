using UnityEngine;

public class MovingPlatformBossFight : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 1f;

    private Vector3 velocity;

    // ChatGPT adapted this function from the MovingPlatform class to make the platform slow down at the start and end of its motion
    void FixedUpdate()
    {
        float pingPongTime = Mathf.PingPong(Time.time * speed, 1f);

        // Apply easing to pingPongTime to slow down at the ends
        float easedTime = EaseInOutPingPong(pingPongTime);

        Vector3 newPosition = Vector3.Lerp(pointA, pointB, easedTime);
        velocity = (newPosition - transform.position) / Time.deltaTime;
        transform.position = newPosition;
    }

    // Easing function to slow down more at the ends
    float EaseInOutPingPong(float t)
    {
        // Sinusoidal easing for more pronounced slow-down
        return 0.5f - 0.5f * Mathf.Cos(Mathf.PI * t);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
