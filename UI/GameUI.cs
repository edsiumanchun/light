using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    private GameObject playerHPBar;
    private GameObject enemyHPBar;
    // hp
    public List<Sprite> menuButtonImages;
    public List<Sprite> buttonImages;
    public GameObject gameButton, buttonImage;

    private bool isMenuOpen, isDashboardOpen, isStageOpen, isInfoOpen;
    public GameObject canvasObject;


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
        // hp
        enemyHPBar = CreateHPBar(true);
        playerHPBar = CreateHPBar(false);
        HideHPBar(true);
        // hp
    }

    public void HideHPBar(bool isHide){
        enemyHPBar.SetActive(!isHide);
        playerHPBar.SetActive(!isHide);
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
        for (int i = 0; i < 1; i++)
        {
            int buttonIndex = i; // Store the index in a local variable to capture it in the lambda expression
            var image = DataHolder.commonImages[((int)UICommon.BigCircleButton)];
            gameButton = Utils.CreateButton(image, buttonPositions[i], circleButtonsContainer.transform, () => ButtonAction(buttonIndex));
            buttonImage = Utils.CreateImage(buttonImages[i], new Vector2(0, 0), gameButton.transform);
        }
        hideGameButton();
    }

    public void hideGameButton() {
        gameButton.SetActive(false);
    }
    public void showGameButton(int index) {
        gameButton.SetActive(true);
        buttonImage.GetComponent<Image>().sprite = buttonImages[index];
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



    public void UpdateHPBar(float enemyHP)
    {
        float hpMax = CacheManager.GetCache(Cache.HPMax.ToString());
        float hp = CacheManager.GetCache(Cache.HP.ToString());
        Debug.Log(hp);
        foreach (Transform child in playerHPBar.transform) {
            if (child.name == "HPBar"){
                child.transform.localScale = new Vector3(hp / hpMax, 1f, 1f);
            }
        }
        foreach (Transform child in enemyHPBar.transform) {
            if (child.name == "HPBar"){
                child.transform.localScale = new Vector3(enemyHP, 1f, 1f);
            }
        }
    }

    private GameObject CreateHPBar(bool isEnemy)
    {
        Vector2 position = isEnemy ? new Vector2(0, - screenSize.y / 2 ) :  new Vector2(0, - screenSize.y / 2);
        int hpbar = isEnemy ? 5 : 4;
        float x = isEnemy ? 0.5f : -0.5f;
        GameObject GO = Utils.CreateImageWithText(menuButtonImages[3], position, new Vector2(130, 0), canvasObject.transform, TextAnchor.MiddleCenter);
        GO.transform.parent.gameObject.name = "HPBarContainer";

        // barGO
        GameObject barGO = Utils.CreateImage(menuButtonImages[hpbar], new Vector2(0, 0), GO.transform.parent.gameObject.transform);
        barGO.name = "HPBar";
        RectTransform rectTransform = barGO.GetComponent<RectTransform>();

        Vector3 currentPosition = rectTransform.position;
        rectTransform.pivot = isEnemy ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        rectTransform.position = currentPosition;

        // rectTransform.anchorMin = isEnemy ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        // rectTransform.anchorMax = isEnemy ? new Vector2(0f, 1f) : new Vector2(1f, 1f);

        Utils.MovePrefab(GO.transform.parent.gameObject, x, 0.5f);
        Utils.MovePrefab(barGO, isEnemy ? -0.5f : 0.5f, -0.5f);
        // barGO.transform.localPosition = new Vector2(0f, 0f);
        return GO.transform.parent.gameObject;
    }

}