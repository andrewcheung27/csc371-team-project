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
    private bool easyMode = false;

    [Header ("Health")]
    private bool playerDamageEnabled = true;  // whether the player can take damage
    private int minHealth = 0;  // at this health or lower, game over
    public int health = 3;  // current health
    public int maxHealth = 8;  // maximum health
    public int easyModeMaxHealth = 1000000000;  // maximum health in easy mode
    private int startingHealth;  // keep track of starting health for respawning
    public HealthBar healthBar;  // health bar UI

    [Header ("Score")]
    private int minScore = 0;  // can't go below this score
    private int score = 0;  // current score
    public int deathPenalty = 100;  // how many points you lose for dying
    public GameObject scorePopUpPrefab;  // prefab for a Canvas with a TextMeshProUGUI child to show points when an enemy is killed
    public float scorePopUpDuration = 2f;  // how many seconds until score pop up disappears

    [Header ("Timer")]
    private float timer = 0;

    [Header ("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI easyModeText;
    public Button respawnButton;  // button to respawn player after dying
    public GameObject damageEffect;  // UI Panel that shows when player takes damage
    public float damageEffectDuration = 0.5f;  // how long damage effect lasts

    [Header ("Player Spawning")]
    public GameObject player;  // game object with PlayerMovement component
    PlayerMovement playerMovement;
    Rigidbody playerRB;
    public List<Transform> spawnPoints;
    public int spawnPointIndex = 0;  // current spawn point, as an index in list of spawnPoints

    [Header ("Boundaries")]
    public float minHeightBeforeDeath = -16f;  // die when player is lower than this height

    [Header ("Effects")]
    public GameObject bloodEffectPrefab; // Assign the blood effect prefab
    public float bloodEffectDuration = 2f;
    public GameObject playerHeadGameObject; // Assign the player's head GameObject

    [Header("Death Tracking")]
    private int deathCount = 0; // Track deaths in the current level
    private int defaultMaxHealth; // Store the original max health for reset

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

        defaultMaxHealth = maxHealth;

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
        if (healthBar == null)
        {
            healthBar = FindFirstObjectByType<HealthBar>();
        }
    }

    void Start()
    {
        // carry over score from previous level
        if (ScoreManager.instance != null) {
            score = ScoreManager.instance.GetScore();
        }

        if (player != null) {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerRB = player.GetComponent<Rigidbody>();
        }

        StartGame();
    }

    void Update()
    {
        if (gameRunning) {
            CheckBoundaries();

            UpdateTimer();

            if (Input.GetKeyDown(KeyCode.Minus)) {
                ToggleEasyMode();
            }

            if (Input.GetKeyDown(KeyCode.Equals)) {
                KillPlayer();
            }
        }
    }

    public bool GameIsRunning()
    {
        return gameRunning;
    }

    void ToggleEasyMode()
    {
        // turn off easy mode
        if (easyMode) {
            health = maxHealth;
            startingHealth = health;
            // update health bar
            if (healthBar != null) {
                healthBar.SetHealth(health);
                healthBar.SetMaxHealth(maxHealth);
            }
            // update UI
            if (easyModeText != null) {
                easyModeText.gameObject.SetActive(false);
            }
        }

        // turn on easy mode
        else {
            health = easyModeMaxHealth;
            startingHealth = health;
            // update health bar
            if (healthBar != null) {
                healthBar.SetHealth(health);
                healthBar.SetMaxHealth(easyModeMaxHealth);
            }
            // update UI
            if (easyModeText != null) {
                easyModeText.gameObject.SetActive(true);
            }
        }

        easyMode = !easyMode;
    }

    public int GetScore()
    {
        return score;
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

    public bool EasyMode()
    {
        return easyMode;
    }

    void KillPlayer()
    {
        deathCount++; // Increase the death counter

        // Every 3 deaths, increase max health by 150%
        if (deathCount % 3 == 0)
        {
            maxHealth = Mathf.RoundToInt(maxHealth * 1.5f);
            startingHealth = maxHealth; // Update starting health
        }
        AddToScore(-1 * deathPenalty);  // penalty for dying
        EndGame();
    }

    public void AddToHealth(int n)
    {
        if (!playerDamageEnabled && n < 1) {
            return;
        }

        // update health with min and max restrictions
        health = Mathf.Clamp(health + n, minHealth, easyMode ? easyModeMaxHealth : maxHealth);

        // update health bar
        if (healthBar != null) {
            healthBar.SetHealth(health);
        }

        // damage effect
        if (n < 0) {
            StartCoroutine(DamageEffectRoutine());

            // Spawn blood effect when taking damage
            if (bloodEffectPrefab != null && playerHeadGameObject != null)
            {
                GameObject blood = Instantiate(bloodEffectPrefab, playerHeadGameObject.transform.position, Quaternion.identity);

                if (blood == null)
                {
                    Debug.LogError("GameManager: Failed to instantiate blood effect.");
                }
                else
                {
                    // Set the blood effect to shoot upwards
                    blood.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    Destroy(blood, bloodEffectDuration);
                }
            }
            else
            {
                Debug.LogError("GameManager: bloodEffectPrefab or playerHeadGameObject is null.");
            }
        }

        // end game if out of health
        if (health <= minHealth) {
            KillPlayer();
            return;
        }
    }

    public void SetPlayerDamageEnabled(bool b)
    {
        playerDamageEnabled = b;
    }

    // show a message like "+100" for a certain amount of time.
    // based on: https://www.youtube.com/watch?v=KOt85IoD__4
    public void ShowScorePopup(Vector3 position, int amount)
    {
        if (!gameRunning) {
            return;
        }

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

        if (healthBar != null) {
            healthBar.SetMaxHealth(easyMode ? easyModeMaxHealth : maxHealth);
            healthBar.SetHealth(health);
        }
        // UI elements
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
        if (player != null && spawnPoints.Count > 0) {
            // set position
            player.transform.position = spawnPoints[spawnPointIndex].transform.position;

            // reset velocity
            if (playerRB != null) {
                playerRB.linearVelocity = Vector3.zero;
            }

            // cancel dash
            if (playerMovement != null) {
                playerMovement.SetIsDashing(false);
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