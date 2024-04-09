using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class HomeController : MonoBehaviour
{

    public GameObject storyCanvasPrefab;
    public int numPlates = 5;
    public float plateSpacing = 2.0f;


    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    void Start()
    {

        // UI
        this.gameObject.AddComponent<CameraViewController>();
        this.gameObject.AddComponent<GameUIController>();
        this.gameObject.AddComponent<StoryController>();
        this.gameObject.AddComponent<SettingsController>();
        this.gameObject.AddComponent<ConfirmController>();
        this.gameObject.AddComponent<DashboardController>();
        this.gameObject.AddComponent<StageController>();
        this.gameObject.AddComponent<InfoController>();
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