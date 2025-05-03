using UnityEngine;

// Dynamically scales the game's time (speed) based on the user's live heart rate
// The higher the heart rate, the faster the game runs

public class HRTimeScaler : MonoBehaviour
{
    [Header("Heart Rate to Time Scale")]
    public float minHR = 50f; // Lowest expected heart rate
    public float maxHR = 120f; // Highest expected heart rate
    public float minTimeScale = 1f; // Normal game speed
    public float maxTimeScale = 1.5f; // Increased game speed for high HR

    void Update()
    {
        // Skip scaling if no HR data is available or if the sensor is disconnected
        if (!HeartRateReceiver.isConnected) return;

        // Do not interfere if the game is currently paused
        if (Time.timeScale == 0f)
        return;

        if (HeartRateReceiver.currentHR <= 0) return;

        float hr = HeartRateReceiver.currentHR;

        // Normalize HR between 0 and 1
        float normalizedHR = Mathf.InverseLerp(minHR, maxHR, hr);

        // Scale the game speed based on the normalized HR
        float newTimeScale = Mathf.Lerp(minTimeScale, maxTimeScale, normalizedHR);

        Time.timeScale = newTimeScale;
    }
}
