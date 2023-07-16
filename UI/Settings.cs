using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class SettingsController : MonoBehaviour
{
    public Button soundVolumeUpButton;
    public Button soundVolumeDownButton;
    public Button bgmVolumeUpButton;
    public Button bgmVolumeDownButton;
    public Button englishButton;
    public Button japaneseButton;
    public Button chineseButton;

    private float soundVolume = 1.0f;
    private float bgmVolume = 1.0f;
    private string language = "English";
    private Dictionary<string, string> localizedText;

    private void Start()
    {
        // Load saved settings
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        language = PlayerPrefs.GetString("Language", "English");

        // Load localized text
        LoadLocalizedText();

        // Set initial values for buttons
        UpdateSoundVolumeText();
        UpdateBGMVolumeText();
        UpdateLanguageText();

        // Add listeners to buttons
        soundVolumeUpButton.onClick.AddListener(OnSoundVolumeUp);
        soundVolumeDownButton.onClick.AddListener(OnSoundVolumeDown);
        bgmVolumeUpButton.onClick.AddListener(OnBGMVolumeUp);
        bgmVolumeDownButton.onClick.AddListener(OnBGMVolumeDown);
        englishButton.onClick.AddListener(OnEnglishSelected);
        japaneseButton.onClick.AddListener(OnJapaneseSelected);
        chineseButton.onClick.AddListener(OnChineseSelected);
    }

    private void LoadLocalizedText()
    {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, "../Localization.json");

        if (File.Exists(filePath))
        {
            // Read the JSON file
            string dataAsJson = File.ReadAllText(filePath);

            // Convert JSON to dictionary
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        else
        {
            Debug.LogError("Cannot load localization data!");
        }
    }

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
        UpdateLanguageText();
        // set language to English in your localization manager or wherever you handle your localization
    }

    private void OnJapaneseSelected()
    {
        language = "Japanese";
        PlayerPrefs.SetString("Language", language);
        UpdateLanguageText();
        // set language to Japanese in your localization manager or wherever you handle your localization
    }

    private void OnChineseSelected()
    {
        language = "Chinese";
        PlayerPrefs.SetString("Language", language);
        UpdateLanguageText();
        // set language to Chinese in your localization manager or wherever you handle your localization
    }

    private void UpdateSoundVolumeText()
    {
        // Update the text of the sound volume button
        string localizedTextKey = "sound_volume";
        string localizedTextValue = GetLocalizedText(localizedTextKey);
        soundVolumeUpButton.GetComponentInChildren<Text>().text = localizedTextValue + " " +Mathf.Round(soundVolume * 10).ToString();
    }

    private void UpdateBGMVolumeText()
    {
        // Update the text of the bgm volume button
        string localizedTextKey = "bgm_volume";
        string localizedTextValue = GetLocalizedText(localizedTextKey);
        bgmVolumeUpButton.GetComponentInChildren<Text>().text = localizedTextValue + " " + Mathf.Round(bgmVolume * 10).ToString();
    }

    private void UpdateLanguageText()
    {
        // Update the text of the language buttons
        string englishText = GetLocalizedText("english");
        string japaneseText = GetLocalizedText("japanese");
        string chineseText = GetLocalizedText("chinese");

        englishButton.GetComponentInChildren<Text>().text = englishText;
        japaneseButton.GetComponentInChildren<Text>().text = japaneseText;
        chineseButton.GetComponentInChildren<Text>().text = chineseText;
    }

    private string GetLocalizedText(string key)
    {
        // Get the localized text for the given key and language
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        else
        {
            Debug.LogError("Cannot find localized text for key: " + key);
            return "";
        }
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