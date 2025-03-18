using UnityEngine;
using TMPro;

public class ScrollingText : MonoBehaviour {
    public TextMeshProUGUI uiText; // Drag your TextMeshProUGUI element here.
    public float scrollSpeed = 100f; // Increased speed for faster upward scroll.

    void Update() {
        // Moves the text upward.
        uiText.rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }
}
