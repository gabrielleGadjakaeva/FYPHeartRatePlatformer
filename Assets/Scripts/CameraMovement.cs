using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the side-scrolling movement of the camera

public class CameraMovement : MonoBehaviour
{

    public float cameraSpeed = 3f; // Speed at which camera moves horizontally
    void Update()
    {
        // Move the camera to the right every frame, scaled by deltaTime for frame rate independence
        transform.position += new Vector3(cameraSpeed * Time.deltaTime, 0, 0);
    }
}
