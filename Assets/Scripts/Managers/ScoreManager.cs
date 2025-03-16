using UnityEngine;


// this component should be in each scene with a score.
// since this uses DontDestroyOnLoad, it should NOT be on the same object as the other managers.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int startingScore = 0;
    int score;

    void Awake()
    {
        // this is a singleton class
        if (instance == null) {
            instance = this;
            score = startingScore;
        }
        else {
            Destroy(gameObject);
            return;
        }

        // so we can carry over score between scenes
        DontDestroyOnLoad(gameObject);
    }

    public void SetScore(int n)
    {
        score = n;
    }

    public int GetScore()
    {
        return score;
    }
}
