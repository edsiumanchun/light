
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class StoryDataWrapper
{
    public List<StoryData> storyArray;
}

[System.Serializable]
public class StoryData
{
    public string name;
    public string imagePath;
    public string backgroundPath;
    public string bgmPath;
    public string[] dialogue;
}

public class StoryController : MonoBehaviour
{
    public Texture2D leftGifTexture;
    public Texture2D rightGifTexture;
    public GameObject leftImage;
    public GameObject rightImage;
    public GameObject backgroundImage;
    public GameObject dialogueText;
    public GameObject dialoguePanel;
    private Queue<string> sentences = new Queue<string>();
    private bool isTyping = false;
    private int currentStoryIndex;
    public StoryDataWrapper wrapper;
    public string currentSentence = "";

    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public Vector2 screenSize;

    private void Start()
    {
        // Create Canvas object
        canvasObject = Utils.CreateCanvas("StoryCanvas", true);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        // setup
        sentences = new Queue<string>();
        isTyping = false;
        currentStoryIndex = 0;
        //
        if(backgroundImage != null) {
            return;
        };
        screenSize = Utils.GetCanvasSize(canvasObject);
        Vector2 backgroundPosition = new Vector2(0, -screenSize.y / 2);
        Vector2 dialoguePosition = new Vector2(0f, 0f);
        Vector2 dialoguePanelPosition = new Vector2(0f, -Screen.height / 2);

        backgroundImage = Utils.CreateImage(DataHolder.storyImages[0], backgroundPosition, canvasObject.transform);
        Utils.MoveImage(backgroundImage, 0, 0.5f);
        RectTransform imageRectTransform = backgroundImage.GetComponent<RectTransform>();
        dialogueText = Utils.CreateText(
            new Vector2(imageRectTransform.sizeDelta.x * 7.0f / 8.8f, imageRectTransform.sizeDelta.y * 7.0f / 8.8f),
            imageRectTransform.localPosition, canvasObject.transform, TextAnchor.UpperLeft);

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

    public void LoadStoryDataFromJSON(string jsonFilePath)
    {
        canvasObject.SetActive(true);
        string storyFilePath = "Json/Story/" + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + jsonFilePath;

        //if (File.Exists(jsonFilePath))
        //{
        //    string jsonString = File.ReadAllText(jsonFilePath);
        //    storyArray = JsonConvert.DeserializeObject<StoryData[]>(jsonString);
        //}
        //else
        //{
        //    Debug.LogError("JSON file not found: " + jsonFilePath);
        //}

        print(storyFilePath);
        TextAsset jsonFile = Resources.Load<TextAsset>(storyFilePath);
        print(jsonFile);
        
        wrapper = JsonUtility.FromJson<StoryDataWrapper>(jsonFile.text);
        LoadStory(wrapper.storyArray[currentStoryIndex]);

    }


    void OnClickButton()
    {
        // Set the active state of the game object to false
        DisplayNextSentence();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Called when the screen is tapped
        DisplayNextSentence();
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.GetComponent<Text>().text = "";
        isTyping = true;

        foreach (char character in sentence)
        {
            dialogueText.GetComponent<Text>().text += character;
            yield return new WaitForSeconds(0.03f); // Adjust the delay between characters if needed
        }

        isTyping = false;
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            // Finish typing the current sentence immediately
            StopAllCoroutines();
            dialogueText.GetComponent<Text>().text = currentSentence;
            isTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            // End of dialogue
            dialoguePanel.SetActive(false);

            // Move to the next story
            currentStoryIndex++;
            if (currentStoryIndex < wrapper.storyArray.Count)
            {
                LoadStory(wrapper.storyArray[currentStoryIndex]);
            }
            else
            {
                canvasObject.SetActive(false);
                // End of all stories
                // Perform any desired actions here
            }

            return;
        }

        currentSentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(currentSentence));
    }

    private void LoadStory(StoryData story)
    {
        Vector2 rightPosition = new Vector2(screenSize.x / 2f, -screenSize.y / 2f);
        Sprite rightSprite = Resources.Load<Sprite>("Sprites/Characters/" + story.imagePath);
        print("Sprites/Characters/" + story.imagePath);
        rightImage = Utils.CreateImage(rightSprite, rightPosition, canvasObject.transform);
        Utils.MoveImage(rightImage, -0.5f, 0.5f);
        rightImage.GetComponent<Image>().sprite = rightSprite;

        Vector2 leftPosition = new Vector2(-screenSize.x / 2f, -screenSize.y / 2f);
        Sprite leftSprite = Resources.Load<Sprite>("Sprites/Characters/" + story.imagePath);
        print("Sprites/Characters/" + story.imagePath);
        leftImage = Utils.CreateImage(leftSprite, leftPosition, canvasObject.transform);
        Utils.MoveImage(leftImage, 0.5f, 0.5f);
        leftImage.GetComponent<Image>().sprite = leftSprite;

        if (bgmAudioSource.clip != null)
        {
            bgmAudioSource.Stop();
        }
        if (!string.IsNullOrEmpty(story.bgmPath))
        {
            AudioClip bgmClip = Resources.Load<AudioClip>("BGM/" + story.bgmPath);
            bgmAudioSource.clip = bgmClip;
            bgmAudioSource.Play();
        }

        sentences.Clear();
        foreach (string sentence in story.dialogue)
        {
            sentences.Enqueue(sentence);
        }

        // Start displaying the first sentence
        DisplayNextSentence();
    }
}
