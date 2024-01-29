using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

public class SettingsController : MonoBehaviour
{
    public GameObject canvasObject;
    public AudioSource bgmAudioSource;
    public GameObject backgroundImage;

    public GameObject soundVolumeGO;
    public GameObject bgmVolumeGO;
    public GameObject languageGO;

    private float soundVolume = 1.0f;
    private float bgmVolume = 1.0f;
    private string language = "English";
    public List<Sprite> settingImages;
    public List<string> buttonTitles;


    private void Start()
    {
        language = PlayerPrefs.GetString("Language", "English");
        settingImages = DataHolder.settingImages;
        buttonTitles = new List<string>((new[] { "Clear data", "Left"}));
        SetupCanvas();
        SetupSelctionBar();
        SetupButtons();
    }

    private void SetupSelctionBar(){
        int xPos = 68;
        int yPos = -68;
        int space = 86;
        Utils.CreateNavigationBar("設定", new Vector2(0f, 193f), canvasObject.transform, () =>
        {
            canvasObject.SetActive(false);
        });

        soundVolumeGO = Utils.CreateSelectionBar("Sound", new Vector2(xPos, yPos), canvasObject.transform, () =>
        {
            CacheManager.DecreaseCache("SoundVolume", 1, 1);
            UpdateSoundVolumeText();
        }, () => {
            CacheManager.IncreaseCache("SoundVolume", 1, 5);
            UpdateSoundVolumeText();
        });

        bgmVolumeGO = Utils.CreateSelectionBar("BGM", new Vector2(xPos, yPos + space), canvasObject.transform, () =>
        {
            CacheManager.DecreaseCache("BGMVolume", 1, 1);
            UpdateBGMVolumeText();
        }, () =>
        {
            CacheManager.IncreaseCache("BGMVolume", 1, 5);
            UpdateBGMVolumeText();
        });

        languageGO = Utils.CreateSelectionBar("Language", new Vector2(xPos, yPos + space * 2), canvasObject.transform, () =>
        {
        }, () =>
        {
        });
        GameObject soundVolumeTitle = Utils.CreateText(new Vector2(150,50), new Vector2(-250, 0), soundVolumeGO.transform, TextAnchor.MiddleLeft);
        soundVolumeTitle.GetComponent<Text>().text = "Sound";
        GameObject bgmVolumeTitle = Utils.CreateText(new Vector2(150,50), new Vector2(-250, 0), bgmVolumeGO.transform, TextAnchor.MiddleLeft);
        bgmVolumeTitle.GetComponent<Text>().text = "BGM";
        GameObject languageTitle = Utils.CreateText(new Vector2(150,50), new Vector2(-250, 0), languageGO.transform, TextAnchor.MiddleLeft);
        languageTitle.GetComponent<Text>().text = "Language";
        UpdateSoundVolumeText();
        UpdateBGMVolumeText();
    }

    private void SetupButtons()
    {
        List<GameObject> buttons = new List<GameObject>();
        List<UnityAction> actions = new List<UnityAction>();
        actions.Add(
            () =>
            {
                    ConfirmController confirmController = Camera.main.gameObject.GetComponent<ConfirmController>();
                    confirmController.SetupAction(()=>{
                        Caching.ClearCache();
                        PlayerPrefs.DeleteAll();
                        MainController.LoadAssetsFromJson(0, 0 , "HomeScene");
                    }, "Confirm to delete records permanently?",
                    new List<string>(){
                        "Cancel", "Confirm"
                    });
                    confirmController.canvasObject.SetActive(true);
            }
        );
        actions.Add(
            () =>
            {
                    // ConfirmController confirmController = Camera.main.gameObject.GetComponent<ConfirmController>();
                    // confirmController.canvasObject.SetActive(true);
            }
        );
        // Create circle buttons container
        GameObject circleButtonsContainer = new GameObject("CircleButtonsContainer");
        circleButtonsContainer.transform.SetParent(canvasObject.transform, false);

        // Set position of circleButtonsContainer
        RectTransform containerRectTransform = circleButtonsContainer.AddComponent<RectTransform>();
        containerRectTransform.anchoredPosition = new Vector2(0, -157);

        // Instantiate circle buttons
        for (int i = 0; i < settingImages.Count; i++)
        {
            int buttonIndex = i; // Store the index in a local variable to capture it in the lambda expression
            GameObject buttonGO = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.LongButton)], new Vector2(-150 + i * 150, 0), containerRectTransform.transform, actions[i]);

            GameObject detailGO = Utils.CreateImageWithText(settingImages[0], new Vector2(-68, 0), new Vector2(130, 0), buttonGO.transform, TextAnchor.MiddleLeft);
            buttons.Add(buttonGO);
            Text text0 = detailGO.GetComponent<Text>();
            text0.fontSize = 28;
            text0.text = buttonTitles[i];
        }
        
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
        backgroundImage = Utils.CreateImage(DataHolder.commonImages[((int)UICommon.Panel)], backgroundPosition, canvasObject.transform);
        
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
        soundVolumeGO.GetComponent<Text>().text = soundVolume.ToString();
        AudioListener.volume = soundVolume;
    }

    private void UpdateBGMVolumeText()
    {
        bgmVolume = PlayerPrefs.GetInt("BGMVolume", 3);
        bgmVolumeGO.GetComponent<Text>().text = bgmVolume.ToString();
    }

    void OnClickButton()
    {
        // Set the active state of the game object to false
        canvasObject.SetActive(false);
    }
}


// public class RowComponent : MonoBehaviour
// {
//     public GameObject descriptionGO;
//     public GameObject valueGO;
//     public List<Sprite> commonImages;

//     public void InitializeRow(string description, string value, UnityAction onClickActionRight, UnityAction onClickActionLeft)
//     {
//         commonImages = DataHolder.commonImages;
//         // Set description text
//         descriptionGO = Utils.CreateText(new Vector2(100f, 100f), new Vector2(-150f, 0f), this.gameObject.transform, TextAnchor.MiddleLeft);
//         descriptionGO.GetComponent<Text>().text = description;

//         // Set description text
//         valueGO = Utils.CreateText(new Vector2(100f, 100f), new Vector2(80f, 0f), this.gameObject.transform, TextAnchor.MiddleCenter);
//         valueGO.GetComponent<Text>().text = value;

//         GameObject buttonLeft = Utils.CreateButton(commonImages[((int)UICommon.Left)], new Vector2(10f, 0f), this.gameObject.transform, onClickActionLeft);
//         GameObject buttonRight = Utils.CreateButton(commonImages[((int)UICommon.Right)], new Vector2(150f, 0f), this.gameObject.transform, onClickActionRight);

//     }

//     static public GameObject CreateRow(Vector2 size, Vector2 position, Transform parent, string description, string value, UnityAction onClickAction, UnityAction onClickAction2)
//     {
//         GameObject gameObject = new GameObject("Row");
//         RowComponent rowComponent = gameObject.AddComponent<RowComponent>();

//         rowComponent.InitializeRow(description, value, onClickAction, onClickAction2);
//         gameObject.transform.SetParent(parent);

//         RectTransform textTransform = gameObject.AddComponent<RectTransform>();
//         textTransform.anchoredPosition = position;
//         textTransform.sizeDelta = size;
//         return gameObject;
//     }
// }