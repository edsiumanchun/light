using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// VictoryStoryEvent.cs
public interface IStoryEvent
{
    IEnumerator Play(DialogBox dialogBox);
}

public class WelcomeStoryEvent : IStoryEvent
{
    public IEnumerator Play(DialogBox dialogBox)
    {
        dialogBox.Show("Character 1: Welcome to the world of floating plates!");
        yield return new WaitForSeconds(2.0f);
        dialogBox.Show("Character 2: It's so beautiful and peaceful here.");
        yield return new WaitForSeconds(5.0f);
        dialogBox.Hide();
    }
}

public class VictoryStoryEvent : IStoryEvent
{
    public IEnumerator Play(DialogBox dialogBox)
    {
        dialogBox.Show("Character 1: We did it! We defeated the boss!");
        yield return new WaitForSeconds(2.0f);
        dialogBox.Show("Character 2: Yes, and now we can enjoy the peace once again.");
        yield return new WaitForSeconds(5.0f);
        dialogBox.Hide();
    }
}

public class DialogBox : MonoBehaviour
{
    public Text messageText;
    public GameObject dialogBox;

    public void Show(string message)
    {
        messageText.text = message;
        dialogBox.SetActive(true);
    }

    public void Hide()
    {
        dialogBox.SetActive(false);
    }
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

public class SceneGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject platePrefab;
    public GameObject playerPrefab;
    public GameObject bossPrefab;
    public int numPlates = 5;
    public float plateSpacing = 2.0f;

    public AudioClip introStoryClip;
    public AudioClip endingStoryClip;
    public GameObject dialogBoxPrefab;
    public float dialogDelay = 2.0f;
    public float storyDelay = 5.0f;

    private AudioSource audioSource;
    private StoryController storyController;

    void Start()
    {
        // Create the wall and floor
        Instantiate(wallPrefab, new Vector3(0, 0, 10), Quaternion.identity);
        Instantiate(floorPrefab, new Vector3(0, -5, 0), Quaternion.identity);

        // Create the floating plates
        for (int i = 0; i < numPlates; i++)
        {
            float x = Random.Range(-5.0f, 5.0f);
            float y = Random.Range(0.0f, 5.0f);
            float z = Random.Range(-5.0f, 5.0f);
            Vector3 position = new Vector3(x, y, z) * plateSpacing;

            Instantiate(platePrefab, position, Quaternion.identity);
        }

        // Create the player
        Vector3 playerPosition = new Vector3(0, -4, -5);
        Instantiate(playerPrefab, playerPosition, Quaternion.identity);

        // Create the boss
        Vector3 bossPosition = new Vector3(0, 4, 5);
        GameObject boss = Instantiate(bossPrefab, bossPosition, Quaternion.identity);

        // Set up boss death event
        // int currentHealth = boss.GetComponent<BossController>().currentHealth;
        // if (currentHealth != null)
        // {
        //     currentHealth.OnBossDeath += PlayEndingStory;
        // }

        // Create the story controller
        GameObject storyControllerObject = new GameObject("StoryController");
        storyController = storyControllerObject.AddComponent<StoryController>();

        // Play the introduction story
        if (introStoryClip != null)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = introStoryClip;

            // Load the WelcomeStoryEvent data from JSON
            string welcomeStoryJson = Resources.Load<TextAsset>("WelcomeStoryEvent").text;
            WelcomeStoryEvent welcomeStoryEvent = JsonUtility.FromJson<WelcomeStoryEvent>(welcomeStoryJson);

            // Play the WelcomeStoryEvent using the StoryController
            audioSource.Play();
            storyController.StartCoroutine(storyController.PlayStory(welcomeStoryEvent, dialogBoxPrefab, audioSource, dialogDelay, storyDelay));
        }
    }

    void PlayEndingStory()
    {
        // Stop any playing audio
        audioSource.Stop();

        // Play the ending story
        if (endingStoryClip != null)
        {
            audioSource.clip = endingStoryClip;

            // Load the VictoryStoryEvent data from JSON
            string victoryStoryJson = Resources.Load<TextAsset>("VictoryStoryEvent").text;
            VictoryStoryEvent victoryStoryEvent = JsonUtility.FromJson<VictoryStoryEvent>(victoryStoryJson);

            // Play the VictoryStoryEvent using the StoryController
            audioSource.Play();
            storyController.StartCoroutine(storyController.PlayStory(victoryStoryEvent, dialogBoxPrefab, audioSource, dialogDelay, storyDelay));
        }

        // Load next scene after ending story finishes
        Invoke("LoadNextScene", endingStoryClip.length);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("NextSceneName");
    }
}