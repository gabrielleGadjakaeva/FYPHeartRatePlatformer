using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Dynamically generates platforms in front of the camera
// based on either heart rate sensor input or pre-recorded RR data

public class PlatformGenerator : MonoBehaviour
{
    [Header("Platform Prefab")]
    public GameObject platformPrefab;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel;

    [Header("Platform Settings")]
    public float platformHeight = 0.5f;
    public float firstPlatformWidth = 1f;
    public float minPlatformWidth = 0.5f;
    public float maxPlatformWidth = 1.5f;
    public float minGap = 2f;
    public float maxGap = 5f;
    public float maxHeightDifference = 3.5f;
    public float firstPlatformY = -0.7f;
    public float highestPlatformY = 1f;
    public float lowestPlatformY = -3f;
    public float bufferDistanceAhead = 3f;

    // Internal tracking
    private float lastRightEdgeX;
    private float lastPlatformY;
    private bool initialized = false;

    // For simulated RR data
    private List<float> rrList = new List<float>();
    private int rrIndex = 0;
    private bool useSimulated = false;
    private bool dataEnded = false;

    void Start()
    {
        // Check if the game should use simulated or real sensor data
        string inputMode = PlayerPrefs.GetString("InputMode", "Sensor");
        useSimulated = inputMode == "Simulated";

        if (useSimulated)
        {
            LoadRRFile();
            StartCoroutine(SimulateRR());
        }

        initialized = false;
    }

    void Update()
    {
        if (dataEnded) return;
        if (!useSimulated && !HeartRateReceiver.isConnected) return;

        float cameraX = Camera.main.transform.position.x;

        if (!initialized)
        {
            SpawnFirstPlatform();
        }

        // Continuously generate platforms ahead of the camera
        while (lastRightEdgeX < cameraX + bufferDistanceAhead)
        {
            SpawnNextPlatform();
        }
    }

    // Load RR interval data from text file (for simulated mode)
    void LoadRRFile()
    {
        string fileName = PlayerPrefs.GetString("SelectedRRFile", "003.txt");
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                if (float.TryParse(line, out float rr))
                {
                    rrList.Add(rr);
                }
            }
        }
    }

    // Coroutine to simulate real-time RR and HR updates from file

    IEnumerator SimulateRR()
    {
        while (rrIndex < rrList.Count)
        {
            float rr = rrList[rrIndex];
            float hr = 60000f / rr;

            HeartRateReceiver.currentHR = hr;
            HeartRateReceiver.currentRR = rr;

            rrIndex++;
            yield return new WaitForSeconds(rr / 1000f);
        }

        // All data used = trigger level complete UI
        dataEnded = true;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }

    // Spawns the first platform at a fixed Y position
    void SpawnFirstPlatform()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, firstPlatformY, 0);
        GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        platform.transform.localScale = new Vector3(firstPlatformWidth, platformHeight, 1f);

        lastRightEdgeX = spawnPos.x + firstPlatformWidth / 2f;
        lastPlatformY = firstPlatformY;
        initialized = true;
    }

    // Spawns a new platform based on HR/RR input
    void SpawnNextPlatform()
    {
        float hr = HeartRateReceiver.currentHR;
        float rr = HeartRateReceiver.currentRR;

        if (hr <= 0 || rr <= 0) return;

        // Width based on heart rate (higher HR = smaller platform)
        float normalizedWidthHR = Mathf.InverseLerp(50f, 120f, hr);
        float currentPlatformWidth = Mathf.Lerp(maxPlatformWidth, minPlatformWidth, normalizedWidthHR);

        // Gap based on RR interval (higher RR = longer spacing)
        float normalizedRR = Mathf.InverseLerp(600f, 1000f, rr);
        float gap = Mathf.Lerp(minGap, maxGap, normalizedRR);

        float halfWidth = currentPlatformWidth / 2f;
        float spawnX = lastRightEdgeX + gap + halfWidth;

        // Vertical position based on HR
        float normalizedHR = Mathf.InverseLerp(50f, 120f, hr);
        float targetY = Mathf.Lerp(lowestPlatformY, highestPlatformY, normalizedHR);

        // Limit height jumps between platforms
        float spawnY = Mathf.Clamp(targetY, lastPlatformY - maxHeightDifference, lastPlatformY + maxHeightDifference);
        spawnY = Mathf.Clamp(spawnY, lowestPlatformY, highestPlatformY);

        // Spawn and scale platform
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);
        GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        platform.transform.localScale = new Vector3(currentPlatformWidth, platformHeight, 1f);

        // Color platform based on HR zone
        SpriteRenderer sr = platform.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            if (hr < 70f)
                sr.color = Color.green;
            else if (hr < 100f)
                sr.color = Color.yellow;
            else
                sr.color = Color.red;
        }

        // Update edge tracking
        lastRightEdgeX = spawnX + halfWidth;
        lastPlatformY = spawnY;
    }
}
