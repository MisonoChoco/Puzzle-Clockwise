using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int currentScore = 0;
    private int highScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
    }

    private void SaveHighScore()
    {
        string key = GetHighScoreKey();
        PlayerPrefs.SetInt(key, highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        string key = GetHighScoreKey();
        highScore = PlayerPrefs.GetInt(key, 0);
    }

    private string GetHighScoreKey()
    {
        return GameSettings.CurrentMode == GameMode.Timed ? "HighScore_Timed" : "HighScore_Normal";
    }
}