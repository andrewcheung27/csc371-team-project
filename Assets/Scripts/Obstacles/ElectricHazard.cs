using UnityEngine;

public class ElectricHazard : MonoBehaviour
{
    [Header ("Motion")]
    public bool rotate = true;
    public float rotateSpeed = 90f;

    void Start()
    {
        
    }

    void Update()
    {
        if (rotate) {
            transform.Rotate(new Vector3(rotateSpeed * Time.deltaTime, 0f, 0f));
        }
    }
}
