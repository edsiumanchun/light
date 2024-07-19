
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
    public GameObject titleText, dialogueText;
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
    GameObject gameUI;
    public bool canPlayerMove = false;
    public bool onlyOne = false;
    public Button button;
    public void ShowTutor(){
        int showTutor = CacheManager.GetCache(Cache.ShowTutor.ToString());
        if(showTutor == 0) {
            SetIntervative(false);
            LoadStoryDataFromText("【移動】左右滑動屏幕","【跳躍】點擊屏幕\n【特殊】貼近人物前方點擊右下角按鈕" );
            
        }
    }

    public void SetIntervative(bool isActive){
        GameObject backgroundImageObject = canvasObject.transform.Find("BackgroundImageObject").gameObject;
        backgroundImageObject.SetActive(isActive);
        canPlayerMove = !isActive;
    }
    private void Start()
    {
        gameUI = GameObject.Find("GameCanvas");
        // Create Canvas object
        canvasObject = Utils.CreateCanvas("StoryCanvas", true, 0f);
        button = canvasObject.AddComponent<Button>();
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
            new Vector2(imageRectTransform.localPosition.x, imageRectTransform.localPosition.y - 33f), canvasObject.transform, TextAnchor.UpperLeft);
        titleText = Utils.CreateText(
            new Vector2(imageRectTransform.sizeDelta.x * 7.0f / 8.8f, 30),
            new Vector2(0,-368), canvasObject.transform, TextAnchor.UpperLeft);
        

        dialoguePanel = new GameObject("DialoguePanel");
        // parent
        RectTransform dialoguePanelTransform = dialoguePanel.AddComponent<RectTransform>();
        dialoguePanelTransform.SetParent(canvasObject.transform); // Set the parent transform
        dialoguePanelTransform.localPosition = dialoguePanelPosition;

        // Create the audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        // Other initialization code...
        canvasObject.SetActive(false);
        ShowTutor();
    }

    public void LoadStoryDataFromJSON(string suffix, int index){
        string storyFilePath = "Json/Story/" + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + CacheManager.GetCache(Cache.CurrentFloor.ToString() ) + suffix;

        print(storyFilePath+ index.ToString());
        TextAsset jsonFile = Resources.Load<TextAsset>(storyFilePath);
        print(jsonFile );
        wrapper = JsonUtility.FromJson<StoryDataWrapper>(jsonFile.text);
        if(suffix == ""){
            currentStoryIndex = 0;
            onlyOne = false;
        } else {
            currentStoryIndex = index;
            onlyOne = true;
        }
        LoadStoryDataFromJSONWithPath(currentStoryIndex);   
    }

    public void LoadStoryDataFromTextList(string title, string[] results){
        onlyOne = false;
        StoryData storyData = new StoryData
        {
            dialogue = results,
            name = title
        };
        // Create a new StoryDataWrapper and add the StoryData object
        StoryDataWrapper storyDataWrapper = new StoryDataWrapper
        {
            storyArray = new System.Collections.Generic.List<StoryData>
            {
                storyData
            }
        }; 
        wrapper = storyDataWrapper;
        LoadStoryDataFromJSONWithPath(0);
    }
    public void LoadStoryDataFromText(string title, string result){
        LoadStoryDataFromTextList(title, new string[] { result });
    }
    public void LoadStoryDataFromJSONWithPath(int index)
    {
        SetupFadeObject(false);
        if(canPlayerMove == false) {
            GameObject player = GameObject.Find("Player");
            player.GetComponent<JoystickController>().isFighting = true;
        }
        if(leftPrefab != null) Destroy(leftPrefab);
        if(rightPrefab != null) Destroy(rightPrefab);
        isTyping = false;
        currentStoryIndex = index;
        StoryData story = wrapper.storyArray[currentStoryIndex];
        isStory = story.name != null && story.name != "";
        dialogueText.GetComponent<Text>().alignment = isStory ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;
        titleText.GetComponent<Text>().alignment = isStory ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;

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
        canvasObject.SetActive(!isStart);
        if(canPlayerMove) { // for real and fake stone
            gameUI.SetActive(true);
        } else {
            if(titleText.GetComponent<Text>().text == "") { // reminder
                Camera.main.GetComponent<GameUIController>().HideBattle(isStart);
            } else {
                gameUI.SetActive(isStart);
            }
        }
    }

    void OnClickButton()
    {
        if(canPlayerMove) return;
        if(isWaitingEvent) return;
        DisplayNextSentence();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(canPlayerMove) return;
        if(isWaitingEvent) return;
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

    public void EndStory(){

        SetupFadeObject(true);
        CacheManager.SetClearFloor();
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().isFighting = false;
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
            var count = onlyOne ? 1 : wrapper.storyArray.Count;
            if (currentStoryIndex < count)
            {
                LoadStory(wrapper.storyArray[currentStoryIndex]);
            }
            else
            {
                EndStory();
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
            leftPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
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
                rightPrefab.transform.localScale = new Vector3(-1f, 1f, 1f);
                Utils.MovePrefab(rightPrefab, -0.33f, 0.5f);
                rightPrefab.name = story.rightPrefab;
            }
        }
        if(isStory && leftPrefab != null) {
            if(leftPrefab.name.Contains(story.name)) {
                Utils.changeAlpha(leftPrefab, 1f);
                Utils.changeAlpha(rightPrefab, 0.1f);
            }
        } else {
            Utils.changeAlpha(leftPrefab, 0.1f);
        }
        if(isStory && rightPrefab != null) {
            if(rightPrefab.name.Contains(story.name)) {
                Utils.changeAlpha(leftPrefab, 0.1f);
                Utils.changeAlpha(rightPrefab, 1f);
            } 
        } else {
            Utils.changeAlpha(rightPrefab, 0.1f);
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
            if(leftPrefab != null) leftPrefab.SetActive(false);
            if(rightPrefab != null) rightPrefab.SetActive(false);
            if(dialogueText != null) dialogueText.transform.parent.gameObject.SetActive(false);
            StartCoroutine(ShowEvent(story));
        } else {
            if(leftPrefab != null) leftPrefab.SetActive(true);
            if(rightPrefab != null) rightPrefab.SetActive(true);
            if(dialogueText != null) dialogueText.transform.parent.gameObject.SetActive(true);

            titleText.GetComponent<Text>().text = story.name;
            foreach (string sentence in story.dialogue)
            {
                sentences.Enqueue(sentence);
                // sentences.Enqueue(story.name + "\n" + sentence);
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
 