// based on credits tutorial: https://www.youtube.com/watch?v=Eeee4TU69x4.
// adapted with some ChatGPT code to detect scroll progress and fade in "The End" text.

using TMPro;
using UnityEngine;

public class ScrollingCredits : MonoBehaviour
{
    [Header ("Scrolling")]
    public float scrollSpeed = 30f;
    public float stopAfter = 60f * 3;

    [Header ("Title")]
    public RectTransform titleRect;

    [Header ("Credits")]
    public TextMeshProUGUI credits;
    RectTransform creditsRect;
    private float totalHeight;

    [Header ("End Message")]
    public CanvasGroup theEndCanvasGroup;
    public float whenToShowEndMessage = 1.4f;
    float fadeTimer = 0f;
    float fadeDuration = 3f;
    bool endTextFadingIn = false;

    [Header ("Player Animation")]
    public GameObject player;
    public float animationSpeed = 3f;
    public float whenToPlayAnimation = 0.8f;  // 0.8 means 80% through the credits
    private bool animationPlayed = false;

    void Start()
    {
        creditsRect = credits.GetComponent<RectTransform>();

        totalHeight = credits.preferredHeight;

        if (theEndCanvasGroup != null) {
            theEndCanvasGroup.alpha = 0; // Start hidden
            theEndCanvasGroup.gameObject.SetActive(true);
        }

        // destroy this after a while so it doesn't scroll forever
        Destroy(gameObject, stopAfter);
    }

    void Update()
    {
        // do scroll
        titleRect.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        creditsRect.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        // Calculate the scroll progress as a percentage
        float scrollProgress = Mathf.Abs(creditsRect.anchoredPosition.y) / totalHeight;

        // Trigger animation at 80% scroll
        if (scrollProgress >= whenToPlayAnimation && !animationPlayed)
        {
            PlayAnimation();
            animationPlayed = true;
        }

        // Start fading in "The End" message when scrolling is complete
        if (scrollProgress >= whenToShowEndMessage && !endTextFadingIn)
        {
            endTextFadingIn = true;
        }

        // Handle fade-in effect
        if (endTextFadingIn)
        {
            fadeTimer += Time.deltaTime;
            theEndCanvasGroup.alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
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
