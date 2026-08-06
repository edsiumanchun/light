using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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
        System.Type[] controllers = {
            typeof(CameraViewController), typeof(GameUIController), typeof(StoryController),
            typeof(SettingsController), typeof(ConfirmController), typeof(StageController),
            typeof(InfoController), typeof(ItemController), typeof(ShopController),
            typeof(VideoPlayerController)
        };
        foreach (System.Type controller in controllers)
            if (GetComponent(controller) == null) gameObject.AddComponent(controller);
        loaded = true;
        LoadGame();
    }

    public static void LoadGame(){
        if (Camera.main == null || DataHolder.loadedAssets == null)
        {
            Debug.LogError("Cannot load game: camera or floor data is missing.");
            return;
        }
        // Camera.main.gameObject.GetComponent<InfoController>().DisplayPage();
        StageController stageController = Camera.main.gameObject.GetComponent<StageController>();
        if (stageController != null) stageController.updateButtonImages();
        if(AssetData.gameObject != null) Destroy(AssetData.gameObject);
        DataHolder.player = null;
        AssetData.gameObject = new GameObject("MainGameObject");
        AssetData.goUp = false;
        AssetData.showStory = false;
        AssetData.wallSkip = null;
        AssetData.teleportExit = 0;
        AssetData.teleport = 0;
        AssetData.exit = 0;
        AssetData.nameList = new HashSet<string>();
        for (int groupIndex = 0; groupIndex < DataHolder.loadedAssets.Count; groupIndex++)
        {
            List<AssetData> assetDataList = DataHolder.loadedAssets[groupIndex];
            for (int assetIndex = 0; assetIndex < assetDataList.Count; assetIndex++)
            {
                AssetData asset = assetDataList[assetIndex];
                if (asset == null) continue;
                if (assetIndex == 0 && asset.name != "Audio")
                    new AssetData { name = "Audio", gridArray0 = new List<int> { 0 } }
                        .CreateObjects(0, groupIndex);
                asset.CreateObjects(-assetIndex, groupIndex);
            }
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
            SpawnFireworkNow(gameObject, isLow);
            yield return new WaitForSeconds(Random.Range(6.6f, 8.8f));
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
        if (mainGameObject == null)
        {
            Destroy(gameObject);
            return;
        }
        gameObject.transform.SetParent(mainGameObject.transform);
        gameObject.transform.localPosition = spawnPosition;
    }
}
