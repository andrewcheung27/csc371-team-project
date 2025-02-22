using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header ("Player Audio Sources")]
    public AudioSource playerShoot;


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
    }

    public void PlayerShoot()
    {
        if (playerShoot != null) {
            playerShoot.Play();
        }
    }
}
