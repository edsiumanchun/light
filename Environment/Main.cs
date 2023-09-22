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
        stages = new List<string>((new [] { "Egypt", "Lam", "Ding" }));
        LoadAssetsFromJson(stages[0], "HomeScene");
        LoadAssetsForButtonsUI();
        LoadAssetsForMenuUI();
    }

    public void LoadAssetsForButtonsUI()
    {
        List<string> paths = new List<string>((new[] { "Purify", "Crouch", "Jump" }));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.UIButtons = sprites;
    }

    public void LoadAssetsForMenuUI()
    {
        List<string> paths = new List<string>((new[] { "Menu" }));
        List<Sprite> sprites = LoadSprites("UI", paths);
        DataHolder.menuButtonImage = sprites[0];
    }

    public void LoadAssetsFromJson(string stage, string sceneName)
    {
        DataHolder.loadedAssets = new List<AssetData>();
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage);
        if (jsonFile != null)
        {
            AssetsData assetsData = JsonUtility.FromJson<AssetsData>(jsonFile.text);
            if (assetsData != null && assetsData.assets != null)
            {
                foreach (AssetData assetData in assetsData.assets)
                {
                    LoadAsset(stage, assetData, typeof(Sprite));
                }
            }
            else
            {
                Debug.LogError("Failed to parse JSON data.");
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + stage);
        }
        SceneManager.LoadScene(sceneName);
        // Create the background
    }

    private void LoadAsset(string stage, AssetData assetData, System.Type type)
    {
        assetData.tempAssets = new List<object>();
        List<string> paths = assetData.paths == null || assetData.paths.Count == 0 ? new List<string>((new [] { stage })) : assetData.paths;
            print(paths);
        //assetData.tempAssets = LoadSprites(assetData.name, paths);
        foreach (string path in paths)
        {
            var asset = Resources.Load("Sprites/" + assetData.name + "/" + path, type);
            if (asset != null)
            {
                assetData.tempAssets.Add(asset);
            }
            else
            {
                Debug.LogError("Failed to load asset." + path);
            }
        }
        DataHolder.loadedAssets.Add(assetData);
    }

    public static void SetupScene(){
        Camera mainCamera = Camera.main;
        mainCamera.gameObject.AddComponent<CameraController>();
        // Set the orthographic size
        float desiredSize = 3f;
        mainCamera.orthographicSize = desiredSize;
    }

    public static List<Sprite> LoadSprites(string folder, List<string> paths)
    {
        List<Sprite> temp = new List<Sprite>();
        foreach (string path in paths)
        {
            Sprite asset = (Sprite)Resources.Load("Sprites/" + folder + "/" + path, typeof(Sprite));
            if (asset != null)
            {
                temp.Add(asset);
            }
        }
        return temp;
    }
}
