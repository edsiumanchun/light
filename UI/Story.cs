
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class StoryController : MonoBehaviour
{
    public Image backgroundImage;
    public Image storyImage;
    public Image character2Image;
    public Text storyName;
    public Text character2NameText;
    public Text dialogueText;
    public GameObject dialoguePanel;
    public AudioSource bgmAudioSource;

    private Queue<string> sentences = new Queue<string>();
    private bool isTyping = false;

    void Start()
    {
        //LoadDialogue("Story1");
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    DisplayNextSentence();
        //}
    }

    public void LoadDialogue(string jsonFile)
    {
        string storyFilePath = "Assets/Json/" + jsonFile + ".json";
        string storyJson = File.ReadAllText(storyFilePath);
        print(storyJson);
        StoryData storyData = JsonUtility.FromJson<StoryData>(storyJson);
        storyImage.sprite = Resources.Load<Sprite>("Characters/" + storyData.imagePath);
        storyName.text = storyData.name;


        backgroundImage.sprite = Resources.Load<Sprite>("Backgrounds/" + storyData.backgroundPath);

        if (bgmAudioSource.clip != null)
        {
            bgmAudioSource.Stop();
        }
        if (storyData.bgmPath != "")
        {
            AudioClip bgmClip = Resources.Load<AudioClip>("BGM/" + storyData.bgmPath);
            bgmAudioSource.clip = bgmClip;
            bgmAudioSource.Play();
        }

        sentences.Clear();
        foreach (string sentence in storyData.dialogue)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            // Finish typing the current sentence immediately
            StopAllCoroutines();
            dialogueText.text = sentences.Peek();
            isTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            // End of dialogue
            dialoguePanel.SetActive(false);
            return;
        }

        string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
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

    public IEnumerator PlayStory(IStoryEvent storyEvent, GameObject dialogBoxPrefab, AudioSource audioSource, float dialogDelay, float storyDelay)
    {
        // Create the dialog box
        DialogBox dialogBox = Instantiate(dialogBoxPrefab, transform).GetComponent<DialogBox>();

        // Play the story event
        yield return storyEvent.Play(dialogBox);

        // Wait for the story to finish before playing the next one
        yield return new WaitForSeconds(storyDelay);

        // Hide the dialog box
        dialogBox.Hide();

        // Wait for a delay before playing the next story
        yield return new WaitForSeconds(dialogDelay);

        // Play the next story event if there is one
        if (audioSource !=null)
        {
            audioSource.Stop();
        }
    }
}
