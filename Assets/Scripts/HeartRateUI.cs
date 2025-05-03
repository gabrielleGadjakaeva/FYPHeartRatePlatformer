using UnityEngine;
using TMPro;

// Displays the current heart rate in the game's UI, based on data from the HeartRateReceiver

public class HeartRateUI : MonoBehaviour
{
    public TMP_Text heartRateText; // Reference to the TextMeshPro UI element

    void Update()
    {
        // Retrieve current HR value from the static HeartRateReceiver
        float hr = HeartRateReceiver.currentHR;

        // Display heart rate as a rounded integer
        heartRateText.text = $"HR: {Mathf.RoundToInt(hr)}";
    }
}
