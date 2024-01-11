//gitlab: glpat-FBMK5PA9Nqe2cMz8edpx


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    public static List<EnemyData> enemyDataList;
    List<string> stages;
    private void Start()
    {
        LoadAssetsFromJson(CacheManager.GetCache(Cache.CurrentStage.ToString()), CacheManager.GetCache(Cache.CurrentFloor.ToString()), "HomeScene");
        LoadAssetsForButtonsUI();
        LoadAssetsForMenuUI();
        LoadAssetsForSettingsUI();
        LoadAssetsForDashboardUI();
        LoadAssetsForStageUI();
        LoadAssetsForStoryUI();
        LoadAssetsForCommonUI();
        LoadAssetsForSettingBackground();
    }


    static public void LoadEnemyData(int stage)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage.ToString() + "/EnemyData");
        EnemyDataWrapper assetsData = JsonUtility.FromJson<EnemyDataWrapper>(jsonFile.text);
        enemyDataList = assetsData.enemies;
    }

    public void LoadAssetsForButtonsUI()
    {
        //List<string> paths = new List<string>((new[] { "Purify", "Crouch", "Jump" }));
        List<string> paths = new List<string>((new[] { "Purify"}));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.gameButtonImages = sprites;
    }

    public void LoadAssetsForMenuUI()
    {
        List<string> paths = new List<string>((new[] { "Menu" }));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.menuButtonImage = sprites[0];
    }

    public void LoadAssetsForSettingsUI()
    {
        List<string> paths = new List<string>((new[] { "Right", "Left"}));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.settingButtonImages = sprites;
    }


    public void LoadAssetsForDashboardUI()
    {
        List<string> paths = new List<string>((new[] { "Heart", "Shield", "Shield", "Coin","KeyCopper", "KeySilver", "KeyGold", "KeyFlower", "NameCard" }));
        List<Sprite> sprites = LoadSprites("UI/Dashboard", paths);
        DataHolder.keyImages = sprites;
        List<string> paths2 = new List<string>((new[] { "Setting", "Book"}));
        List<Sprite> sprites2 = LoadSprites("UI/Dashboard", paths2);
        DataHolder.dashboardButtonImages = sprites2;

        List<string> paths3 = new List<string>((new[] { "DashboardPanel", "GoldPlate", "Plate" }));
        List<Sprite> sprites3 = LoadSprites("UI/Dashboard", paths3);
        DataHolder.dashboardBackgroundImages = sprites3;
    }


    public void LoadAssetsForCommonUI()
    {
        List<string> paths = new List<string>((new[] { "Close", "Left", "Right", "CommonButton" } ));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.commonImages = sprites;
    }

    public void LoadAssetsForSettingBackground()
    {
        List<string> paths2 = new List<string>((new[] { "SettingPanel"}));
        List<Sprite> sprites2 = LoadSprites("UI", paths2);
        DataHolder.settingBackgroundImage = sprites2[0];
    }

    public void LoadAssetsForStageUI()
    {
        List<string> paths = new List<string>((new[] { "StagePanel" }));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.stageImages = sprites;
    }

    public void LoadAssetsForStoryUI()
    {
        List<string> paths = new List<string>((new[] { "ChatPanel"}));
        List<Sprite> sprites = LoadSprites("UI/Story", paths);
        DataHolder.storyImages = sprites;
    }

    static public void LoadAssetsFromJson(int stage, int floor, string sceneName)
    {
        CacheManager.SetCache(Cache.CurrentStage.ToString(), stage);
        MainController.LoadEnemyData(stage);
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
        List<Sprite> temp = new List<Sprite>();
        foreach (string path in paths)
        {
            Sprite asset = (Sprite)Resources.Load("Sprites/" + folder + "/" + path, typeof(Sprite));
            //print("Sprites/" + folder + "/" + path);
            if (asset != null)
            {
                temp.Add(asset);
            }
        }
        return temp;
    }

    public static List<Object> LoadPrefabs(string folder, List<string> paths)
    {
        List<Object> temp = new List<Object>();
        foreach (string path in paths)
        {
            Object asset = Resources.Load<GameObject>("Prefabs/" + folder + "/" + path.Replace("_p", ""));
            //print("Sprites/" + folder + "/" + path);
            if (asset != null)
            {
                temp.Add(asset);
            }
        }
        return temp;
    }
}
