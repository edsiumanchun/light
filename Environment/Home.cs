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
        Camera.main.gameObject.GetComponent<StageController>().updateButtonImages();
        if(AssetData.gameObject != null) Destroy(AssetData.gameObject);
        AssetData.gameObject = new GameObject();
        AssetData.gameObject.name = "MainGameObject";
        AssetData.goUp = false;
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

    public void SpawnFirework(GameObject firework, bool isLow)
    {
        StartCoroutine(SpawnFireworkCoroutine(firework, isLow));
    }
    private IEnumerator SpawnFireworkCoroutine(GameObject fireworkPrefab, bool isLow)
    {
        while (true)
        {
            GameObject gameObject = Instantiate(fireworkPrefab);
            // ParticleSystem particleEffect = gameObject.GetComponent<ParticleSystem>();
            // particleEffect.Stop();

            // particleEffect.Clear();
            // particleEffect.time = 0f;
            // particleEffect.Play();
            SpawnFireworkNow(gameObject, isLow);
            float delay = Random.Range(6.6f, 8.8f);
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnFireworkNow(GameObject gameObject, bool isLow)
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(-4, 4),
            isLow ? Random.Range(-6, -4) : Random.Range(0, 4),
            -3
        );
        // Vector2 worldVector = Utils.ScaleToWorld(spawnPosition);
        GameObject mainGameObject = GameObject.Find("MainGameObject");
        gameObject.transform.SetParent(mainGameObject.transform);
        gameObject.transform.localPosition = spawnPosition;
    }
}