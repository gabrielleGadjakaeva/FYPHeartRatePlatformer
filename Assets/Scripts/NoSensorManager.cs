using UnityEngine;
using UnityEngine.SceneManagement;

// Handles logic when no heart rate sensor is detected

public class NoSensorManager : MonoBehaviour
{
    // Called when the user clicks the button to return to the main menu
    // Resumes the game time in case it was paused and loads the main menu scene
    public void MainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset before switching scenes
        SceneManager.LoadScene("MainMenu");
    }
}
