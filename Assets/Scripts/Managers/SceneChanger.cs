using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// each scene in the game should have one object with this component
public class SceneChanger : MonoBehaviour
{
    [Header ("Scenes")]
    public static SceneChanger instance;
    public string nextSceneName = null;  // name of next scene to load

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

        // initialize startGameButton
        if (startGameButton != null) {
            startGameButton.onClick.AddListener(LoadNextSceneWrapper);
        }
    }

    // call LoadNextScene() with no arguments
    void LoadNextSceneWrapper()
    {
        LoadNextScene();
    }

    // after a delay, load the next scene
    public void LoadNextScene(float delay=0f)
    {
        StartCoroutine(LoadNextSceneRoutine(delay));
    }

    // load the next scene asynchronously
    IEnumerator LoadNextSceneRoutine(float delay)
    {
        // wait for delay
        yield return new WaitForSeconds(delay);

        // load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        // TODO: show loading message?

        // wait until scene loading is finished, then move on to the code below
        while (!asyncLoad.isDone) {
            yield return null;
        }

        // destroy manager game object because it should be in the next scene
        Destroy(gameObject);
    }
}
