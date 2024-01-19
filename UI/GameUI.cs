using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public List<Sprite> menuButtonImages;
    public List<Sprite> buttonImages;

    private bool isMenuOpen, isDashboardOpen, isStageOpen, isInfoOpen;
    private GameObject canvasObject;


    public Vector2[] buttonPositions = new Vector2[]
    {
        new Vector2(0f, 0f),  // Bottom right button position
        //new Vector2(0f, 100f),  // Bottom right button position
        //new Vector2(0f, 200f),   // Bottom right button position
    };

    public Vector2 dashboardButtonPosition, menuButtonPosition, containerPosition;  // Top right menu button position

    public Vector2 screenSize;

    private void Start()
    {
        float padding = 80;
        canvasObject = Utils.CreateCanvas("GameCanvas", false);
        screenSize = Utils.GetCanvasSize(canvasObject);
        menuButtonPosition = new Vector2(screenSize.x / 2 - padding, screenSize.y / 2 - padding);
        dashboardButtonPosition = new Vector2(padding - screenSize.x / 2, screenSize.y / 2 - padding);
        containerPosition = new Vector2(screenSize.x / 2 - padding, -1 * screenSize.y / 2 + padding);
        buttonImages = DataHolder.gameButtonImages;
        menuButtonImages = DataHolder.menuButtonImages;
        // Create Canvas object

        // Create EventSystem object VERY IMPORTANT!!!
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        // Create EventSystem object VERY IMPORTANT!!!

        SetupButton();
    }

    // private void SetupDashboard()
    // {
    //     Utils.CreateImageWithText(buttonImages[0], new Vector2(-50,0), new Vector2(50,0), canvasObject.transform, TextAnchor.MiddleRight);
    // }

    private void SetupButton()
    {
        // Instantiate menu button
        GameObject settingButton = Utils.CreateButton(menuButtonImages[0], menuButtonPosition, canvasObject.transform, () => {

            isMenuOpen = !isMenuOpen;
            // Activate/deactivate the circle buttons container based on menu state
            //GameObject circleButtonsContainer = canvasObject.transform.Find("CircleButtonsContainer").gameObject;
            //circleButtonsContainer.SetActive(isMenuOpen);

            //SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
            //settingsController.canvasObject.SetActive(true);
            SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
            settingsController.canvasObject.SetActive(true);
        });
        GameObject stageButton = Utils.CreateButton(menuButtonImages[1], new Vector2(menuButtonPosition.x, menuButtonPosition.y - 110), canvasObject.transform, () => {

            isStageOpen = !isStageOpen;
            // Activate/deactivate the circle buttons container based on menu state
            //GameObject circleButtonsContainer = canvasObject.transform.Find("CircleButtonsContainer").gameObject;
            //circleButtonsContainer.SetActive(isMenuOpen);

            //SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
            //settingsController.canvasObject.SetActive(true);
            StageController stageController = Camera.main.gameObject.GetComponent<StageController>();
            stageController.canvasObject.SetActive(true);
        });
        GameObject infoButton = Utils.CreateButton(menuButtonImages[2], new Vector2(menuButtonPosition.x, menuButtonPosition.y - 220), canvasObject.transform, () => {

            isInfoOpen = !isInfoOpen;
            // Activate/deactivate the circle buttons container based on menu state
            //GameObject circleButtonsContainer = canvasObject.transform.Find("CircleButtonsContainer").gameObject;
            //circleButtonsContainer.SetActive(isMenuOpen);

            //SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
            //settingsController.canvasObject.SetActive(true);
            InfoController infoController = Camera.main.gameObject.GetComponent<InfoController>();
            infoController.canvasObject.SetActive(true);
        });

        // Instantiate menu button
        GameObject dashboardButton = Utils.CreateButton(menuButtonImages[2], dashboardButtonPosition, canvasObject.transform, () => {

            isDashboardOpen = !isDashboardOpen;

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