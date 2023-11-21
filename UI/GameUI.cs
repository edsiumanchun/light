using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Sprite menuButtonImage;
    public List<Sprite> buttonImages;

    private bool isMenuOpen;
    private GameObject canvasObject;


    public Vector2[] buttonPositions = new Vector2[]
    {
        new Vector2(0f, 0f),  // Bottom right button position
        //new Vector2(0f, 100f),  // Bottom right button position
        //new Vector2(0f, 200f),   // Bottom right button position
    };

    public Vector2 menuButtonPosition, containerPosition;  // Top right menu button position

    public Vector2 screenSize;

    private void Start()
    {
        canvasObject = Utils.CreateCanvas("GameCanvas", false);
        screenSize = Utils.GetCanvasSize(canvasObject);
        menuButtonPosition = new Vector2(screenSize.x / 2, screenSize.y / 2);
        containerPosition = new Vector2(screenSize.x / 2, -1 * screenSize.y / 2);
        buttonImages = DataHolder.gameButtonImages;
        menuButtonImage = DataHolder.menuButtonImage;
        // Create Canvas object

        // Create EventSystem object
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Set Canvas as parent for EventSystem
        eventSystemObject.transform.SetParent(canvasObject.transform);

        // Instantiate menu button
        GameObject menuButton = Utils.CreateButton(menuButtonImage, menuButtonPosition, canvasObject.transform, () => {

            isMenuOpen = !isMenuOpen;

            // Activate/deactivate the circle buttons container based on menu state
            //GameObject circleButtonsContainer = canvasObject.transform.Find("CircleButtonsContainer").gameObject;
            //circleButtonsContainer.SetActive(isMenuOpen);

            //SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
            //settingsController.canvasObject.SetActive(true);

            DashboardController dashboardController = Camera.main.gameObject.GetComponent<DashboardController>();
            dashboardController.SetText();
            dashboardController.canvasObject.SetActive(true);
        });

        // Create circle buttons container
        GameObject circleButtonsContainer = new GameObject("CircleButtonsContainer");
        circleButtonsContainer.transform.SetParent(canvasObject.transform, false);

        // Set position of circleButtonsContainer
        RectTransform containerRectTransform = circleButtonsContainer.AddComponent<RectTransform>();
        containerRectTransform.anchoredPosition = containerPosition;

        // Instantiate circle buttons
        for (int i = 0; i < buttonImages.Count; i++)
        {
            int buttonIndex = i; // Store the index in a local variable to capture it in the lambda expression
            GameObject button = Utils.CreateButton(buttonImages[i], buttonPositions[i], circleButtonsContainer.transform, () => ButtonAction(buttonIndex));
        }
    }

    private void ButtonAction(int buttonIndex)
    {
        JoystickController joystickController = GameObject.Find("Player").GetComponent<JoystickController>();
        switch (buttonIndex)
        {
            case 0:
                joystickController.Purify();
                break;
            case 1:
                joystickController.Crouch();
                break;
            case 2:
                joystickController.Jump();
                break;
            default:
                Debug.Log("Button " + buttonIndex + " pressed.");
                break;
        }
    }
}