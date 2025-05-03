using UnityEngine;
using UnityEngine.SceneManagement;

// Manages the transition after the player completes a level

public class LevelCompleteManager : MonoBehaviour
{
    // Called by a UI button to return to the main menu.
    // Also resets the game's time scale in case it was altered
    public void MainMenu()
    {
        Time.timeScale = 1f; // Ensure game isn't paused or slowed
        SceneManager.LoadScene("MainMenu"); // Load the main menu scene
    }
}
