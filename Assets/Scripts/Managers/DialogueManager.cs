using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public GameObject dialogueImage;  // speech bubble image
    public TextMeshProUGUI dialogueText;  // text that is a child of the speech bubble image
    float mostRecentDialogueDuration;
    float mostRecentDialogueTime;

    void Awake()
    {
        // make sure there is only one DialogueManager instance
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (Time.time - mostRecentDialogueTime > mostRecentDialogueDuration) {
            dialogueImage.SetActive(false);
        }
    }

    public void ShowDialogue(string dialogue, float duration)
    {
        dialogueImage.SetActive(true);
        dialogueText.text = dialogue;

        mostRecentDialogueTime = Time.time;
        mostRecentDialogueDuration = duration;
    }
}
