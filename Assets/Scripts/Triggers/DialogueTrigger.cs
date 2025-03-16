using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string dialogue = "";
    public float duration = 3f;
    public bool onlyShowOnce = true;
    private bool triggerEnabled = true;

    void OnTriggerEnter(Collider other)
    {
        if (!triggerEnabled) {
            return;
        }

        if (other.gameObject.CompareTag("Player")) {
            DialogueManager.instance.ShowDialogue(dialogue, duration);
            if (onlyShowOnce) {
                triggerEnabled = false;
            }
        }
    }
}
