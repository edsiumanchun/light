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
        new Vector2(0f, 150f),  // Bottom right button position
        new Vector2(0f, 300f),   // Bottom right button position
    };

    public Vector2 menuButtonPosition = new Vector2(560f, 300f);  // Top right menu button position
    public Vector2 containerPosition = new Vector2(560f, -300f);  // Top right menu button position

    private void Start()
    {
        buttonImages = DataHolder.gameButtonImages;
        menuButtonImage = DataHolder.menuButtonImage;
        // Create Canvas object
        canvasObject = new GameObject("GameCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Create EventSystem object
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Set Canvas as parent for EventSystem
        eventSystemObject.transform.SetParent(canvasObject.transform);

        // Instantiate menu button
        Button menuButton = UIUtils.CreateButton(menuButtonImage, menuButtonPosition, canvasObject.transform);
        menuButton.onClick.AddListener(ToggleMenu);

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
            Button button = UIUtils.CreateButton(buttonImages[i], buttonPositions[i], circleButtonsContainer.transform);
            button.onClick.AddListener(() => ButtonAction(buttonIndex));
        }
    }


    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        // Activate/deactivate the circle buttons container based on menu state
        //GameObject circleButtonsContainer = canvasObject.transform.Find("CircleButtonsContainer").gameObject;
        //circleButtonsContainer.SetActive(isMenuOpen);

        //SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
        //settingsController.canvasObject.SetActive(true);

        DashboardController dashboardController = Camera.main.gameObject.GetComponent<DashboardController>();
        dashboardController.canvasObject.SetActive(true);
    }

    private void ButtonAction(int buttonIndex)
    {
        JoystickController joystickController = GameObject.Find("Player").GetComponent<JoystickController>();
        switch (buttonIndex)
        {
            case 0:
                joystickController.Jump();
                break;
            case 1:
                joystickController.Crouch();
                break;
            case 2:
                joystickController.Purify();
                break;
            default:
                Debug.Log("Button " + buttonIndex + " pressed.");
                break;
        }
    }
}