using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour {
    public TextMeshProUGUI uiText; // Drag your TextMeshProUGUI element here via the Inspector.
    public float letterDelay = 0.005f; // Delay between letters.
    
    private string accumulatedText = ""; // Keeps all finished lines.
    
    // Call this method to type a new line.
    public void TypeLine(string message, System.Action onComplete = null) {
        StartCoroutine(TypeLineRoutine(message, onComplete));
    }
    
    IEnumerator TypeLineRoutine(string message, System.Action onComplete) {
        string currentLine = "";
        for (int i = 0; i < message.Length; i++) {
            currentLine += message[i];
            // Always show the accumulated (old) text plus what we are currently typing.
            uiText.text = accumulatedText + currentLine;
            yield return new WaitForSeconds(letterDelay);
        }
        // When finished, add the completed line plus a newline.
        accumulatedText += currentLine + "\n";
        onComplete?.Invoke();
    }
    
    // Use this method if you need to reset the text at the start.
    public void ResetText() {
        accumulatedText = "";
        uiText.text = "";
    }
}
