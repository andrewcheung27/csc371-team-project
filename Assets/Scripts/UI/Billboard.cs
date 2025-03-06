using UnityEngine;

public class Billboard : MonoBehaviour
{

    public Transform cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
