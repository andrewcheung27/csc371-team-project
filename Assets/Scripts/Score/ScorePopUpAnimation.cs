// based on tutorial: https://www.youtube.com/watch?v=KOt85IoD__4

using TMPro;
using UnityEngine;

public class ScorePopUpAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;

    private TextMeshProUGUI popupText;
    private float time = 0;
    private Vector3 popupOrigin;

    void Awake()
    {
        popupText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        popupOrigin = transform.position;
    }

    void Update()
    {
        popupText.alpha = opacityCurve.Evaluate(time);
        transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        transform.position = popupOrigin + new Vector3(0, 1 + heightCurve.Evaluate(time), 0);

        time += Time.deltaTime;
    }
}
