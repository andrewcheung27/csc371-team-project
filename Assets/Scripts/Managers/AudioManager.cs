using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header ("Background Music")]
    public bool playBackgroundMusic = true;
    public float volume = 1f;
    public AudioSource backgroundMusicAudioSource;  // should not have a clip
    public AudioClip backgroundMusicToPlayOnce;
    public AudioClip backgroundMusicToLoop;

    [Header ("Player Audio Clips")]
    public AudioClip playerShoot;
    public AudioClip hunterAttack;
    public AudioClip hunterDeath;
    public AudioClip spitterDeath;
    public AudioClip slasherAttack;
    public AudioClip slasherDeath;
    public AudioClip electricHazardDamage;

    [Header("Player Reference")]
    public Transform playerTransform; 

    [Header ("Audio Settings")]
    public int audioSourcePoolSize = 10;

    private AudioSource[] audioSources;
    private int currentIndex = 0;

    void Awake()
    {
        // Singleton Pattern
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        // Create AudioSource pool
        audioSources = new AudioSource[audioSourcePoolSize];
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            GameObject audioObj = new GameObject("AudioSource_" + i);
            audioObj.transform.parent = transform;
            audioSources[i] = audioObj.AddComponent<AudioSource>();

            // Prevent the sound from playing on awake
            audioSources[i].playOnAwake = false;

            // Enable 3D sound (makes the sound positional)
            audioSources[i].spatialBlend = 1f; // 0 = 2D, 1 = 3D

            // Adjust the rolloff distance so the sound fades out over distance
            audioSources[i].minDistance = 5f;  // Full volume within 5 units
            audioSources[i].maxDistance = 50f; // Completely inaudible beyond 50 units
            audioSources[i].rolloffMode = AudioRolloffMode.Linear; // Smooth fade-out
        }

        StartBackgroundMusic();
    }

    void StartBackgroundMusic()
    {
        if (!playBackgroundMusic) {
            return;
        }

        backgroundMusicAudioSource.volume = Mathf.Clamp(volume, 0f, 1f);

        float loopingMusicDelay = 0f;
        // play the track that should be played once
        if (backgroundMusicToPlayOnce != null) {
            backgroundMusicAudioSource.clip = backgroundMusicToPlayOnce;
            backgroundMusicAudioSource.loop = false;
            backgroundMusicAudioSource.Play();
            loopingMusicDelay = backgroundMusicToPlayOnce.length;
        }

        // play looping music afterwards
        StartCoroutine(PlayLoopingBackgroundMusicAfterDelay(loopingMusicDelay));
    }

    IEnumerator<WaitForSeconds> PlayLoopingBackgroundMusicAfterDelay(float whenToStart)
    {
        yield return new WaitForSeconds(whenToStart);

        if (backgroundMusicToLoop != null) {
            backgroundMusicAudioSource.clip = backgroundMusicToLoop;
            backgroundMusicAudioSource.loop = true;
            backgroundMusicAudioSource.Play();
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f, float minPitch = 0.8f, float maxPitch = 1.2f)
    {
         if (clip == null || playerTransform == null) return;

        // Get the next AudioSource in the pool
        AudioSource source = audioSources[currentIndex];

        // Set the clip and volume
        source.clip = clip;
        source.volume = volume;

        // set random pitch
        if (minPitch < maxPitch) {
            source.pitch = Random.Range(minPitch, maxPitch);
        }

        // Move the AudioSource to the player's position
        source.transform.position = playerTransform.position;

        // Play the sound
        source.Play();

        // Cycle through the pool
        currentIndex++;
        if (currentIndex >= audioSourcePoolSize)
            currentIndex = 0;
    }

    public void PlayerShoot()
    {
        PlaySound(playerShoot);
    }

    public void HunterAttack()
    {
        PlaySound(hunterAttack, 1f);
    }

    public void HunterDeath()
    {
        PlaySound(hunterDeath, 1f);
    }

    public void SpitterDeath()
    {
        PlaySound(spitterDeath, 0.5f);
    }

    public void SlasherAttack()
    {
        PlaySound(slasherAttack, 1.5f);
    }

    public void SlasherDeath()
    {
        PlaySound(slasherDeath, 1f);
    }

    public void ElectricHazardDamage()
    {
        PlaySound(electricHazardDamage, 0.75f);
    }
}
