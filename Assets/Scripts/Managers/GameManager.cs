using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header ("Game")]
    public static GameManager instance;
    private bool gameRunning = false;

    [Header ("Health")]
    private int minHealth = 0;  // at this health or lower, game over
    public int health = 3;  // current health
    public int maxHealth = 3;  // maximum health
    private int startingHealth;  // keep track of starting health for respawning

    [Header ("Score")]
    private int minScore = 0;  // can't go below this score
    public int score = 0;  // current score
    public int deathPenalty = 100;  // how many points you lose for dying
    public GameObject scorePopUpPrefab;  // prefab for a Canvas with a TextMeshProUGUI child to show points when an enemy is killed
    public float scorePopUpDuration = 2f;  // how many seconds until score pop up disappears

    [Header ("Timer")]
    private float timer = 0;

    [Header ("UI")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Button respawnButton;  // button to respawn player after dying
    public GameObject damageEffect;  // UI Panel that shows when player takes damage
    public float damageEffectDuration = 0.5f;  // how long damage effect lasts

    [Header ("Player Spawning")]
    public GameObject player;  // game object with PlayerMovement component
    public List<Transform> spawnPoints;
    public int spawnPointIndex = 0;  // current spawn point, as an index in list of spawnPoints

    [Header ("Boundaries")]
    public float minHeightBeforeDeath = -16f;  // die when player is lower than this height

    void Awake()
    {
        // this is a singleton class
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        // enforce minimum health and score
        if (health <= minHealth) {
            throw new Exception("Health must be greater than " + minHealth.ToString());
        }
        if (score < minScore) {
            throw new Exception("Score must be greater than or equal to " + minScore.ToString());
        }

        // save starting health for respawn
        startingHealth = health;

        // respawn button listener
        if (respawnButton != null) {
            respawnButton.onClick.AddListener(StartGame);
        }
    }

    void Start()
    {
        // TODO: title screen before game start

        StartGame();
    }

    void Update()
    {
        if (gameRunning) {
            CheckBoundaries();

            UpdateTimer();
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null) {
            healthText.text = "Health: " + health.ToString();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null) {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null) {
            timerText.text = Mathf.Floor(timer).ToString();
        }
    }

    void UpdateTimer()
    {
        timer += Time.deltaTime;
        UpdateTimerText();
    }

    void KillPlayer()
    {
        AddToScore(-1 * deathPenalty);  // penalty for dying
        EndGame();
    }

    public void AddToHealth(int n)
    {
        // update health with min and max restrictions
        health = Mathf.Clamp(minHealth, health + n, maxHealth);

        // update UI
        UpdateHealthText();

        // damage effect
        if (n < 0) {
            StartCoroutine(DamageEffectRoutine());
        }

        // end game if out of health
        if (health <= minHealth) {
            KillPlayer();
            return;
        }
    }

    // show a message like "+100" for a certain amount of time.
    // based on: https://www.youtube.com/watch?v=KOt85IoD__4
    public void ShowScorePopup(Vector3 position, int amount)
    {
        // instantiate and set text
        GameObject popup = Instantiate(scorePopUpPrefab, position, Quaternion.identity);
        if (popup.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI popupText)) {
            popupText.text = "+" + amount.ToString();
        }

        // destroy pop-up after a certain amount of time
        Destroy(popup, scorePopUpDuration);
    }

    public void AddToScore(int n) {
        score = Mathf.Max(score + n, minScore);
        UpdateScoreText();
    }

    void CheckBoundaries()
    {
        if (player != null && player.transform.position.y < minHeightBeforeDeath) {
            KillPlayer();
        }
    }

    void StartGame()
    {
        // start the game
        Time.timeScale = 1;
        gameRunning = true;

        // reset health for respawning
        health = startingHealth;

        // UI elements
        if (healthText != null) {
            UpdateHealthText();
            healthText.gameObject.SetActive(true);
        }
        if (scoreText != null) {
            UpdateScoreText();
            scoreText.gameObject.SetActive(true);
        }
        if (timerText != null) {
            UpdateTimerText();
            timerText.gameObject.SetActive(true);
        }

        // disable respawn button
        if (respawnButton != null) {
            respawnButton.gameObject.SetActive(false);
        }

        // disable damage effect
        if (damageEffect != null) {
            damageEffect.gameObject.SetActive(false);
        }

        // spawn player at current spawn point
        if (player != null) {
            // set position
            player.transform.position = spawnPoints[spawnPointIndex].transform.position;

            // reset velocity
            if (player.TryGetComponent(out Rigidbody rb)) {
                rb.linearVelocity = Vector3.zero;
            }
        }
    }

    void EndGame()
    {
        // stop game
        Time.timeScale = 0;
        gameRunning = false;

        // show respawn button
        if (respawnButton != null) {
            respawnButton.gameObject.SetActive(true);
        }
    }

    IEnumerator<WaitForSeconds> DamageEffectRoutine()
    {
        if (damageEffect != null) {
            damageEffect.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(damageEffectDuration);

        if (damageEffect != null) {
            damageEffect.gameObject.SetActive(false);
        }
    }

    public void SetSpawnPoint(Transform newSpawnPoint) {
        // add a spawn point to the end of the list and set it as the current spawn point
        if (spawnPoints[spawnPoints.Count - 1] != newSpawnPoint) {
            spawnPoints.Add(newSpawnPoint);
            spawnPointIndex = spawnPoints.Count - 1;
        }
    }
}
