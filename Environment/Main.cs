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
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Game/" + stage);
        if (jsonFile != null)
        {
            AssetsData assetsData = JsonUtility.FromJson<AssetsData>(jsonFile.text);
            Path paths = assetsData.paths;
            DataHolder.plateH = LoadSprites("PlateH", paths.plateH);
            DataHolder.plateV = LoadSprites("PlateV", paths.plateV);
            DataHolder.objects = LoadSprites("Object", paths.objects);
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
}
