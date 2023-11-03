using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class DashboardController : MonoBehaviour
{
    public List<Sprite> commonImages;
    public List<Sprite> dashboardImages;
    public List<Sprite> dashboardButtonImages;
    public Vector2[] infoPositions = new Vector2[]
    {
        new Vector2(0, -200f),  // Bottom right button position
        new Vector2(0, 0f),  // Bottom right button position
        new Vector2(0, 200f),   // Bottom right button position
    };
    public Vector2[] buttonPositions = new Vector2[]
    {
        new Vector2(0f, 500f),   // Bottom right button position
        new Vector2(200f, -500f),  // Bottom right button position
    };

    public GameObject canvasObject;
    public AudioSource bgmAudioSource;
    public GameObject leftImage;
    public GameObject rightImage;
    public GameObject backgroundImage;
    public GameObject dialogueText;
    public GameObject dialoguePanel;
    //
    public Button settingButton;
    public Button closeButton;

    public Text copperKeyText;
    public Text silverKeyText;
    public Text goldKeyText;

    private void Start()
    {
        commonImages = DataHolder.commonImages;
        dashboardImages = DataHolder.dashboardImages;
        dashboardButtonImages = DataHolder.dashboardButtonImages;
        // canvas
        SetupCanvas();
        SetupInfo();
        SetupButtons();
    }

    private void SetupInfo()
    {
        copperKeyText = UIUtils.CreateRow(dashboardImages[0], infoPositions[0], canvasObject.transform);
        silverKeyText = UIUtils.CreateRow(dashboardImages[1], infoPositions[1], canvasObject.transform);
        goldKeyText = UIUtils.CreateRow(dashboardImages[2], infoPositions[2], canvasObject.transform);
        int copperKeyCount = 10;
        int silverKeyCount = 5;
        int goldKeyCount = 2;
        // Update the key count texts
        copperKeyText.text = copperKeyCount.ToString();
        silverKeyText.text = silverKeyCount.ToString();
        goldKeyText.text = goldKeyCount.ToString();

    }

    private void SetupButtons()
    {
        closeButton = UIUtils.CreateButton(commonImages[0], buttonPositions[0], canvasObject.transform);
        settingButton = UIUtils.CreateButton(dashboardButtonImages[0], buttonPositions[1], canvasObject.transform);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        settingButton.onClick.AddListener(OnSettingsButtonClicked);
    }

    private void SetupCanvas()
    {
        // Create Canvas object
        canvasObject = new GameObject("Dashboard");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        //
        if (backgroundImage != null)
        {
            return;
        };
        var elementWidth = 360;
        var elementHeight = elementWidth * 2 / 3;
        Vector2 backgroundPosition = new Vector2(0, 5f);
        Vector2 leftPosition = new Vector2(-Screen.width / 2f + elementWidth / 2f, 0f);
        Vector2 rightPosition = new Vector2(Screen.width / 2f - elementWidth / 2f, 0f);
        Vector2 dialoguePosition = new Vector2(0f, 0f);
        Vector2 dialoguePanelPosition = new Vector2(0f, -Screen.height / 2);

        backgroundImage = Utils.CreateImage(canvasObject.transform, backgroundPosition, new Vector2(Screen.width, Screen.height));
        leftImage = Utils.CreateImage(canvasObject.transform, leftPosition, new Vector2(elementWidth, elementWidth));
        rightImage = Utils.CreateImage(canvasObject.transform, rightPosition, new Vector2(elementWidth, elementWidth));

        dialogueText = Utils.CreateText(canvasObject.transform, dialoguePosition, new Vector2(Screen.width, Screen.height / 2));

        dialoguePanel = new GameObject("DialoguePanel");
        // parent
        RectTransform dialoguePanelTransform = dialoguePanel.AddComponent<RectTransform>();
        dialoguePanelTransform.SetParent(canvasObject.transform); // Set the parent transform
        dialoguePanelTransform.localPosition = dialoguePanelPosition;

        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        // Handle the exit button click
        // Add your code to exit the dashboard controller here
        canvasObject.SetActive(false);
    }

    private void OnSettingsButtonClicked()
    {
        SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
        settingsController.canvasObject.SetActive(true);
    }
}
