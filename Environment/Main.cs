using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    private void Start()
    {
        if(Utils.isUAT) {
            PlayerPrefs.DeleteAll();
            CacheManager.SetCache(Cache.CurrentStage.ToString(), 1);
            CacheManager.SetCache(Cache.CurrentFloor.ToString(), 8);
            CacheManager.SetCache(Cache.ExitH.ToString(), 4); 
            CacheManager.SetCache(Cache.ExitV.ToString(), -1); 
            CacheManager.SetCache(Cache.ShowTutor.ToString(), 1);
            CacheManager.SetCache(Cache.HPMax.ToString(), 9999);
            CacheManager.SetCache(Cache.HP.ToString(), 9999);
            CacheManager.SetCache(Cache.MPMax.ToString(), 9999);
            CacheManager.SetCache(Cache.MP.ToString(), 9999);
            CacheManager.ChangeKey("Gold", 999);
            CacheManager.ChangeKey("Silver", 999);
            CacheManager.ChangeKey("Copper", 999);
        }
        LoadItemData();
        LoadAssetsFromJson(CacheManager.GetCache(Cache.CurrentStage.ToString()), CacheManager.GetCache(Cache.CurrentFloor.ToString()));
        LoadAssetsForButtonsUI(); 
        LoadAssetsForMenuUI();
        LoadAssetsForSettingsUI();
        LoadAssetsForStoryUI();
        LoadAssetsForCommonUI();
        LoadAssetsForText();
    }

    static public void LoadShopData(int stage)
    {
        ProductDataWrapper assetsData = LoadJson<ProductDataWrapper>("Json/Game/" + stage + "/Product");
        DataHolder.productsDataList = assetsData != null && assetsData.products != null
            ? assetsData.products
            : new List<ProductData>();
    }

    static public void LoadItemData()
    {
        ItemDataWrapper assetsData = LoadJson<ItemDataWrapper>("Json/Game/Item");
        DataHolder.itemDataList = assetsData != null && assetsData.items != null
            ? assetsData.items
            : new List<ItemData>();
    }

    static public void LoadEnemyData(int stage)
    {
        EnemyDataWrapper assetsData = LoadJson<EnemyDataWrapper>("Json/Game/" + stage + "/EnemyData");
        DataHolder.enemyDataList = assetsData != null && assetsData.targets != null
            ? assetsData.targets
            : new List<EnemyData>();
    }

    public void LoadAssetsForButtonsUI()
    {
        //List<string> paths = new List<string>((new[] { "Purify", "Crouch", "Jump" }));
        DataHolder.gameButtonImages = LoadSprites("UI/GameUI", "Purify", "Down", "Chat", "Gate", "Teleport");
    }

    public void LoadAssetsForMenuUI()
    {
        DataHolder.menuButtonImages = LoadSprites("UI/GameUI", "Setting", "Floor", "Info", "HPBar", "BarGreen", "BarRed", "BarBlue", "Avatar", "Bag");
    }

    public void LoadAssetsForSettingsUI()
    {
        DataHolder.settingImages = LoadSprites("UI/Setting", "Bin");
    }
    public void LoadAssetsForText()
    {
        DataHolder.textPrefabs = LoadPrefabs("Text", "RedText", "GreenText", "BlueText");
    }

    public void LoadAssetsForCommonUI()
    {
        DataHolder.commonImages = LoadSprites("UI/Common", "Close", "Left", "Right", "BigCircleButton", "CircleButton", "SelCircleButton", "SelectionBar", "Panel", "LongButton", "ConfirmPanel", "ShortButton", "NameCard", "NameCard2", "NameCardSel", "Panel2");
    }

    public void LoadAssetsForStoryUI()
    {
        DataHolder.storyImages = LoadSprites("UI/Story", "ChatPanel");
        DataHolder.storyPrefabs = LoadPrefabs("Story", "Player");
    }

    static public void LoadAssetsFromJson(int stage, int floor) {
        LoadAssetsFromJsonWithIsSave(stage, floor, true);
    }

    static public void LoadAssetsFromJsonWithIsSave(int stage, int floor, bool isSave)
    {
        if(isSave) {
            CacheManager.SetCache(Cache.CurrentStage.ToString(), stage);
            MainController.LoadShopData(stage);
            MainController.LoadEnemyData(stage);
        }
        string configPath = "Json/Game/" + stage + "/Config";
        AssetsData assetsData = LoadJson<AssetsData>(configPath);
        if (assetsData != null && assetsData.paths != null)
        {
            Path paths = assetsData.paths;
            DataHolder.walls = LoadSprites("Walls", paths.walls);
            DataHolder.audios = LoadAudios("Audios", paths.audios);
            DataHolder.plateH = LoadSprites("PlateH", paths.plateH);
            DataHolder.plateV = LoadSprites("PlateV", paths.plateV);
            DataHolder.items = LoadSprites("Items", paths.items);
            DataHolder.decorations = LoadSprites("Decorations", paths.decorations);
            DataHolder.obstacles = LoadSprites("Obstacles", paths.obstacles);
            DataHolder.triggers = LoadSprites("Triggers", paths.triggers);
            DataHolder.characters = LoadPrefabs("Characters", paths.characters);
            DataHolder.enemies = LoadPrefabs("Enemies", paths.enemies);
            DataHolder.bosses = LoadPrefabs("Bosses", paths.bosses);
            DataHolder.backgrounds = LoadSprites("Background", paths.backgrounds);
            DataHolder.gems = LoadSprites("Enemies", paths.gems);
            LoadFloorAssetsFromJsonWithIsSave(stage, floor, isSave);
            Debug.Log("LoadFloorAssetsFromJson");
        }
        else
        {
            Debug.LogError("Failed to load stage config: " + configPath);
        }
    }

    static public void LoadFloorAssetsFromJson(int floor){
        LoadFloorAssetsFromJsonWithIsSave(CacheManager.GetCache(Cache.CurrentStage.ToString()), floor, true);
    }

    static public void LoadFloorAssetsFromJsonWithIsSave(int stage, int floor, bool isSave)
    {
        if(isSave) {
            CacheManager.SetCache( Cache.CurrentFloor.ToString(), floor);
        }
        Debug.Log("Json/Game/" + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + floor.ToString());
        string floorPath = "Json/Game/" + stage + "/" + floor;
        AssetsData assetsData = LoadJson<AssetsData>(floorPath);
        if (assetsData != null)
        {
            List<List<AssetData>> assetDatas = new List<List<AssetData>>((new[] { assetsData.assets0, assetsData.assets1, assetsData.assets2 }));
            assetDatas.RemoveAll(list => list == null);
            DataHolder.loadedAssets = assetDatas;
        }
        else
        {
            Debug.LogError("Failed to load floor data: " + floorPath);
            return;
        }
        if(HomeController.loaded) {
            HomeController.LoadGame();
        } else {
            SceneManager.LoadScene("HomeScene");
        }
    }

    public static void SetupScene(){
        // Camera mainCamera = Camera.main;
        // mainCamera.gameObject.AddComponent<CameraController>();
        // Set the orthographic size
        // float desiredSize = 3f;
        // mainCamera.orthographicSize = desiredSize;
    }

    public static List<Sprite> LoadSprites(string folder, List<string> paths)
    {
        return Utils.LoadSprites(folder, paths);
    }

    private static List<Sprite> LoadSprites(string folder, params string[] paths)
        => Utils.LoadSprites(folder, new List<string>(paths));

    public static List<Object> LoadPrefabs(string folder, List<string> paths)
    {
        return Utils.LoadPrefabs(folder, paths);
    }

    private static List<Object> LoadPrefabs(string folder, params string[] paths)
        => Utils.LoadPrefabs(folder, new List<string>(paths));

    public static List<AudioClip> LoadAudios(string folder, List<string> paths)
    {
        return Utils.LoadAudios(paths);
    }

    private static T LoadJson<T>(string resourcePath) where T : class
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);
        if (jsonFile == null)
        {
            Debug.LogError("Missing JSON resource: " + resourcePath);
            return null;
        }

        try
        {
            return JsonUtility.FromJson<T>(jsonFile.text);
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Invalid JSON resource " + resourcePath + ": " + exception.Message);
            return null;
        }
    }

}
