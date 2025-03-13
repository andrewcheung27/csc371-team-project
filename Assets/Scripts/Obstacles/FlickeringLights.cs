using UnityEngine;

public class FlickeringLights : MonoBehaviour
{
    Light lightComp;

    [Header ("Flickering")]
    public float flickerIntervalOn = 2f;
    public float flickerIntervalOnVariance = 1f;
    public float flickerIntervalOff = 0.5f;
    public float flickerIntervalOffVariance = 0.25f;
    bool lightOff = false;
    float nextFlickerInterval = 2f;
    float timeSinceFlicker = 0f;

    [Header ("Light Intensity")]
    public float minIntensity = 2f;
    public float maxIntensity = 8f;

    void Awake()
    {
        lightComp = GetComponentInChildren<Light>();
    }

    void Start()
    {
        nextFlickerInterval = GetNextFlickerInterval();
    }

    void Update()
    {
        timeSinceFlicker += Time.deltaTime;
        if (timeSinceFlicker > nextFlickerInterval) {
            Flicker();
            nextFlickerInterval = GetNextFlickerInterval();
            timeSinceFlicker = 0f;
        }
    }

    float GetNextFlickerInterval()
    {
        if (lightOff) {
            return flickerIntervalOff + Random.Range(-flickerIntervalOffVariance, flickerIntervalOffVariance);
        }

        return flickerIntervalOn + Random.Range(-flickerIntervalOnVariance, flickerIntervalOnVariance);
    }

    void Flicker()
    {
        // flick on: random intensity
        if (lightOff) {
            lightComp.intensity = Random.Range(minIntensity, maxIntensity);
        }
        // flick off
        else {
            lightComp.intensity = 0f;
        }

        lightOff = !lightOff;
    }
}
