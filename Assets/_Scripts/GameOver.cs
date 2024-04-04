using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI leaderboardScoreText;
    [SerializeField] private TextMeshProUGUI leaderboardNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int score;
    private string playerName;
    private int highScore;

    private const string HighScoreKey = "HighScore";
    private const string PlayerNameKey = "PlayerName";

    private void Start()
    {
        // Load the high score and player name from PlayerPrefs when the game starts
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        highScoreText.text = highScore.ToString();

        playerName = PlayerPrefs.GetString(PlayerNameKey, "");
        leaderboardNameText.text = playerName;
    }

    public void StopGame(int score)
    {
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = " ";
        SubmitScore();
    }

    public void SubmitScore()
    {
        leaderboardScoreText.text = score.ToString();

        if (score > highScore)
        {
            highScore = score;
            // Save the new high score to PlayerPrefs
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
            highScoreText.text = highScore.ToString();
        }
    }

    public void SubmitName()
    {
        playerName = inputField.text;
        leaderboardNameText.text = playerName;
        // Save the player name to PlayerPrefs
        PlayerPrefs.SetString(PlayerNameKey, playerName);
        PlayerPrefs.Save();
    }

    public void AddXP(int score)
    {
        // Implement this method if needed
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

