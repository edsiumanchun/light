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
        foreach(string path in paths){
            var asset = Resources.Load("Sprites/" + assetData.name + "/" + path, type);
            if(asset != null) {
                assetData.tempAssets.Add(asset);
            } else {
                Debug.LogError("Failed to load asset." + path);
            }
        }
            print(assetData.tempAssets.Count + assetData.name);
        DataHolder.loadedAssets.Add(assetData);
    }

    public static void SetupScene(){
        Camera mainCamera = Camera.main;
        mainCamera.gameObject.AddComponent<CameraController>();
        // Set the orthographic size
        float desiredSize = 3f;
        mainCamera.orthographicSize = desiredSize;
    }
}
