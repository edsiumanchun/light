using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class DashboardController : MonoBehaviour
{
    public List<Sprite> commonImages;
    public List<Sprite> keyImages;
    public List<Sprite> dashboardButtonImages;
    public Vector2 goldPosition = new Vector2(-80f, 88f);
    public Vector2[] infoPositions = new Vector2[]
    {
        new Vector2(-210f, -80f),  // Bottom right button position
        new Vector2(-70f, -80f),  // Bottom right button position
        new Vector2(70f, -80f),   // Bottom right button position
        new Vector2(210f, -80f),   // Bottom right button position
    };
    public Vector2[] buttonPositions = new Vector2[]
    {
        new Vector2(218f, 88f),   // Bottom right button position
        new Vector2(118f, 88f),  // Bottom right button position
    };

    /// ////////
    public GameObject canvasObject;
    public AudioSource bgmAudioSource;
    public GameObject backgroundImage;
    public GameObject coinTextImage;
    public List<GameObject> keyTextImages;
    //
    public List<GameObject> buttons;

    private void Start()
    {
        commonImages = DataHolder.commonImages;
        keyImages = DataHolder.keyImages;
        dashboardButtonImages = DataHolder.dashboardButtonImages;
        // canvas
        SetupCanvas();
        SetupInfo();
        SetupButtons();
        SetText();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Dashboard", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        Vector2 backgroundPosition = new Vector2(0, 0f);

        backgroundImage = Utils.CreateImage(DataHolder.dashboardBackgroundImages[0], backgroundPosition, canvasObject.transform);
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);
    }


    private void SetupInfo()
    {
        keyTextImages = new List<GameObject>();
        Utils.CreateImage(keyImages[0], infoPositions[0], canvasObject.transform);
        Utils.CreateImage(keyImages[1], infoPositions[1], canvasObject.transform);
        Utils.CreateImage(keyImages[2], infoPositions[2], canvasObject.transform);
        Utils.CreateImage(keyImages[3], infoPositions[3], canvasObject.transform);
        keyTextImages.Add(Utils.CreateTextImage(DataHolder.dashboardBackgroundImages[2], new Vector2(infoPositions[0].x, infoPositions[0].y - 130), canvasObject.transform));
        keyTextImages.Add(Utils.CreateTextImage(DataHolder.dashboardBackgroundImages[2], new Vector2(infoPositions[1].x, infoPositions[1].y - 130), canvasObject.transform));
        keyTextImages.Add(Utils.CreateTextImage(DataHolder.dashboardBackgroundImages[2], new Vector2(infoPositions[2].x, infoPositions[2].y - 130), canvasObject.transform));
        keyTextImages.Add(Utils.CreateTextImage(DataHolder.dashboardBackgroundImages[2], new Vector2(infoPositions[3].x, infoPositions[3].y - 130), canvasObject.transform));

        coinTextImage = Utils.CreateImageWithText(DataHolder.dashboardBackgroundImages[1], goldPosition, new Vector2(50, 0), canvasObject.transform, TextAnchor.MiddleLeft);
    }

    public void SetText()
    {
        int copperKeyCount = CacheManager.GetCache("Copper");
        int silverKeyCount = CacheManager.GetCache("Silver");
        int goldKeyCount = CacheManager.GetCache("Gold");
        int soul = 32;
        int coin = 2;
        // Update the key count texts
        keyTextImages[0].GetComponent<Text>().text = copperKeyCount.ToString();
        keyTextImages[1].GetComponent<Text>().text = silverKeyCount.ToString();
        keyTextImages[2].GetComponent<Text>().text = goldKeyCount.ToString();
        keyTextImages[3].GetComponent<Text>().text = soul.ToString();
        coinTextImage.GetComponent<Text>().text = coin.ToString();

    }

    private void SetupButtons()
    {
        buttons = new List<GameObject>();
        Utils.CreateNavigationBar("茶茶的魔法世界", new Vector2(0f, 278f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });
        buttons.Add(Utils.CreateTextButton(dashboardButtonImages[0], buttonPositions[0], new Vector2(0, -38), canvasObject.transform, () =>
        {
            StageController stageController = Camera.main.gameObject.GetComponent<StageController>();
            stageController.canvasObject.SetActive(true);
        }));
        buttons.Add(Utils.CreateTextButton(dashboardButtonImages[1], buttonPositions[1], new Vector2(0, -38), canvasObject.transform, () =>
        {
            SettingsController settingsController = Camera.main.gameObject.GetComponent<SettingsController>();
            settingsController.canvasObject.SetActive(true);
        }));
        Text text0 = buttons[0].GetComponent<Text>();
        Text text1 = buttons[1].GetComponent<Text>();
        text0.fontSize = 28;
        text1.fontSize = 28;
        text0.text = "setup";
        text1.text = "info";
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}
