using UnityEngine;

public class SpawnPointSetter : MonoBehaviour
{
    public bool canSetSpawnPoint = true;

    void OnTriggerEnter(Collider other)
    {
        // when player enters this trigger, set spawn point to here
        if (canSetSpawnPoint && other.gameObject.CompareTag("Player")) {
            GameManager.instance.SetSpawnPoint(transform);
            canSetSpawnPoint = false;
        }
    }
}
