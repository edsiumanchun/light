using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    private void Start()
    {
        LoadAssetsFromJson("Json/Game/Home", "HomeScene");
    }

    public void LoadAssetsFromJson(string jsonFilePath, string sceneName)
    {
        DataHolder.loadedAssets = new List<AssetData>();
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFilePath);
        if (jsonFile != null)
        {
            AssetsData assetsData = JsonUtility.FromJson<AssetsData>(jsonFile.text);
            if (assetsData != null && assetsData.assets != null)
            {
                foreach (AssetData assetData in assetsData.assets)
                {
                    LoadAsset(assetData, typeof(Sprite));
                }
            }
            else
            {
                Debug.LogError("Failed to parse JSON data.");
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + jsonFilePath);
        }
        SceneManager.LoadScene(sceneName);
        // Create the background
    }

    private void LoadAsset(AssetData assetData, System.Type type)
    {
        var asset = Resources.Load(assetData.path, type);
        if (asset != null)
        {
            assetData.asset = asset;
            DataHolder.loadedAssets.Add(assetData);
        }
        else
        {
            Debug.LogError("Failed to load asset: " + assetData.name);
        }
    }

    public static void SetupScene(){
        Camera mainCamera = Camera.main;
        mainCamera.gameObject.AddComponent<CameraController>();
        // Set the orthographic size
        float desiredSize = 3.3f;
        mainCamera.orthographicSize = desiredSize;
    }
}
