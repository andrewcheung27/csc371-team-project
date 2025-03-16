using System.Collections.Generic;
using UnityEngine;

public class MiniBossPhaseManager : MonoBehaviour
{
    MiniBossMovement movement;
    MiniBossShooting shooter;
    EnemyHealth health;

    [Header ("Boss Phases")]
    int phase = 1;
    public int healthPercentThresholdPhase2 = 60;
    public int healthPercentThresholdPhase3 = 30;
    int healthThresholdPhase2;
    int healthThresholdPhase3;

    [Header ("Glowing Eyes")]
    public Light glowingEye1;
    public Light glowingEye2;
    public float glowingEyesDelayAfterNewPhase = 1f;
    public List<Color> colorsForEachPhase;
    public List<float> intensitiesForEachPhase;

    [Header ("Background Music")]
    public float bgmVolPhase1 = 0.6f;
    public float bgmVolPhase2 = 0.9f;
    public float bgmVolPhase3 = 1.3f;

    void Start()
    {
        movement = GetComponent<MiniBossMovement>();
        shooter = GetComponent<MiniBossShooting>();
        health = GetComponent<EnemyHealth>();

        // convert health percents into ints
        healthThresholdPhase2 = Mathf.RoundToInt(healthPercentThresholdPhase2 / 100f * health.GetHealth());
        healthThresholdPhase3 = Mathf.RoundToInt(healthPercentThresholdPhase3 / 100f * health.GetHealth());

        StartCoroutine(SetGlowingEyes(delay: 0f, phase: 1));

        AudioManager.instance.SetBackgroundMusicVolume(bgmVolPhase1);
    }

    void Update()
    {
        if (phase == 1 && health.GetHealth() <= healthThresholdPhase2) {
            DoPhase2();
            phase = 2;
        }
        else if (phase == 2 && health.GetHealth() <= healthThresholdPhase3) {
            DoPhase3();
            phase = 3;
        }
    }

    void DoPhase2()
    {
        Debug.Log("Boss Phase 2");

        movement.MultiplyOriginalNormalSpeed(1.2f);  // 20% faster than phase 1
        shooter.AddShotsPerLoad(1);  // add 1 more shot to the volley
        health.AddToBossHealthDropInterval(-2);  // make health drops appear more often

        AudioManager.instance.SetBackgroundMusicVolume(bgmVolPhase2);
        AudioManager.instance.BossRoar(volume: 2f);
        movement.Flinch();

        StartCoroutine(SetGlowingEyes(delay: glowingEyesDelayAfterNewPhase, phase: 2));
    }

    void DoPhase3()
    {
        Debug.Log("Boss Phase 3");

        movement.MultiplyOriginalNormalSpeed(1.5f);  // 50% faster than phase 1
        shooter.AddShotsPerLoad(1);  // add 1 more shot to the volley
        health.AddToBossHealthDropInterval(-2);  // make health drops appear more often

        AudioManager.instance.SetBackgroundMusicVolume(bgmVolPhase3);
        AudioManager.instance.BossRoar(volume: 4f);
        movement.Flinch();

        StartCoroutine(SetGlowingEyes(delay: glowingEyesDelayAfterNewPhase, phase: 3));
    }

    IEnumerator<WaitForSeconds> SetGlowingEyes(float delay, int phase) {
        yield return new WaitForSeconds(delay);

        int p = phase - 1;  // because list indices start at 0

        // set color
        Color newColor;
        if (colorsForEachPhase.Count > p) {
            newColor = colorsForEachPhase[p];
        }
        else {
            newColor = Color.red;
        }
        glowingEye1.color = newColor;
        glowingEye2.color = newColor;

        // set intensity
        float newIntensity;
        if (intensitiesForEachPhase.Count > p) {
            newIntensity = intensitiesForEachPhase[p];
        }
        else {
            newIntensity = 1f;
        }
        glowingEye1.intensity = newIntensity;
        glowingEye2.intensity = newIntensity;
    }
}
