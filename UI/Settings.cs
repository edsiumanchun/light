using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

public class SettingsController : MonoBehaviour
{
    public List<Sprite> commonImages;
    public List<Sprite> settingButtonImages;
    public Vector2[] rowPositions = new Vector2[]
    {
        new Vector2(30f, 130f),  // Bottom right button position
        new Vector2(30f, 0f),  // Bottom right button position
        new Vector2(30f, -130f),   // Bottom right button position
    };
    public Vector2[] buttonPositions = new Vector2[]
    {
        new Vector2(230f, 380f),   // Bottom right button position
    };

    public GameObject canvasObject;
    public AudioSource bgmAudioSource;
    public GameObject backgroundImage;
    public GameObject closeButton;
    public GameObject localeButton;

    public GameObject dropdownGO;

    public GameObject soundVolumeRow;
    public GameObject bgmVolumeRow;

    private float soundVolume = 1.0f;
    private float bgmVolume = 1.0f;
    private string language = "English";

    private void Start()
    {
        commonImages = DataHolder.commonImages;
        settingButtonImages = DataHolder.settingButtonImages;
        // canvas
        language = PlayerPrefs.GetString("Language", "English");
        SetupCanvas();
        SetupButtons();
    }

    private void SetupButtons()
    {
        closeButton = Utils.CreateButton(commonImages[0], buttonPositions[0], canvasObject.transform, ()=>
        {
            canvasObject.SetActive(false);
        });

        soundVolumeRow = RowComponent.CreateRow(new Vector2(260, 100), rowPositions[0], canvasObject.transform, "Sound", soundVolume.ToString(), ()=>
        {
            CacheController.DecreaseCache("SoundVolume", 1, 1);
            UpdateSoundVolumeText();
        }, () =>
        {
            CacheController.IncreaseCache("SoundVolume", 1, 5);
            UpdateSoundVolumeText();
        });
        bgmVolumeRow = RowComponent.CreateRow(new Vector2(260, 100), rowPositions[1], canvasObject.transform, "BGM", bgmVolume.ToString(), ()=>
        {
            CacheController.DecreaseCache("BGMVolume", 1, 1);
            UpdateBGMVolumeText();
        }, ()=>
        {
            CacheController.IncreaseCache("BGMVolume", 1, 5);
            UpdateBGMVolumeText();
        });

        localeButton = Utils.CreateButton(settingButtonImages[0], rowPositions[2], canvasObject.transform, ()=>
        {
            ScrollViewController scrollViewController = Camera.main.gameObject.GetComponent<ScrollViewController>();
            scrollViewController.canvasObject.SetActive(true);
        });
        UpdateSoundVolumeText();
        UpdateBGMVolumeText();
    }

    private void SetupCanvas()
    {
        canvasObject = Utils.CreateCanvas("Settings", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        if (backgroundImage != null)
        {
            return;
        };
        Vector2 backgroundPosition = new Vector2(0, 5f);
        Vector2 dialoguePosition = new Vector2(0f, 0f);
        Vector2 dialoguePanelPosition = new Vector2(0f, -Screen.height / 2);
        backgroundImage = Utils.CreateImage(DataHolder.settingBackgroundImage, backgroundPosition, canvasObject.transform);

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

    private void UpdateSoundVolumeText()
    {
        soundVolume = PlayerPrefs.GetInt("SoundVolume", 3);
        soundVolumeRow.GetComponent<RowComponent>().valueGO.GetComponent<Text>().text = soundVolume.ToString();
        AudioListener.volume = soundVolume;
    }

    private void UpdateBGMVolumeText()
    {
        bgmVolume = PlayerPrefs.GetInt("BGMVolume", 3);
        bgmVolumeRow.GetComponent<RowComponent>().valueGO.GetComponent<Text>().text = bgmVolume.ToString();
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}


public class RowComponent : MonoBehaviour
{
    public GameObject descriptionGO;
    public GameObject valueGO;
    public List<Sprite> settingButtonImages;

    public void InitializeRow(string description, string value, UnityAction onClickAction, UnityAction onClickAction2)
    {
        settingButtonImages = DataHolder.settingButtonImages;
        // Set description text
        descriptionGO = Utils.CreateText(new Vector2(100f, 100f), new Vector2(-150f, 0f), this.gameObject.transform, TextAnchor.MiddleLeft);
        descriptionGO.GetComponent<Text>().text = description;

        // Set description text
        valueGO = Utils.CreateText(new Vector2(100f, 100f), new Vector2(80f, 0f), this.gameObject.transform, TextAnchor.MiddleCenter);
        valueGO.GetComponent<Text>().text = value;

        GameObject button1 = Utils.CreateButton(settingButtonImages[0], new Vector2(150f, 0f), this.gameObject.transform, onClickAction);
        GameObject button2 = Utils.CreateButton(settingButtonImages[1], new Vector2(10f, 0f), this.gameObject.transform, onClickAction2);

    }

    static public GameObject CreateRow(Vector2 size, Vector2 position, Transform parent, string description, string value, UnityAction onClickAction, UnityAction onClickAction2)
    {
        GameObject gameObject = new GameObject("Row");
        RowComponent rowComponent = gameObject.AddComponent<RowComponent>();

        rowComponent.InitializeRow(description, value, onClickAction, onClickAction2);
        gameObject.transform.SetParent(parent);

        RectTransform textTransform = gameObject.AddComponent<RectTransform>();
        textTransform.anchoredPosition = position;
        textTransform.sizeDelta = size;
        return gameObject;
    }
}