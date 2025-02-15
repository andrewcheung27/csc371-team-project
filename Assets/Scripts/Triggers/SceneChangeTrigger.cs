using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    public float sceneChangeDelay = 0f;

    void OnTriggerEnter(Collider other)
    {
        // when player enters this trigger, set spawn point to here
        if (other.gameObject.CompareTag("Player")) {
            SceneChanger.instance.LoadNextScene(sceneChangeDelay);
        }
    }
}
