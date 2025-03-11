using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // Assign the weapon prefab in the inspector
    public float rotationSpeed = 30f; // Degrees per second
    public float floatSpeed = 0.5f; // Speed of floating motion
    public float floatHeight = 0.2f; // Max height offset
    private Vector3 startPosition;
    private bool isPickedUp = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isPickedUp)
        {
            // Rotate the weapon slowly around the Y-axis
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            // Make the weapon move up and down using a sine wave
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeaponHandler weaponHandler = other.GetComponent<PlayerWeaponHandler>();
            if (weaponHandler != null)
            {
                weaponHandler.PickUpWeapon(weaponPrefab);

                // Stop animation and make the pickup disappear
                isPickedUp = true;
                Destroy(gameObject); // Destroy the pickup object after it's picked up
            }
        }
    }
}