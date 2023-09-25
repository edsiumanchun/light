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

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        CameraViewController cameraViewController = Camera.main.gameObject.AddComponent<CameraViewController>();
        cameraViewController.SetBackgroundWidth();

    }

    void Start()
    {

        // UI
        this.gameObject.AddComponent<GameUIController>();
        this.gameObject.AddComponent<StoryController>();
        // UI

        int j = 0;
        foreach (List<AssetData> assetDataList in DataHolder.loadedAssets)
        {
            int i = 0;
            foreach (AssetData asset in assetDataList)
            {
                asset.CreateObjects(-1 * i, j);
                i++;
            }
            j++;
        }

        // Set up boss death event
        // int currentHealth = boss.GetComponent<BossController>().currentHealth;
        // if (currentHealth != null)
        // {
        //     currentHealth.OnBossDeath += PlayEndingStory;
        // }
    }
}