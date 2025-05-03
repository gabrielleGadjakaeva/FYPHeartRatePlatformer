using UnityEngine;
using UnityEngine.SceneManagement;

// Handles main menu interactions. Selecting input mode and navigating scenes

public class MainMenuManager : MonoBehaviour
{
    // Called when the "Play with Sensor" button is clicked
    // Sets input mode to use the real heart rate sensor and loads the game scene
    public void PlayWithSensor()
    {
        PlayerPrefs.SetString("InputMode", "Sensor");
        SceneManager.LoadScene("GameScene");
    }

    // Called when the "Play without Sensor" button is clicked
    // Sets input mode to simulated data and loads the level selection screen
    public void PlayWithoutSensor()
    {
        PlayerPrefs.SetString("InputMode", "Simulated");
        SceneManager.LoadScene("LevelSelect");
    }
}
