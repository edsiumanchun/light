using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class HomeController : MonoBehaviour
{

    public int numPlates = 5;
    public float plateSpacing = 2.0f;

    public AudioClip introStoryClip;
    public AudioClip endingStoryClip;
    public GameObject dialogBoxPrefab;
    public float dialogDelay = 2.0f;
    public float storyDelay = 5.0f;

    private AudioSource audioSource;
    private StoryController storyController;


    byte[] fileData;


    void Start()
    {

        MainController.SetupScene();
        this.gameObject.AddComponent<GameUIController>();
        
        print(DataHolder.loadedAssets);
        int i = 0;
         foreach (AssetData asset in DataHolder.loadedAssets)
        {
             print(asset.tempAssets.Count);
            asset.CreateObjects(-1 * i);
            i++;
        }

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