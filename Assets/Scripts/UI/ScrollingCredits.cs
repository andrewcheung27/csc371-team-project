// credits tutorial: https://www.youtube.com/watch?v=Eeee4TU69x4

using UnityEngine;

public class ScrollingCredits : MonoBehaviour
{
    public float scrollSpeed = 30f;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }
}
