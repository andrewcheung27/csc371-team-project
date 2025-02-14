using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// each scene in the game should have one object with this component
public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    [Header ("UI")]
    public Button startGameButton;

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

        if (startGameButton != null) {
            startGameButton.onClick.AddListener(StartGame);
        }
    }

    // load scene of the first level
    void StartGame()
    {
        Destroy(gameObject);  // the first level should have its own object with a SceneChanger component
        SceneManager.LoadScene("1 - Freezer");
    }
}
