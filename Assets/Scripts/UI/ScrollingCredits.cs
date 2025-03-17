// based on credits tutorial: https://www.youtube.com/watch?v=Eeee4TU69x4. adapted with some ChatGPT code to detect scroll progress.

using UnityEngine;

public class ScrollingCredits : MonoBehaviour
{
    public float scrollSpeed = 30f;
    public bool controlAnimation = false;
    public GameObject player;
    public float animationSpeed = 3f;
    private RectTransform rectTransform;
    private float totalHeight;
    private bool animationPlayed = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        totalHeight = rectTransform.rect.height + 50; // Get the height of the credits

        // destroy this after a while so it doesn't scroll forever
        Destroy(gameObject, 60f * 10);
    }

    void Update()
    {
        // Move the credits
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if (!controlAnimation) {
            return;
        }

        // Calculate the scroll progress as a percentage
        float scrollProgress = Mathf.Abs(rectTransform.anchoredPosition.y) / totalHeight;

        // Trigger animation at 80% scroll
        if (scrollProgress >= 0.8f && !animationPlayed)
        {
            PlayAnimation();
            animationPlayed = true;
        }
    }

    void PlayAnimation()
    {
        Debug.Log("ScrollingCredits: playing animation");

        if (player.TryGetComponent(out Rigidbody rb)) {
            rb.linearVelocity = new Vector3(0f, 0f, animationSpeed);
        }
    }
}
