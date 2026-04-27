using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text finalScoreText;

    private string sessionStartTimeTimestamp = "";
    private string sessionEndTimeTimestamp = "";
    private const string HIGH_SCORE_KEY = "FlappyHighScore";

    private int currentScore = 0;
    private int highScore = 0;

    private float sessionStartTime = 0;
    private int pipesPassed = 0;
    private int clicks = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    public void ResetScore()
    {
        currentScore = 0;
        clicks = 0;
        
        pipesPassed = 0;
        sessionStartTime = Time.time;
        sessionStartTimeTimestamp = DateTime.UtcNow.ToString("o");
        
        UpdateScoreDisplay();
    }

    public void AddPoint()
    {
        currentScore++;
        pipesPassed++;
        UpdateScoreDisplay();
    }

    public int GetCurrentScore() => currentScore;
    public int GetHighScore() => highScore;

    public void SaveHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
            finalScoreText.text = $"Score: {currentScore}\nBest: {highScore}";

        sessionEndTimeTimestamp = DateTime.UtcNow.ToString("o");
        
        if (FirebaseManager.Instance != null)
        {
            int duration = Mathf.RoundToInt(Time.time - sessionStartTime);
            FirebaseManager.Instance.SubmitScore(currentScore, clicks, pipesPassed, duration, sessionStartTimeTimestamp, sessionEndTimeTimestamp);
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString();

        if (highScoreText != null)
            highScoreText.text = $"Best: {highScore}";
    }

    public void UpdateClicksCount()
    {
        clicks++;
    }
}
