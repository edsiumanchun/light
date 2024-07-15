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
        if(Utils.isUAT) {
            PlayerPrefs.DeleteAll();
            CacheManager.SetCache(Cache.CurrentStage.ToString(), 0);
            CacheManager.SetCache(Cache.CurrentFloor.ToString(), 9);
            CacheManager.SetCache(Cache.Exit.ToString(),  2); // 2 = down, 0 = up
            CacheManager.SetCache(Cache.ShowTutor.ToString(), 1);
            CacheManager.ChangeKey("Gold", 1);
            //CacheManager.ChangeKey("Silver", 999);
            //CacheManager.ChangeKey("Copper", 999);
        }
        CacheManager.SetCache(Cache.Exit.ToString(), currentFloor == 0 ? 2 : 0);
        LoadItemData();
        LoadAssetsFromJson(CacheManager.GetCache(Cache.CurrentStage.ToString()), CacheManager.GetCache(Cache.CurrentFloor.ToString()), "HomeScene");
        LoadAssetsForButtonsUI();
        LoadAssetsForMenuUI();
        LoadAssetsForSettingsUI();
        LoadAssetsForStoryUI();
        LoadAssetsForCommonUI();
    }

    static public void LoadShopData(int stage)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage.ToString() + "/Product");
        ProductDataWrapper assetsData = JsonUtility.FromJson<ProductDataWrapper>(jsonFile.text);
        DataHolder.productsDataList = assetsData.products;
    }

    static public void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/Item");
        ItemDataWrapper assetsData = JsonUtility.FromJson<ItemDataWrapper>(jsonFile.text);
        DataHolder.itemDataList = assetsData.items;
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
        List<string> paths = new List<string>((new[] { "Setting", "Floor", "Info", "HPBar", "BarGreen", "BarRed", "BarBlue", "Avatar", "Bag" }));
        List<Sprite> sprites = LoadSprites("UI/GameUI", paths);
        DataHolder.menuButtonImages = sprites;
    }

    public void LoadAssetsForSettingsUI()
    {
        List<string> paths2 = new List<string>((new[] { "Bin"}));
        List<Sprite> sprites2 = LoadSprites("UI/Setting", paths2);
        DataHolder.settingImages = sprites2;
    }

    public void LoadAssetsForCommonUI()
    {
        List<string> paths = new List<string>((new[] { "Close", "Left", "Right", "BigCircleButton", "CircleButton", "SelCircleButton", "SelectionBar", "Panel", "LongButton", "ConfirmPanel",  "ShortButton", "NameCard", "NameCard2", "NameCardSel", "Panel2" } ));
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

    static public void LoadAssetsFromJson(int stage, int floor, string sceneName) {
        LoadAssetsFromJsonWithIsSave(stage, floor, sceneName, true);
    }

    static public void LoadAssetsFromJsonWithIsSave(int stage, int floor, string sceneName, bool isSave)
    {
        if(isSave) {
            CacheManager.SetCache(Cache.CurrentStage.ToString(), stage);
            MainController.LoadShopData(stage);
            MainController.LoadEnemyData(stage);
        }
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
            DataHolder.triggers = LoadSprites("Triggers", paths.triggers);
            DataHolder.characters = LoadPrefabs("Characters", paths.characters);
            DataHolder.enemies = LoadPrefabs("Enemies", paths.enemies);
            DataHolder.bosses = LoadPrefabs("Bosses", paths.bosses);
            DataHolder.backgrounds = LoadSprites("Background", paths.backgrounds);
            DataHolder.gems = LoadSprites("Enemies", paths.gems);
            LoadFloorAssetsFromJsonWithIsSave(stage, floor, sceneName, isSave);
            Debug.Log("LoadFloorAssetsFromJson");
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + stage.ToString() + "/" + floor.ToString());
        }
    }

    static public void LoadFloorAssetsFromJson(int floor, string sceneName){
        LoadFloorAssetsFromJsonWithIsSave(CacheManager.GetCache(Cache.CurrentStage.ToString()), floor, sceneName, true);
    }

    static public void LoadFloorAssetsFromJsonWithIsSave(int stage, int floor, string sceneName, bool isSave)
    {
        if(isSave) {
            CacheManager.SetCache( Cache.CurrentFloor.ToString(), floor);
        }
        //print("Json/Game/" + CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" + floor.ToString());
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage + "/" + floor.ToString());
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
        if(HomeController.loaded) {
            HomeController.LoadGame();
        } else {
            SceneManager.LoadScene(sceneName);
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

    public static List<Object> LoadPrefabs(string folder, List<string> paths)
    {
        return Utils.LoadPrefabs(folder, paths);
    }

    public static List<AudioClip> LoadAudios(string folder, List<string> paths)
    {
        return Utils.LoadAudios(paths);
    }

}
