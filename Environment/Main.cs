//gitlab: glpat-FBMK5PA9Nqe2cMz8edpx


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    private void Start()
    {
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        CacheManager.SetCache(Cache.Exit.ToString(), currentFloor == 0 ? 0 : 2);
        if(Utils.isUAT) {
            PlayerPrefs.DeleteAll();
            CacheManager.SetCache(Cache.CurrentStage.ToString(), 0);
            CacheManager.SetCache(Cache.CurrentFloor.ToString(), 4);
            CacheManager.SetCache(Cache.Exit.ToString(), 0);
            CacheManager.SetCache(Cache.ShowTutor.ToString(), 1);
        }
        LoadShopData();
        LoadAssetsForShop();
        LoadAssetsFromJson(CacheManager.GetCache(Cache.CurrentStage.ToString()), CacheManager.GetCache(Cache.CurrentFloor.ToString()), "HomeScene");
        LoadAssetsForButtonsUI();
        LoadAssetsForMenuUI();
        LoadAssetsForSettingsUI();
        LoadAssetsForDashboardUI();
        LoadAssetsForStoryUI();
        LoadAssetsForCommonUI();
    }

    public void LoadAssetsForShop()
    {
        List<string> paths = new List<string>((new[] { "PotionRed", "PotionBlue", "PotionRed", "PotionBlue", "PotionRed", "PotionBlue"}));
        List<Sprite> sprites = LoadSprites("UI/Products", paths);
        DataHolder.products = sprites;
    }

    static public void LoadShopData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/Shop");
        ProductDataWrapper assetsData = JsonUtility.FromJson<ProductDataWrapper>(jsonFile.text);
        DataHolder.productsDataList = assetsData.products;
    }

    static public void LoadEnemyData(int stage)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage.ToString() + "/EnemyData");
        EnemyDataWrapper assetsData = JsonUtility.FromJson<EnemyDataWrapper>(jsonFile.text);
        DataHolder.enemyDataList = assetsData.targets;
    }

    public void LoadAssetsForButtonsUI()
    {
        //List<string> paths = new List<string>((new[] { "Purify", "Crouch", "Jump" }));
        List<string> paths = new List<string>((new[] { "Purify", "Down", "Chat", "Gate"}));
        List<Sprite> sprites = LoadSprites("UI/GameUI", paths);
        DataHolder.gameButtonImages = sprites;
    }

    public void LoadAssetsForMenuUI()
    {
        List<string> paths = new List<string>((new[] { "Setting", "Floor", "Info", "HPBar", "BarGreen", "BarRed", "BarBlue", "Avatar" }));
        List<Sprite> sprites = LoadSprites("UI/GameUI", paths);
        DataHolder.menuButtonImages = sprites;
    }

    public void LoadAssetsForSettingsUI()
    {
        List<string> paths2 = new List<string>((new[] { "Bin"}));
        List<Sprite> sprites2 = LoadSprites("UI/Setting", paths2);
        DataHolder.settingImages = sprites2;
    }


    public void LoadAssetsForDashboardUI()
    {
        // List<string> paths = new List<string>((new[] { "Heart", "Light", "Dark", "Coin","KeyCopper", "KeySilver", "KeyGold", "KeyFlower", "NameCard", "Dark"}));
        List<string> paths = new List<string>((new[] { "GemGreed", "GemAngry", "GemSilly","KeyCopper", "KeySilver", "KeyGold", "Dark", "NameCard", "ShopCard", "Heart", "Attack"}));
        List<Sprite> sprites = LoadSprites("UI/Dashboard", paths);
        DataHolder.keyImages = sprites;

        List<string> paths3 = new List<string>((new[] { "DashboardPanel", "GoldPlate", "Plate" }));
        List<Sprite> sprites3 = LoadSprites("UI/Dashboard", paths3);
        DataHolder.dashboardBackgroundImages = sprites3;
    }


    public void LoadAssetsForCommonUI()
    {
        List<string> paths = new List<string>((new[] { "Close", "Left", "Right", "BigCircleButton", "CircleButton", "SelCircleButton", "SelectionBar", "Panel", "LongButton", "ConfirmPanel",  "ShortButton" } ));
        List<Sprite> sprites = LoadSprites("UI/Common", paths);
        DataHolder.commonImages = sprites;
    }

    public void LoadAssetsForStoryUI()
    {
        List<string> paths = new List<string>((new[] { "ChatPanel"}));
        List<Sprite> sprites = LoadSprites("UI/Story", paths);
        DataHolder.storyImages = sprites;
        List<string> prefabPaths = new List<string>((new[] { "Player"}));
        DataHolder.storyPrefabs = LoadPrefabs("Story", prefabPaths);
    }

    static public void LoadAssetsFromJson(int stage, int floor, string sceneName)
    {
        CacheManager.SetCache(Cache.CurrentStage.ToString(), stage);
        MainController.LoadEnemyData(stage);
        Debug.Log(stage);
        Debug.Log(floor);
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage.ToString() + "/Config");
        AssetsData assetsData = JsonUtility.FromJson<AssetsData>(jsonFile.text);
        if (jsonFile != null)
        {
            Path paths = assetsData.paths;
            DataHolder.walls = LoadSprites("Walls", paths.walls);
            DataHolder.audios = LoadAudios("Audios", paths.audios);
            DataHolder.plateH = LoadSprites("PlateH", paths.plateH);
            DataHolder.plateV = LoadSprites("PlateV", paths.plateV);
            DataHolder.items = LoadSprites("Items", paths.items);
            DataHolder.decorations = LoadSprites("Decorations", paths.decorations);
            DataHolder.obstacles = LoadSprites("Obstacles", paths.obstacles);
            DataHolder.characters = LoadPrefabs("Characters", paths.characters);
            DataHolder.enemies = LoadPrefabs("Enemies", paths.enemies);
            DataHolder.bosses = LoadPrefabs("Bosses", paths.bosses);
            DataHolder.backgrounds = LoadSprites("Background", paths.backgrounds);
            DataHolder.gems = LoadSprites("Gems", paths.gems);
            LoadFloorAssetsFromJson(floor, sceneName);
            Debug.Log("LoadFloorAssetsFromJson");
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + stage.ToString() + "/" + floor.ToString());
        }
    }

    static public void LoadFloorAssetsFromJson(int floor, string sceneName)
    {
        CacheManager.SetCache( Cache.CurrentFloor.ToString(), floor);
        //print("Json/Game/" + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + floor.ToString());
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + floor.ToString());
        AssetsData assetsData = JsonUtility.FromJson<AssetsData>(jsonFile.text);
        if (jsonFile != null)
        {
            List<List<AssetData>> assetDatas = new List<List<AssetData>>((new[] { assetsData.assets0, assetsData.assets1, assetsData.assets2 }));
            assetDatas.RemoveAll(list => list == null);
            DataHolder.loadedAssets = assetDatas;
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + floor.ToString());
        }
        SceneManager.LoadScene(sceneName);
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

    public static List<Object> LoadPrefabs(string folder, List<string> paths)
    {
        return Utils.LoadPrefabs(folder, paths);
    }

    public static List<AudioClip> LoadAudios(string folder, List<string> paths)
    {
        return Utils.LoadAudios(paths);
    }

}
