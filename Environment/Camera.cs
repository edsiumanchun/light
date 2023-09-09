using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float cameraHalfWidth;
    private float backgroundLeftEdge;
    private float backgroundRightEdge;

    private void Start()
    {
        // Calculate the camera's half width based on the screen aspect ratio
        float cameraHeight = Camera.main.orthographicSize * 2f;
        cameraHalfWidth = cameraHeight * Camera.main.aspect * 0.5f;

        // Assign the left and right edges of the background
        AssignBackgroundEdges(DataHolder.background);
    }

    private void AssignBackgroundEdges(GameObject backgroundObject)
    {
        // Calculate the left and right edges of the background based on its position and scale
        backgroundLeftEdge = backgroundObject.transform.position.x - backgroundObject.transform.localScale.x * 0.5f;
        backgroundRightEdge = backgroundObject.transform.position.x + backgroundObject.transform.localScale.x * 0.5f;
    }

    private void LateUpdate()
    {
        if (DataHolder.player != null)
        {
            // Calculate the desired X position for the camera
            float desiredX = DataHolder.player.transform.position.x;

            // Apply the X position restrictions based on the background edges
            float clampedX = Mathf.Clamp(desiredX, backgroundLeftEdge + cameraHalfWidth, backgroundRightEdge - cameraHalfWidth);

            // Set the camera's position with the clamped X coordinate
            transform.position = new Vector3(desiredX, 0, transform.position.z);
        }
    }
}