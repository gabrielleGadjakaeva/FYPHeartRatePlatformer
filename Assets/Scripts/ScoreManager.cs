using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Tracks and displays score and high score during gameplay (only in sensor mode)

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText; // UI element for displaying current score
    public TMP_Text highScoreText; // UI element for displaying high score

    private float score;
    private int highScore;
    private bool isFlashing = false;

    private bool useSensor = false;

    void Start()
    {
        // Determine if the game is being played with a sensor or simulated
        string inputMode = PlayerPrefs.GetString("InputMode", "Sensor");
        useSensor = inputMode == "Sensor";

        // Only show score UI in sensor mode
        scoreText.gameObject.SetActive(useSensor);
        highScoreText.gameObject.SetActive(useSensor);

        if (useSensor)
        {
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            UpdateHighScoreUI();
        }
    }

    void Update()
    {
        // Only process scoring in sensor mode
        if (!useSensor) return;

        // Only increase score if player is still alive
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            score += 1 * Time.deltaTime; // Increment score over time
            scoreText.text = ((int)score).ToString();

            // Update high score if beaten
            if ((int)score > highScore)
            {
                highScore = (int)score;
                PlayerPrefs.SetInt("HighScore", highScore);
                UpdateHighScoreUI();

                if (!isFlashing)
                {
                    StartCoroutine(GlowHighScore());
                }
            }
        }
    }

    // Updates high score UI text
    void UpdateHighScoreUI()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    // Coroutine to make the high score glow when updated
    IEnumerator GlowHighScore()
    {
        isFlashing = true;

        TMP_Text tmp = highScoreText;
        Color originalColor = tmp.color;
        Color glowColor = Color.red;

        float originalOutlineWidth = tmp.outlineWidth;
        Color originalOutlineColor = tmp.outlineColor;

        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 2f, 1f);

            tmp.color = Color.Lerp(originalColor, glowColor, t);
            tmp.outlineColor = Color.Lerp(originalOutlineColor, glowColor, t);
            tmp.outlineWidth = Mathf.Lerp(0f, 0.2f, t);

            yield return null;
        }
        
        // Reset text appearance
        tmp.color = originalColor;
        tmp.outlineColor = originalOutlineColor;
        tmp.outlineWidth = originalOutlineWidth;

        isFlashing = false;
    }
}
