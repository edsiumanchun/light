//gitlab: glpat-FBMK5PA9Nqe2cMz8edpx

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    List<string> stages;
    private void Start()
    {
        LoadAssetsFromJson(DataHolder.currentStage + DataHolder.currentFloor, "HomeScene");
        LoadAssetsForButtonsUI();
        LoadAssetsForMenuUI();
        LoadAssetsForSettingsUI();
        LoadAssetsForDashboardUI();
        LoadAssetsForScrollViewUI();
        LoadAssetsForStoryUI();
        LoadAssetsForCommonUI();
        LoadAssetsForSettingBackground();
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
        List<string> paths = new List<string>((new[] { "KeyCopper", "KeySilver", "KeyGold", "KeyFlower" }));
        List<Sprite> sprites = LoadSprites("Items", paths);
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
        List<string> paths = new List<string>((new[] { "Close"} ));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.commonImages = sprites;
    }

    public void LoadAssetsForSettingBackground()
    {
        List<string> paths2 = new List<string>((new[] { "SettingPanel"}));
        List<Sprite> sprites2 = LoadSprites("UI", paths2);
        DataHolder.settingBackgroundImage = sprites2[0];
    }

    public void LoadAssetsForScrollViewUI()
    {
        List<string> paths = new List<string>((new[] { "SettingPanel", "ScrollViewCell" }));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.scrollViewImages = sprites;
    }

    public void LoadAssetsForStoryUI()
    {
        List<string> paths = new List<string>((new[] { "ChatPanel"}));
        List<Sprite> sprites = LoadSprites("UI/Story", paths);
        DataHolder.storyImages = sprites;
    }

    public void LoadAssetsFromJson(string stage, string sceneName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage);
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

            List<List<AssetData>> assetDatas = new List<List<AssetData>>((new[] { assetsData.assets0, assetsData.assets1, assetsData.assets2 }));
            assetDatas.RemoveAll(list => list == null);
            DataHolder.loadedAssets = assetDatas;
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + stage);
        }
        SceneManager.LoadScene(sceneName);
        // Create the background
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
