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
        PlayerPrefs.DeleteAll(); // ???
        LoadAssetsFromJson(CacheManager.GetCache(Cache.CurrentStage.ToString()), CacheManager.GetCache(Cache.CurrentFloor.ToString()), "HomeScene");
        LoadAssetsForButtonsUI();
        LoadAssetsForMenuUI();
        LoadAssetsForSettingsUI();
        LoadAssetsForDashboardUI();
        LoadAssetsForStoryUI();
        LoadAssetsForCommonUI();
    }


    static public void LoadEnemyData(int stage)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage.ToString() + "/EnemyData");
        EnemyDataWrapper assetsData = JsonUtility.FromJson<EnemyDataWrapper>(jsonFile.text);
        DataHolder.enemyDataList = assetsData.enemies;
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
        List<string> paths = new List<string>((new[] { "Setting", "Floor", "Info" }));
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
        List<string> paths = new List<string>((new[] { "Heart", "Light", "Coin","KeyCopper", "KeySilver", "KeyGold", "Dark", "NameCard"}));
        List<Sprite> sprites = LoadSprites("UI/Dashboard", paths);
        DataHolder.keyImages = sprites;

        List<string> paths3 = new List<string>((new[] { "DashboardPanel", "GoldPlate", "Plate" }));
        List<Sprite> sprites3 = LoadSprites("UI/Dashboard", paths3);
        DataHolder.dashboardBackgroundImages = sprites3;
    }


    public void LoadAssetsForCommonUI()
    {
        List<string> paths = new List<string>((new[] { "Close", "Left", "Right", "BigCircleButton", "CircleButton", "SelCircleButton", "SelectionBar", "Panel", "LongButton", "ConfirmPanel" } ));
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
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage.ToString() + "/" + floor.ToString());
        AssetsData assetsData = JsonUtility.FromJson<AssetsData>(jsonFile.text);
        if (jsonFile != null)
        {
            Path paths = assetsData.paths;
            DataHolder.plateH = LoadSprites("PlateH", paths.plateH);
            DataHolder.plateV = LoadSprites("PlateV", paths.plateV);
            DataHolder.items = LoadSprites("Items", paths.items);
            DataHolder.decorations = LoadSprites("Decorations", paths.decorations);
            DataHolder.characters = LoadPrefabs("Characters", paths.characters);
            DataHolder.backgrounds = LoadSprites("Background", paths.backgrounds);
            LoadFloorAssetsFromJson(floor, sceneName);
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
}
