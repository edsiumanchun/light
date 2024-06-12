
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class StoryController : MonoBehaviour
{
    bool isWaitingEvent = false;
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
    private bool isStory = false;
    private int currentStoryIndex;
    public StoryDataWrapper wrapper;
    public string currentSentence = "";

    public AudioSource bgmAudioSource;
    public GameObject canvasObject;
    public Vector2 screenSize;
    GameObject container;
    GameObject gameUI;

    private void Start()
    {
        container = GameObject.Find("Container");
        gameUI = GameObject.Find("GameCanvas");
        // Create Canvas object
        canvasObject = Utils.CreateCanvas("StoryCanvas", true, 0f);
        Button button = canvasObject.AddComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        // setup
        sentences = new Queue<string>();
        //
        if(backgroundImage != null) {
            return;
        };
        screenSize = Utils.GetCanvasSize(canvasObject);
        Vector2 backgroundPosition = new Vector2(0, -screenSize.y / 2);
        Vector2 dialoguePanelPosition = new Vector2(0f, -Screen.height / 2);

        backgroundImage = Utils.CreateImage(DataHolder.storyImages[0], backgroundPosition, canvasObject.transform);
        Utils.MovePrefab(backgroundImage, 0, 0.5f);
        RectTransform imageRectTransform = backgroundImage.GetComponent<RectTransform>();
        dialogueText = Utils.CreateText(
            new Vector2(imageRectTransform.sizeDelta.x * 7.0f / 8.8f, imageRectTransform.sizeDelta.y * 6.6f / 8.8f),
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

    public void LoadStoryDataFromJSON(){
        LoadStoryDataFromJSONWithPath(CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + CacheManager.GetCache(Cache.CurrentFloor.ToString()));
    }
    public void LoadStoryDataFromJSONWithPath(string path)
    {
        SetupFadeObject(false);

        isTyping = false;
        currentStoryIndex = 0;
        string storyFilePath = "Json/Story/" + path;
        print(storyFilePath);
        TextAsset jsonFile = Resources.Load<TextAsset>(storyFilePath);
        print(jsonFile);
        wrapper = JsonUtility.FromJson<StoryDataWrapper>(jsonFile.text);
        StoryData story = wrapper.storyArray[currentStoryIndex];
        isStory = story.name != null;
        dialogueText.GetComponent<Text>().alignment = isStory ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;

        if(isStory) {
            backgroundImage.transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            backgroundImage.transform.localScale = new Vector3(0.68f, 0.5f, 1f);
        }
        LoadStory(story);

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
        Fade(container, isStart);
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
        if(isWaitingEvent) return;
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
                CacheManager.SetClearFloor();
            }

            return;
        }

        currentSentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(currentSentence));
    }

    void PositionPrefabAtCorner(GameObject obj, float position)
    {
        Vector2 offset = position == 0 ? new Vector2(3f, 3f) : new Vector2(-3f, 3f) ;
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
        cornerPosition += new Vector2(screenWidth * offset.x, screenHeight + 200);

        // Convert screen coordinates to world coordinates
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(cornerPosition.x, cornerPosition.y, 0f));
        worldPosition.z = obj.transform.position.z; // Preserve the original object's Z position

        // Move the original object to the calculated position
        obj.transform.position = worldPosition;
    }
    public void LoadStory(StoryData story)
    {
        RectTransform imageRectTransform = backgroundImage.GetComponent<RectTransform>();
        Debug.Log("story.leftPrefab" + story.leftPrefab);
        if(story.leftPrefab != null && story.leftPrefab != "") {
            if(leftPrefab != null){
                Destroy(leftPrefab);
            }
            Vector2 leftPosition = new Vector2(- imageRectTransform.sizeDelta.x / 2 ,  - screenSize.y / 2f );
            List<string> paths = new List<string>((new[] { story.leftPrefab}));
            Sprite leftSprite = Utils.LoadSprites("Story", paths)[0];
            leftPrefab = Utils.CreateImage(leftSprite, leftPosition, canvasObject.transform);

            RectTransform rectTransform = leftPrefab.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 380);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380);
            // leftPrefab.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Utils.MovePrefab(leftPrefab, -0.33f, 0.5f);
            leftPrefab.name = story.leftPrefab;
        } 
        if(story.rightPrefab != null) {
            if(rightPrefab != null){
                Destroy(rightPrefab);
            }
            if(story.rightPrefab != ""){
                Vector2 rightPosition = new Vector2(imageRectTransform.sizeDelta.x / 2 ,  - screenSize.y / 2f );
                List<string> paths = new List<string>((new[] { story.rightPrefab}));
                Sprite rightSprite = Utils.LoadSprites("Story", paths)[0];
                rightPrefab = Utils.CreateImage(rightSprite, rightPosition, canvasObject.transform);
                rightPrefab.transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
                Utils.MovePrefab(rightPrefab, -0.33f, 0.5f);
                rightPrefab.name = story.rightPrefab;
            }
        }
        if(isStory) {
            if(leftPrefab.name.Contains(story.name)) {
                Utils.changeAlpha(leftPrefab, 1f);
                Utils.changeAlpha(rightPrefab, 0.1f);
            } else if(rightPrefab.name.Contains(story.name)) {
                Utils.changeAlpha(leftPrefab, 0.1f);
                Utils.changeAlpha(rightPrefab, 1f);
            } else {
                Utils.changeAlpha(leftPrefab, 0.1f);
                Utils.changeAlpha(rightPrefab, 0.1f);
            }
        }
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
        int i = 0;
        if(story.name == "event") {
            StartCoroutine(ShowEvent(story));
        } else {
            foreach (string sentence in story.dialogue)
            {
                sentences.Enqueue(story.name + "\n" + sentence);
                i++;
            }
            DisplayNextSentence();
        }
    }
    IEnumerator ShowEvent(StoryData story)
    {
        isWaitingEvent = true;
        foreach (string order in story.dialogue) {
            float time = EventController.ShowEvent(order);
            yield return new WaitForSeconds(time);
        }
        isWaitingEvent = false;
        DisplayNextSentence();
    }
}
