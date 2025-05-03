using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Automatically destroys platforms that have moved off the left side of the screen
// Prevents memory buildup from unused GameObjects

public class PlatformDestroyer : MonoBehaviour
{
    void Update()
    {
        // Convert the platform's position to the camera's viewport space
        Vector3 platformViewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        // Check if the platform is to the left of the camera
        if (platformViewportPosition.x < -0.5)
        {
            // Destroy the platform if it's off the left side of the screen
            Destroy(gameObject);
        }
    }
}
