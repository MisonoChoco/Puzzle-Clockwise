using TMPro;
using UnityEngine;

public class WinFailScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        int score = ScoreManager.Instance.GetScore();
        int highScore = ScoreManager.Instance.GetHighScore();

        scoreText.text = $"Score Earned: {score}";
        highScoreText.text = $"High Score: {highScore}";
    }
}