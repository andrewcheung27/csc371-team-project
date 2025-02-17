using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public TextMeshProUGUI dialogueText;
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
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = dialogue;
        StartCoroutine(CloseDialogue());
    }

    IEnumerator CloseDialogue()
    {
        yield return new WaitForSeconds(dialogueTime);
        dialogueText.gameObject.SetActive(false);
    }
}
