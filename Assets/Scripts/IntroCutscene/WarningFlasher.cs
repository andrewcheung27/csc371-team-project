using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningFlasher : MonoBehaviour {
    public Image warningImage; // Assign your WarningSymbol Image.
    public float flashInterval = 0.5f;
    private bool flashing = false;

    public void StartFlashing() {
        flashing = true;
        StartCoroutine(Flash());
    }

    IEnumerator Flash() {
        while (flashing) {
            warningImage.enabled = !warningImage.enabled;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    public void StopFlashing() {
        flashing = false;
        warningImage.enabled = false;
    }
}
