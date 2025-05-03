using UnityEngine;
using UnityEngine.SceneManagement;

// Manages the game over state, including restart and returning to main menu

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // UI Panel that appears when player dies

    private void Update()
    {
        // If player no longer exists, show the game over panel
        if (GameObject.FindGameObjectWithTag("Player")== null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // Reloads the current scene to restart the level
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Loads main menu scene
    public void MainMenu()
    {
        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }

}
