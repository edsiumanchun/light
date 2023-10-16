using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewController : MonoBehaviour
{
    private int index = 0; // the current index
    public GameObject player; // Reference to the player object
    public float backgroundWidth;
    public Transform backgroundObject; // Reference to the background object
    public float animationDuration = 0.08f; // Duration of the camera animation

    private Vector3 initialCameraPosition; // Initial position of the camera
    private Vector3 targetCameraPosition; // Target position for the camera animation
    private bool isAnimating; // Flag to check if camera animation is in progress

    public void SetBackgroundWidth()
    {
        Sprite backgroundSprite = DataHolder.backgrounds[0];
        backgroundWidth = backgroundSprite.bounds.size.x;
    }

    private void Start()
    {
        initialCameraPosition = transform.position;
    }

    void Update()
    {
        if (player == null || isAnimating)
        {
            return;
        }
        float playerX = player.transform.position.x;
        // Check if the player has crossed the threshold going right
        if (playerX > backgroundWidth * (0.5f + index))
        {
            index++;
            float newXPosition = backgroundWidth * index;
            targetCameraPosition = new Vector3(newXPosition, 0, initialCameraPosition.z);
            StartCoroutine(AnimateCamera());
        }
        // Check if the player has crossed the threshold going left
        else if (index > 0 && playerX < backgroundWidth * (-0.5f + index))
        {
            index--;
            float newXPosition = backgroundWidth * index;
            targetCameraPosition = new Vector3(newXPosition, 0, initialCameraPosition.z);
            StartCoroutine(AnimateCamera());
        }
    }

    private IEnumerator AnimateCamera()
    {
        isAnimating = true;

        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < animationDuration)
        {
            // Calculate the new position based on the animation curve
            float t = elapsedTime / animationDuration;
            transform.position = Vector3.Lerp(startingPosition, targetCameraPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;

            // lock gate
            GameObject gateObject = GameObject.Find("Trigger");
            gateObject.GetComponent<GateTriggerController>().ResetPositions();
        }

        transform.position = targetCameraPosition;
        isAnimating = false;
    }
}