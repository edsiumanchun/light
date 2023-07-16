using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The object the camera should follow
    public Vector3 offset; // The offset from the target position
    public float smoothTime = 0.3f; // The smoothing time for the camera movement

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Smoothly move the camera towards the target position
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}