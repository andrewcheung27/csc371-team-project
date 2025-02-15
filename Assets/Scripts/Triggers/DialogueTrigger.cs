using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string dialogue = "";
    public bool onlyShowOnce = true;
    private bool triggerEnabled = true;

    void OnTriggerEnter(Collider other)
    {
        if (!triggerEnabled) {
            return;
        }

        if (other.gameObject.CompareTag("Player")) {
            DialogueManager.instance.ShowDialogue(dialogue);
            if (onlyShowOnce) {
                triggerEnabled = false;
            }
        }
    }
}
