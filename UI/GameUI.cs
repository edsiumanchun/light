using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    private GameObject playerHPBar;
    private GameObject playerMPBar;
    private GameObject enemyHPBar;
    private GameObject enemyHPBarText, playerMPBarText, playerHPBarText;
    // hp
    public List<Sprite> menuButtonImages;
    public List<Sprite> buttonImages;
    public GameObject gameButton, buttonImage;
    public GameObject avatarButton, settingButton, stageButton, infoButton, bagButton, circleButtonsContainer;

    private bool isMenuOpen, isavatarOpen, isStageOpen, isInfoOpen, isItemOpen;
    public GameObject canvasObject;



    public Vector2[] buttonPositions = new Vector2[]
    {
        new Vector2(0f, 0f),  // Bottom right button position
        new Vector2(-130f, 0f),  // Bottom right button position
        //new Vector2(0f, 200f),   // Bottom right button position
    };

    public Vector2 avatarButtonPosition, menuButtonPosition, containerPosition;  // Top right menu button position

    public Vector2 screenSize;

    private void Start()
    {
        float padding = 90;
        float leftPadding = 38;
        canvasObject = Utils.CreateCanvas("GameCanvas", false);
        screenSize = Utils.GetCanvasSize(canvasObject);
        menuButtonPosition = new Vector2(screenSize.x / 2 - padding , screenSize.y / 2 - padding + 10);
        avatarButtonPosition = new Vector2(leftPadding + padding - screenSize.x / 2, screenSize.y / 2 - padding);
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
        float hpBarOffset = -74;
        float xScale = 0.705f;
        enemyHPBar = CreateBar(true, new Vector2(0, - screenSize.y / 2 ), 5, canvasObject);
        enemyHPBarText = Utils.CreateText(new Vector2(200,50), new Vector2(0,0), enemyHPBar.transform, TextAnchor.MiddleCenter);
        playerHPBar = CreateBar(false, new Vector2(0, hpBarOffset), 4, avatarButton);
        playerHPBar.transform.localScale = new Vector3(xScale, 1, 1);
        playerHPBarText = Utils.CreateText(new Vector2(200,50), new Vector2(0,0), playerHPBar.transform, TextAnchor.MiddleCenter);
        playerMPBar = CreateBar(false, new Vector2(0, hpBarOffset - 16), 6, avatarButton);
        playerMPBar.transform.localScale = new Vector3(xScale, 1, 1);
        playerMPBarText = Utils.CreateText(new Vector2(200,50), new Vector2(0,0), playerMPBar.transform, TextAnchor.MiddleCenter);
        int fontSize = 18;
        enemyHPBarText.GetComponent<Text>().fontSize = fontSize;
        playerHPBarText.GetComponent<Text>().fontSize = fontSize;
        playerMPBarText.GetComponent<Text>().fontSize = fontSize;
        DisplayHPMPInfo();
        HideEnemyHPBar(true);
        // hp
    }

    public void HideEnemyHPBar(bool isHide){
        enemyHPBar.SetActive(!isHide);
        circleButtonsContainer.SetActive(isHide);
        settingButton.SetActive(isHide);
        infoButton.SetActive(isHide);
        stageButton.SetActive(isHide);
    }

    // private void Setupavatar()
    // {
    //     Utils.CreateImageWithText(buttonImages[0], new Vector2(-50,0), new Vector2(50,0), canvasObject.transform, TextAnchor.MiddleRight);
    // }

    private void SetupButton()
    {
        float yOffset = 110f;
        // Instantiate menu button
        settingButton = Utils.CreateButton(menuButtonImages[0], menuButtonPosition, canvasObject.transform, () => {

            isMenuOpen = !isMenuOpen;
            SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
            settingsController.canvasObject.SetActive(true);
        });
        stageButton = Utils.CreateButton(menuButtonImages[1], new Vector2(menuButtonPosition.x, menuButtonPosition.y - yOffset), canvasObject.transform, () => {

            isStageOpen = !isStageOpen;
            StageController stageController = Camera.main.gameObject.GetComponent<StageController>();
            stageController.canvasObject.SetActive(true);
        });
        // bagButton = Utils.CreateButton(menuButtonImages[8], new Vector2(118 - screenSize.x / 2, menuButtonPosition.y - yOffset * 2), canvasObject.transform, () => {

        // });


        infoButton = Utils.CreateButton(menuButtonImages[2], new Vector2(menuButtonPosition.x, menuButtonPosition.y - yOffset * 2), canvasObject.transform, () => {
            isInfoOpen = !isInfoOpen;
            InfoController infoController = Camera.main.gameObject.GetComponent<InfoController>();
            infoController.DisplayPage();
            infoController.canvasObject.SetActive(true);
        });

        // Instantiate menu button
        avatarButton = Utils.CreateButton(menuButtonImages[7], avatarButtonPosition, canvasObject.transform, () => {
            isItemOpen = !isItemOpen;
            ItemController itemController = Camera.main.gameObject.GetComponent<ItemController>();
            itemController.DisplayPage();
            itemController.canvasObject.SetActive(true);
        });

        // Create circle buttons container
        circleButtonsContainer = new GameObject("CircleButtonsContainer");
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
        if(gameButton != null) gameButton.SetActive(false);
        // if(infoButton != null) infoButton.SetActive(false);
    }
    public void showGameButton(int index) {
        if(gameButton == null) return;
        gameButton.SetActive(true);
        buttonImage.GetComponent<Image>().sprite = buttonImages[index];
        // if(index == (int)UIGameButton.Purify) {
        //     infoButton.SetActive(true);
        // }
    }

    private void ButtonAction(int buttonIndex)
    {
        Debug.Log("Button " + buttonIndex + " pressed.");
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
                break;
        }
    }

    public void DisplayHPMPInfo(){
        int hpMax = CacheManager.GetCache(Cache.HPMax.ToString());
        int hp = CacheManager.GetCache(Cache.HP.ToString());
        int mpMax = CacheManager.GetCache(Cache.MPMax.ToString());
        int mp = CacheManager.GetCache(Cache.MP.ToString());
        playerHPBarText.GetComponent<Text>().text = hp.ToString() + "/" + hpMax.ToString();
        playerMPBarText.GetComponent<Text>().text = mp.ToString() + "/" + mpMax.ToString();
        foreach (Transform child in playerHPBar.transform) {
            if (child.name == "HPBar"){
                child.transform.localScale = new Vector3((float)hp / hpMax, 1f, 1f);
            }
        }
        foreach (Transform child in playerMPBar.transform) {
            if (child.name == "HPBar"){
                child.transform.localScale = new Vector3((float)mp / mpMax, 1f, 1f);
            }
        }
    }

    public void UpdateAllHPMPBar(int enemyHP, int enemyMaxHP)
    {
        DisplayHPMPInfo();
        int hpMax = CacheManager.GetCache(Cache.HPMax.ToString());
        int hp = CacheManager.GetCache(Cache.HP.ToString());
        enemyHPBarText.GetComponent<Text>().text = enemyHP.ToString() + "/" + enemyMaxHP.ToString();
        foreach (Transform child in enemyHPBar.transform) {
            if (child.name == "HPBar"){
                float ratio = (float)enemyHP / enemyMaxHP;
                child.transform.localScale = new Vector3(ratio, 1f, 1f);
            }
        }
    }

    private GameObject CreateBar(bool isEnemy, Vector2 position, int hpbarType, GameObject parent)
    {   
        GameObject GO = Utils.CreateImageWithText(menuButtonImages[3], position, new Vector2(130, 0), parent.transform, TextAnchor.MiddleCenter);
        GO.transform.parent.gameObject.name = "HPBarContainer";

        // barGO
        GameObject barGO = Utils.CreateImage(menuButtonImages[hpbarType], new Vector2(0, 0), GO.transform.parent.gameObject.transform);
        barGO.name = "HPBar";
        RectTransform rectTransform = barGO.GetComponent<RectTransform>();

        Vector3 currentPosition = rectTransform.position;
        rectTransform.pivot = new Vector2(0f, 0f) ;
        rectTransform.position = currentPosition;

        // rectTransform.anchorMin = isEnemy ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        // rectTransform.anchorMax = isEnemy ? new Vector2(0f, 1f) : new Vector2(1f, 1f);

        Utils.MovePrefab(GO.transform.parent.gameObject, 0, 0.5f);
        Utils.MovePrefab(barGO, -0.5f , -0.5f);
        // barGO.transform.localPosition = new Vector2(0f, 0f);
        return GO.transform.parent.gameObject;
    }

}