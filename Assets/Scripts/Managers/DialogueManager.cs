using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public GameObject dialogueImage;  // speech bubble image
    public TextMeshProUGUI dialogueText;  // text that is a child of the speech bubble image
    public float dialogueTime = 3f;

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

    public void ShowDialogue(string dialogue)
    {
        dialogueImage.SetActive(true);
        dialogueText.text = dialogue;
        StartCoroutine(CloseDialogue());
    }

    IEnumerator CloseDialogue()
    {
        yield return new WaitForSeconds(dialogueTime);
        dialogueImage.SetActive(false);
    }
}
