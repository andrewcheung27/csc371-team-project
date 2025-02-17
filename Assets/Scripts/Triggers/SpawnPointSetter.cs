using UnityEngine;

public class SpawnPointSetter : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // when player enters this trigger, set spawn point to here
        if (other.gameObject.CompareTag("Player")) {
            GameManager.instance.SetSpawnPoint(transform);
        }
    }
}
