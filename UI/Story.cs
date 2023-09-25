
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
    public AudioSource bgmAudioSource;
    private Queue<string> sentences = new Queue<string>();
    private bool isTyping = false;
    private int currentStoryIndex;
    public StoryDataWrapper wrapper;

    public GameObject canvasObject;

    private void Start()
    {
        // Create Canvas object
        canvasObject = new GameObject("StoryCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        // setup
        sentences = new Queue<string>();
        isTyping = false;
        currentStoryIndex = 0;
        //
        if(backgroundImage != null) {
            return;
        };
        Vector3 backgroundPosition = new Vector3(0f, 5f, 0f);
        Vector3 leftPosition = new Vector3(-10f, 0f, 0f);
        Vector3 rightPosition = new Vector3(10f, 0f, 0f);
        Vector3 dialoguePosition = new Vector3(0f, 0f, 0f);
        Vector3 dialoguePanelPosition = new Vector3(0f, -5f, 0f);

        backgroundImage = Utils.CreateImage(canvasObject.transform, backgroundPosition);
        leftImage = Utils.CreateImage(canvasObject.transform, leftPosition);
        rightImage = Utils.CreateImage(canvasObject.transform, rightPosition);
        print(rightImage);
        dialogueText = Utils.CreateText(canvasObject.transform, dialoguePosition);

        dialoguePanel = new GameObject("DialoguePanel");
        // parent
        dialoguePanel.transform.SetParent(canvasObject.transform); // Set the parent transform
        dialoguePanel.transform.position = dialoguePanelPosition;
        dialoguePanel.AddComponent<RectTransform>(); // Add a RectTransform component

        // 
        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
    }

    public void LoadStoryDataFromJSON(string jsonFilePath)
    {
        string storyFilePath = "Json/Story/" + DataHolder.currentStage + "/" + jsonFilePath;

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
            yield return new WaitForSeconds(0.1f); // Adjust the delay between characters if needed
        }

        isTyping = false;
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            // Finish typing the current sentence immediately
            StopAllCoroutines();
            dialogueText.GetComponent<Text>().text = sentences.Peek();
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
                // End of all stories
                // Perform any desired actions here
            }

            return;
        }

        string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    private void LoadStory(StoryData story)
    {
        // Load and display the story data

        //Sprite leftSprite = Sprite.Create(leftGifTexture, new Rect(0, 0, leftGifTexture.width, leftGifTexture.height), Vector2.one * 0.5f);
        //leftImage.sprite = leftSprite;

        Sprite rightSprite = Resources.Load<Sprite>("Sprites/Characters/" + story.imagePath);
        print("Sprites/Characters/" + story.imagePath);
        print(rightImage);
        print(rightSprite);
        rightImage.GetComponent<Image>().sprite = rightSprite;

        backgroundImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Backgrounds/" + story.backgroundPath);

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
