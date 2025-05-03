using UnityEngine;
using UnityEngine.SceneManagement;

// Handles level selection when playing without a heart rate sensor
// Loads the selected RR interval file and starts the game

public class LevelSelectManager : MonoBehaviour
{
    // Called when a level button is clicked
    // Stores the name of the selected RR interval file and loads the game scene
    public void SelectLevel(string rrFileName)
    {
        // Save the selected RR file name for use in game
        PlayerPrefs.SetString("SelectedRRFile", rrFileName);
        // Load main game scene
        SceneManager.LoadScene("GameScene");
    }

    // Return to main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
