using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class StoryController : MonoBehaviour
{
    public Image backgroundImage;
    public Image character1Image;
    public Image character2Image;
    public Text character1NameText;
    public Text character2NameText;
    public Text dialogueText;
    public GameObject dialoguePanel;
    public AudioSource bgmAudioSource;

    private Queue<string> sentences = new Queue<string>();
    private bool isTyping = false;

    void Start()
    {
        LoadDialogue("character1", "character2");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    public void LoadDialogue(string character1Name, string character2Name)
    {
        string character1FilePath = "Assets/Resources/" + character1Name + ".json";
        string character1JsonString = File.ReadAllText(character1FilePath);
        CharacterData character1Data = JsonUtility.FromJson<CharacterData>(character1JsonString);
        character1Image.sprite = Resources.Load<Sprite>("Characters/" + character1Data.imagePath);
        character1NameText.text = character1Data.name;

        string character2FilePath = "Assets/Resources/" + character2Name + ".json";
        string character2JsonString = File.ReadAllText(character2FilePath);
        CharacterData character2Data = JsonUtility.FromJson<CharacterData>(character2JsonString);
        character2Image.sprite = Resources.Load<Sprite>("Characters/" + character2Data.imagePath);
        character2NameText.text = character2Data.name;

        backgroundImage.sprite = Resources.Load<Sprite>("Backgrounds/" + character1Data.backgroundPath);

        if (bgmAudioSource.clip != null)
        {
            bgmAudioSource.Stop();
        }
        if (character1Data.bgmPath != "")
        {
            AudioClip bgmClip = Resources.Load<AudioClip>("BGM/" + character1Data.bgmPath);
            bgmAudioSource.clip = bgmClip;
            bgmAudioSource.Play();
        }

        sentences.Clear();
        foreach (string sentence in character1Data.dialogue)
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
    public class CharacterData
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
