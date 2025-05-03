using UnityEngine;
using UnityEngine.SceneManagement;

// Manages pause functionality, including resume, restart, and returning to main menu

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel; // The UI panel that appears when the game is paused
    public GameObject gameOverPanel; // Reference to the Game Over panel to avoid overlapping controls
    private bool isPaused = false; // Tracks whether the game is currently paused

    void Update()
    {

        // Don't allow pause if Game Over panel is active
        if (gameOverPanel != null && gameOverPanel.activeInHierarchy)
        return;

        // Toggle pause on ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // Resumes the game from a paused state
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Pauses the game and shows the pause menu
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Restarts the current level from the beginning
    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time in case game was paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Returns to the main menu from the pause screen
    public void MainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is normal before switching scenes
        SceneManager.LoadScene("MainMenu");
    }
}
