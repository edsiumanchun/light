using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class SettingsController : MonoBehaviour
{
    public List<Sprite> commonImages;
    public List<Sprite> settingButtonImages;
    public Vector2[] leftButtonPositions = new Vector2[]
    {
        new Vector2(-200f, -200f),  // Bottom right button position
        new Vector2(-200f, 0f),  // Bottom right button position
        new Vector2(-200f, 200f),   // Bottom right button position
    };
    public Vector2[] rightButtonPositions = new Vector2[]
    {
        new Vector2(200f, -200f),  // Bottom right button position
        new Vector2(200f, 0f),  // Bottom right button position
        new Vector2(200f, 200f),   // Bottom right button position
    };
    public Vector2[] buttonPositions = new Vector2[]
    {
        new Vector2(0f, 500f),   // Bottom right button position
    };

    public GameObject canvasObject;
    public AudioSource bgmAudioSource;
    public GameObject leftImage;
    public GameObject rightImage;
    public GameObject backgroundImage;
    public GameObject dialogueText;
    public GameObject dialoguePanel;
    //
    public Button closeButton;
    public Button soundVolumeUpButton;
    public Button soundVolumeDownButton;
    public Button bgmVolumeUpButton;
    public Button bgmVolumeDownButton;
    public Button localeLeftButton;
    public Button localeRightButton;

    private float soundVolume = 1.0f;
    private float bgmVolume = 1.0f;
    private string language = "English";

    private void Start()
    {
        commonImages = DataHolder.commonImages;
        settingButtonImages = DataHolder.settingButtonImages;
        // canvas
        SetupCanvas();
        // Load saved settings
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        language = PlayerPrefs.GetString("Language", "English");
        SetupButtons();
    }

    private void SetupButtons()
    {
        closeButton = UIUtils.CreateButton(commonImages[0], buttonPositions[0], canvasObject.transform);
        closeButton.onClick.AddListener(OnCloseButtonClicked);

        soundVolumeUpButton = UIUtils.CreateButton(settingButtonImages[0], rightButtonPositions[0], canvasObject.transform);
        soundVolumeDownButton = UIUtils.CreateButton(settingButtonImages[1], leftButtonPositions[0], canvasObject.transform);
        bgmVolumeUpButton = UIUtils.CreateButton(settingButtonImages[0], rightButtonPositions[1], canvasObject.transform);
        bgmVolumeDownButton = UIUtils.CreateButton(settingButtonImages[1], leftButtonPositions[1], canvasObject.transform);
        localeRightButton = UIUtils.CreateButton(settingButtonImages[0], rightButtonPositions[2], canvasObject.transform);
        localeLeftButton = UIUtils.CreateButton(settingButtonImages[1], leftButtonPositions[2], canvasObject.transform);
        // Add listeners to buttons
        soundVolumeUpButton.onClick.AddListener(OnSoundVolumeUp);
        soundVolumeDownButton.onClick.AddListener(OnSoundVolumeDown);
        bgmVolumeUpButton.onClick.AddListener(OnBGMVolumeUp);
        bgmVolumeDownButton.onClick.AddListener(OnBGMVolumeDown);
        UpdateSoundVolumeText();
        UpdateBGMVolumeText();
        //englishButton.onClick.AddListener(OnEnglishSelected);
        //japaneseButton.onClick.AddListener(OnJapaneseSelected);
        //chineseButton.onClick.AddListener(OnChineseSelected);
    }

    private void SetupCanvas()
    {
        // Create Canvas object
        canvasObject = new GameObject("Settings");
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

    //private void LoadLocalizedText()
    //{
    //    localizedText = new Dictionary<string, string>();
    //    string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "../Localization.json");
    //    AssetsData assetsData = JsonUtility.FromJson<AssetsData>(jsonFile.text);
    //    TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage);

    //    if (File.Exists(filePath))
    //    {
    //        // Read the JSON file
    //        string dataAsJson = File.ReadAllText(filePath);

    //        // Convert JSON to dictionary
    //        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

    //        for (int i = 0; i < loadedData.items.Length; i++)
    //        {
    //            localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("Cannot load localization data!");
    //    }
    //}

    private void OnSoundVolumeUp()
    {
        soundVolume += 0.1f;
        if (soundVolume > 1.0f)
        {
            soundVolume = 1.0f;
        }
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
        UpdateSoundVolumeText();
        AudioListener.volume = soundVolume;
    }

    private void OnSoundVolumeDown()
    {
        soundVolume -= 0.1f;
        if (soundVolume < 0.0f)
        {
            soundVolume = 0.0f;
        }
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
        UpdateSoundVolumeText();
        AudioListener.volume = soundVolume;
    }

    private void OnBGMVolumeUp()
    {
        bgmVolume += 0.1f;
        if (bgmVolume > 1.0f)
        {
            bgmVolume = 1.0f;
        }
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        UpdateBGMVolumeText();
        // set bgm volume in your audio manager or wherever you play your bgm
    }

    private void OnBGMVolumeDown()
    {
        bgmVolume -= 0.1f;
        if (bgmVolume < 0.0f)
        {
            bgmVolume = 0.0f;
        }
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        UpdateBGMVolumeText();
        // set bgm volume in your audio manager or wherever you play your bgm
    }

    private void OnEnglishSelected()
    {
        language = "English";
        PlayerPrefs.SetString("Language", language);
        // set language to English in your localization manager or wherever you handle your localization
    }

    private void OnJapaneseSelected()
    {
        language = "Japanese";
        PlayerPrefs.SetString("Language", language);
        // set language to Japanese in your localization manager or wherever you handle your localization
    }

    private void OnChineseSelected()
    {
        language = "Chinese";
        // set language to Chinese in your localization manager or wherever you handle your localization
    }

    private void UpdateSoundVolumeText()
    {
        // Update the text of the sound volume button
        //soundVolumeUpButton.GetComponentInChildren<Text>().text = "2" + " " +Mathf.Round(soundVolume * 10).ToString();
    }

    private void UpdateBGMVolumeText()
    {
        // Update the text of the bgm volume button
        //bgmVolumeUpButton.GetComponentInChildren<Text>().text = "1" + " " + Mathf.Round(bgmVolume * 10).ToString();
    }

    private void OnCloseButtonClicked()
    {
        // Handle the exit button click
        // Add your code to exit the dashboard controller here
        canvasObject.SetActive(false);
    }
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}

[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}