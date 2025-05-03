using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;

// Receives heart rate data via UDP sent from a Python script using the BLE Polar H10 sensor
// Manages connection detection, pause/resume logic, and updates HR/RR values in real time

public class HeartRateReceiver : MonoBehaviour
{
    // Current heart rate (bpm) and RR interval (ms), updated from UDP data
    public static float currentHR = 70f;
    public static float currentRR = 800f;
    public static bool isConnected = false;

    // Ensures only one receiver instance persists
    private static bool isReceiverActive = false;

    private UdpClient client;
    private Thread receiveThread;

    private float lastDataTime = 0f;
    private volatile bool receivedNewData = false;
    private bool tryingToReconnect = false;
    private bool gameStarted = false;

    [Header("UI")]
    public GameObject noSensorPanel; // UI Panel shown when no sensor is detected
    public TMP_Text noSensorText; // Text label to show connection status

    void Start()
    {
        // Check input mode from main menu

        string inputMode = PlayerPrefs.GetString("InputMode", "Sensor");
        if (inputMode == "Simulated")
        {
            // If playing from RR file, no sensor is needed
            if (noSensorPanel != null) noSensorPanel.SetActive(false);
            enabled = false; // Disable this script entirely
            return;
        }

        // Prevent multiple receivers in scene
        if (isReceiverActive)
        {
            Destroy(gameObject);
            return;
        }
        isReceiverActive = true;

        // Pause game until sensor is confirmed
        Time.timeScale = 0f;

        // Setup UDP socket for receiving heart rate data
        client = new UdpClient(5055);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Show connecting status
        if (noSensorPanel != null) noSensorPanel.SetActive(true);
        if (noSensorText != null) noSensorText.text = "Connecting to sensor...";

        lastDataTime = Time.time;
    }

    void Update()
    {
        // If new data has been received, update timestamp
        if (receivedNewData)
        {
            lastDataTime = Time.time;
            receivedNewData = false;
        }

        float timeSinceLastData = Time.time - lastDataTime;

        // No data for more than 5 seconds = disconnected
        if (timeSinceLastData > 5f)
        {
            if (isConnected)
            {
                isConnected = false;
                Time.timeScale = 0f;

                if (noSensorPanel != null) noSensorPanel.SetActive(true);
                if (noSensorText != null) noSensorText.text = "Sensor disconnected.";

                // Begin attempting reconnection
                if (!tryingToReconnect)
                {
                    tryingToReconnect = true;
                    InvokeRepeating(nameof(CheckReconnect), 3f, 3f);
                }
            }
        }
        // If connected for the first time, start game
        else if (!gameStarted && isConnected)
        {
            gameStarted = true;
            Time.timeScale = 1f;

            if (noSensorPanel != null) noSensorPanel.SetActive(false);
        }
    }

    // Checks if data has resumed and resumes game if reconnected
    void CheckReconnect()
    {
        if (Time.time - lastDataTime <= 2f)
        {
            isConnected = true;
            if (noSensorPanel != null) noSensorPanel.SetActive(false);
            Time.timeScale = 1f;
            tryingToReconnect = false;
            CancelInvoke(nameof(CheckReconnect));
        }
    }

    // Continuously receives data from UDP socket and parses HR/RR values
    void ReceiveData()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref endPoint);
                string text = Encoding.UTF8.GetString(data);
                string[] parts = text.Split(',');

                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float hr) &&
                    float.TryParse(parts[1], out float rr))
                {
                    currentHR = hr;
                    currentRR = rr;
                    isConnected = true;
                    receivedNewData = true;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Receive error: " + ex.Message);
            }
        }
    }

    // Cleanup on scene unload or quit
    void OnDestroy()
    {
        isReceiverActive = false;

        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();

        if (client != null)
            client.Close();

        isConnected = false;
    }
}
