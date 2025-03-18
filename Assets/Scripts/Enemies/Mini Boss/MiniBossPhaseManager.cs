using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MiniBossPhaseManager : MonoBehaviour
{
    MiniBossMovement movement;
    MiniBossShooting shooter;
    EnemyHealth health;
    private Animator animator;

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
    public float bgmVolPhase1 = 0.3f;
    public float bgmVolPhase2 = 0.4f;
    public float bgmVolPhase3 = 0.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        StartCoroutine(PhaseTransition(phase: 2, attackCount: 1, roarVolume: .7f, moveMultiplier: 1.2f));
    }

    void DoPhase3()
    {
        Debug.Log("Boss Phase 3");
        StartCoroutine(PhaseTransition(phase: 3, attackCount: 2, roarVolume: 1f, moveMultiplier: 1.8f, projectileSpeedIncrease: 5));
    }

    IEnumerator PhaseTransition(int phase, int attackCount, float roarVolume, float moveMultiplier, int projectileSpeedIncrease = 0)
    {
        // Stop movement
        movement.SetIsAttacking(true);
        movement.agent.isStopped = true;
        shooter.StopShooting();

        // Perform attacks
        AudioManager.instance.BossRoar(roarVolume);
        for (int i = 0; i < attackCount; i++)
        {
            animator.SetTrigger("Attack");

            // Wait for attack animation to finish before continuing
            yield return new WaitForSeconds(2f);
        }

        // Resume movement and update stats
        movement.SetIsAttacking(false);
        movement.agent.isStopped = false;
        movement.MultiplyOriginalNormalSpeed(moveMultiplier);
        shooter.AddShotsPerLoad(1);
        health.AddToBossHealthDropInterval(-2);

        if (projectileSpeedIncrease > 0)
        {
            shooter.AddToProjectileSpeed(projectileSpeedIncrease);
        }

        AudioManager.instance.SetBackgroundMusicVolume(phase == 2 ? bgmVolPhase2 : bgmVolPhase3);
        movement.Flinch();

        StartCoroutine(SetGlowingEyes(glowingEyesDelayAfterNewPhase, phase));
    }

    IEnumerator SetGlowingEyes(float delay, int phase) {
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
