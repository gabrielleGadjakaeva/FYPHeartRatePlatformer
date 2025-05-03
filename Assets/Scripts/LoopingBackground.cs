using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Continuously scrolls the background texture to create a looping effect

public class LoopingBackground : MonoBehaviour
{

    public float backgroundSpeed = 4; // Speed of background scrolling
    public Renderer backgroundRenderer; // Reference to the Renderer component with the background material

    void Update()
    {
        // Shift the background texture to the left over time to simulate movement
        backgroundRenderer.material.mainTextureOffset += new Vector2(backgroundSpeed * Time.deltaTime, 0f);
    }
}
