using TMPro;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Camera cam;
    public float cooldown = 1.0f;
    public GameObject[] objectsToActivate;
    public TextMeshProUGUI activatableMessage;
    public float activatableMessageCooldown = 0.3f;  // how long after player was near the button to show message
    private float timeSinceActivatable = 0f;  // seconds since player was near the button
    private float timeSinceActivated = 0f;  // seconds since player activated the button

    void Awake()
    {
        activatableMessage.text = "Press [E] to Activate";
        activatableMessage.transform.position = cam.WorldToScreenPoint(transform.position);
    }

    void Update()
    {
        timeSinceActivatable += Time.deltaTime;
        if (timeSinceActivatable > activatableMessageCooldown) {
            SetNotActivatable();
        }

        timeSinceActivated += Time.deltaTime;
    }

    public void SetActivatable()
    {
        timeSinceActivatable = 0f;
        activatableMessage.gameObject.SetActive(true);
    }

    public void SetNotActivatable()
    {
        activatableMessage.gameObject.SetActive(false);
    }

    public void Activate()
    {
        // enforce cooldown
        if (timeSinceActivated < cooldown) {
            return;
        }

        // activate objects
        foreach (GameObject obj in objectsToActivate)
        {
            obj.GetComponent<ButtonActivation>().Activate();
        }
        timeSinceActivated = 0f;
    }
}
