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

    [Header ("UI")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public Button respawnButton;  // button to respawn player after dying
    public GameObject damageEffect;  // UI Panel that shows when player takes damage
    public float damageEffectDuration = 0.5f;  // how long damage effect lasts

    [Header ("Player Spawning")]
    public GameObject player;  // game object with PlayerMovement component
    public List<GameObject> spawnPoints;
    public int spawnPointIndex = 0;  // current spawn point, as an index in list of spawnPoints

    [Header ("Boundaries")]
    public float minHeightBeforeDeath = -16f;  // die when player is lower than this height

    void Awake()
    {
        // make sure there is only one GameManager instance
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

        // turn off Update() for title screen
        Time.timeScale = 0;

        // respawn button listener
        respawnButton.onClick.AddListener(StartGame);
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
        }
    }

    void UpdateHealthText()
    {
        healthText.text = "Health: " + health.ToString();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
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

        // end game if out of health
        if (health <= minHealth) {
            KillPlayer();
            return;
        }

        // damage effect
        if (n < 0) {
            StartCoroutine(DamageEffectRoutine());
        }
    }

    public void AddToScore(int n) {
        score = Mathf.Max(score + n, minScore);
        UpdateScoreText();
    }

    void CheckBoundaries()
    {
        if (player.transform.position.y < minHeightBeforeDeath) {
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

        // health and score UI elements
        UpdateHealthText();
        UpdateScoreText();
        healthText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);

        // disable respawn button
        respawnButton.gameObject.SetActive(false);

        // spawn player at current spawn point
        player.transform.position = spawnPoints[spawnPointIndex].transform.position;
    }

    void EndGame()
    {
        // stop game
        Time.timeScale = 0;
        gameRunning = false;

        // show respawn button
        respawnButton.gameObject.SetActive(true);
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
}
