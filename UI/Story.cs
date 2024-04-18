
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class StoryController : MonoBehaviour
{
    public GameObject storyGameObject;
    public Texture2D leftGifTexture;
    public Texture2D rightGifTexture;
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject centerPrefab;

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
        storyGameObject = new GameObject("Story");
        // Create Canvas object
        canvasObject = Utils.CreateCanvas("StoryCanvas", true, 0f);
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
        Utils.MovePrefab(backgroundImage, 0, 0.5f);
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

    public void LoadStoryDataFromJSON()
    {
        SetupFadeObject(false);
        string storyFilePath = "Json/Story/" + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + CacheManager.GetCache(Cache.CurrentFloor.ToString());

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
    public void Fade(GameObject targetObject, bool fadeIn)
    {
        float fadeDuration = 1f;
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;
            Color startColor = material.color;
            Color endColor;

            if (fadeIn)
            {
                endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
                material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
            }
            else
            {
                endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            }

            material.DOFade(endColor.a, fadeDuration)
                .OnComplete(() =>
                {
                    if (!fadeIn)
                    {
                        targetObject.SetActive(false);
                    }
                });
        }

        if (fadeIn)
        {
            targetObject.SetActive(true);
        }
    }

    void SetupFadeObject(bool isStart){
        GameObject container = GameObject.Find("Container");
        GameObject gameUI = GameObject.Find("GameCanvas");
        Fade(container, isStart);
        Fade(storyGameObject, !isStart);
        canvasObject.SetActive(!isStart);
        gameUI.SetActive(isStart);
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
            currentStoryIndex++;
            if (currentStoryIndex < wrapper.storyArray.Count)
            {
                LoadStory(wrapper.storyArray[currentStoryIndex]);
            }
            else
            {
                SetupFadeObject(true);
            }

            return;
        }

        currentSentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(currentSentence));
    }

    void PositionPrefabAtCorner(GameObject obj, float position)
    {
        Vector2 offset = position == 0 ? new Vector2(0.5f, 0.5f) : new Vector2(-0.5f, 0.5f) ;
        // Get the screen dimensions
        float screenRatio = (float)Screen.width / Screen.height;
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * screenRatio;

        // Calculate the position of the corner
        Vector2 cornerPosition = Vector2.zero;

        if (position == 0) // Bottom left
        {
            cornerPosition.x = screenWidth * 0.5f;
        }
        else if (position == 1) // Center
        {
            cornerPosition.x = Screen.width * 0.5f;
        }
        else if (position == 2) // Bottom right
        {
            cornerPosition.x = Screen.width - (screenWidth * 0.5f);
        }

        cornerPosition.y = screenHeight * 0.5f;

        // Apply the offset
        cornerPosition += new Vector2(screenWidth * offset.x, screenHeight * offset.y);

        // Convert screen coordinates to world coordinates
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(cornerPosition.x, cornerPosition.y, 0f));
        worldPosition.z = obj.transform.position.z; // Preserve the original object's Z position

        // Move the original object to the calculated position
        obj.transform.position = worldPosition;
    }
    public void LoadStory(StoryData story)
    {
        if(story.leftPrefab != null && story.leftPrefab != "") {
            Vector2 leftPosition = new Vector2(-screenSize.x / 2f, -screenSize.y / 2f);
            // leftPrefab = Utils.CreateImage(leftSprite, leftPosition, canvasObject.transform);
            GameObject prefab = (GameObject)DataHolder.characters[0];
            leftPrefab = Utils.InstantiateObject(prefab);
            leftPrefab.transform.SetParent(storyGameObject.transform, true);
            leftPrefab.name = "leftPrefab";
            PositionPrefabAtCorner(leftPrefab,0);
        } else {
            if(leftPrefab != null){
                Destroy(leftPrefab);
            }
        }
        if(story.rightPrefab != null && story.rightPrefab != "") {
            Vector2 rightPosition = new Vector2(-screenSize.x / 2f, -screenSize.y / 2f);
            // rightPrefab = Utils.CreateImage(rightSprite, rightPosition, canvasObject.transform);
            GameObject prefab = (GameObject)DataHolder.characters[0];
            rightPrefab = Utils.InstantiateObject(prefab);
            rightPrefab.transform.SetParent(storyGameObject.transform, true);
            rightPrefab.name = "rightPrefab";
            PositionPrefabAtCorner(rightPrefab,2);
        } else {
            if(rightPrefab != null){
                Destroy(rightPrefab);
            }
        }
        if(story.centerPrefab != null && story.centerPrefab != "") {
            Vector2 centerPosition = new Vector2(-screenSize.x / 2f, -screenSize.y / 2f);
            // centerPrefab = Utils.CreateImage(centerSprite, centerPosition, canvasObject.transform);
            GameObject prefab = (GameObject)DataHolder.characters[0];
            centerPrefab = Utils.InstantiateObject(prefab);
            centerPrefab.transform.SetParent(storyGameObject.transform, true);
            centerPrefab.name = "centerPrefab";
            PositionPrefabAtCorner(centerPrefab,1);
        } else {
            if(centerPrefab != null){
                Destroy(centerPrefab);
            }
        }
        
        // Utils.MovePrefab(leftPrefab, 0.5f, 0.5f);
        // leftPrefab.GetComponent<Image>().sprite = leftSprite;

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
