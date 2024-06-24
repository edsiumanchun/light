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

    public static bool loaded = false;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        this.gameObject.AddComponent<CameraViewController>();
        this.gameObject.AddComponent<GameUIController>();
        this.gameObject.AddComponent<StoryController>();
        this.gameObject.AddComponent<SettingsController>();
        this.gameObject.AddComponent<ConfirmController>();
        // this.gameObject.AddComponent<DashboardController>();
        this.gameObject.AddComponent<StageController>();
        this.gameObject.AddComponent<InfoController>();
        this.gameObject.AddComponent<ItemController>();
        this.gameObject.AddComponent<ShopController>();
        this.gameObject.AddComponent<VideoPlayerController>();
        loaded = true;
        LoadGame();
        // Set up boss death event
        // int currentHealth = boss.GetComponent<BossController>().currentHealth;
        // if (currentHealth != null)
        // {
        //     currentHealth.OnBossDeath += PlayEndingStory;
        // }
    }

    public static void LoadGame(){
        // Camera.main.gameObject.GetComponent<InfoController>().DisplayPage();
        if(AssetData.gameObject != null) Destroy(AssetData.gameObject);
        AssetData.gameObject = new GameObject();
        AssetData.gameObject.name = "GameObject";
        int j = 0;
        AssetData.nameList = new HashSet<string>();
        foreach (List<AssetData> assetDataList in DataHolder.loadedAssets)
        {
            int i = 0;
            foreach (AssetData asset in assetDataList)
            {
                if(i == 0 && asset.name != "Audio") {
                    AssetData audioAsset = new AssetData
                    {
                        name = "Audio",
                        gridArray0 = new List<int> { 0 }
                    };
                    audioAsset.CreateObjects(-1 * i, j);
                }  
                asset.CreateObjects(-1 * i, j);
                i++;
            }
            j++;
        }
    }
}