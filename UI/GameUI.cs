using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public GameObject joystickPrefab; // The prefab for the joystick
    public GameObject buttonPrefab; // The prefab for the skill buttons
    public int numButtons = 4; // The number of skill buttons to create

    void Start()
    {
        // Create the joystick
        GameObject joystickObject = Instantiate(joystickPrefab);
        joystickObject.transform.SetParent(transform, false);

        // Position the joystick at the left bottom of the screen
        RectTransform joystickRect = joystickObject.GetComponent<RectTransform>();
        joystickRect.anchorMin = new Vector2(0f, 0f);
        joystickRect.anchorMax = new Vector2(0f, 0f);
        joystickRect.pivot = new Vector2(0f, 0f);
        joystickRect.anchoredPosition = new Vector2(50f, 50f);

        // Create the skill buttons
        for (int i = 0; i < numButtons; i++)
        {
            GameObject buttonObject = Instantiate(buttonPrefab);
            buttonObject.transform.SetParent(transform, false);

            // Position the button based on the index
            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1f, 0f);
            buttonRect.anchorMax = new Vector2(1f, 0f);
            buttonRect.pivot = new Vector2(1f, 0f);
            buttonRect.anchoredPosition = new Vector2(-50f - i * 100f, 50f);
        }
    }
}