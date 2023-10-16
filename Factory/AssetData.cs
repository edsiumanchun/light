using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class AssetsData
{
    public Path paths;
    public List<AssetData> assets0;
    public List<AssetData> assets1;
    public List<AssetData> assets2;
}

[System.Serializable]
public class Path
{
    public List<string> plateV;
    public List<string> plateH;
    public List<string> objects;
    public List<string> backgrounds;
}

[System.Serializable]
public class AssetData
{
    public string name = ""; // Name of the asset
    public List<int> gridArray0;
    public List<int> gridArray1;
    public List<int> gridArray2;
    public List<int> gridArray3;
    public List<int> gridArray4;
    public List<int> gridArray5;
    public List<int> gridArray6;
    int numRows = 5;
    int numColumns = 5;

    public Sprite GetBackgroundSprite()
    {
        return DataHolder.backgrounds[0];
    }

    public Sprite GetPlateSprite()
    {
        return DataHolder.plateV[0];
    }

    public GameObject CreateObjectWithAssetIndex(int rowIndex, int columnIndex, int assetIndex, float z)
    {
        GameObject newObject = new GameObject();
        SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();
        switch (name)
        {
            case "PlateV":
                spriteRenderer.sprite = DataHolder.plateV[assetIndex];
                break;
            case "PlateH":
                spriteRenderer.sprite = DataHolder.plateH[assetIndex];
                break;
            case "Object":
                spriteRenderer.sprite = DataHolder.objects[assetIndex];
                break;
            case "Background":
                spriteRenderer.sprite = DataHolder.backgrounds[assetIndex];
                break;
        }
        if(name != "Background")
        {
            CacheObjectController cacheObjectController  = newObject.AddComponent<CacheObjectController>();
            CacheObjectData cacheObject = new CacheObjectData();
            cacheObject.rowIndex = rowIndex;
            cacheObject.columnIndex = columnIndex;
            cacheObject.stage = DataHolder.currentStage;
            cacheObject.floor = 1;
            cacheObject.type = name;
        }
        // --------- //
        string pathName = spriteRenderer.sprite.name;
        newObject.name = pathName;
        switch (pathName)
        {
            case "Player":
                PlayerController playerController = newObject.AddComponent<PlayerController>();
                JoystickController joystickController = newObject.AddComponent<JoystickController>();
                Camera.main.gameObject.GetComponent<CameraViewController>().player = newObject;
                break;
            case "Trigger":
                newObject.AddComponent<GateTriggerController>(); // Create a new script GateTrigger and attach it to the object
                break;
            case "YellowKey":
                newObject.AddComponent<KeyController>();
                break;
            case "YellowDoor":
                // newObject.AddComponent<KeyController>();
                break;
        }
        return newObject;
    }

    public void CreateObjects(float z, float subStage)
    {
        if (name == "Background")
        {
            CreateObjectWithAssetIndex(0, 0, 0, z);
            return;
        }
        GameObject newObject;
        Sprite plateSprite = GetPlateSprite();
        Sprite backgroundSprite = GetBackgroundSprite();
        Vector2 gridStartPosition = backgroundSprite.bounds.center - new Vector3(backgroundSprite.bounds.size.x / 2f, -backgroundSprite.bounds.size.y / 2f);
        float cellHeight = (backgroundSprite.bounds.size.y - plateSprite.bounds.size.x) / numRows;
        float cellWidth = (backgroundSprite.bounds.size.x - plateSprite.bounds.size.x) / numColumns;
        List<List<int>> gridArrays = new List<List<int>> { gridArray0, gridArray1, gridArray2, gridArray3, gridArray4, gridArray5, gridArray6 };
        float offset = plateSprite.bounds.size.x / 2;
        float offsetX = +subStage * (backgroundSprite.bounds.size.x - plateSprite.bounds.size.x);

        for (int gridIndex = 0; gridIndex < gridArrays.Count; gridIndex++)
        {
            List<int> gridArray = gridArrays[gridIndex];

            for (int column = 0; column < gridArray.Count; column++)
            {
                int cellValue = gridArray[column];

                if (cellValue == 0)
                {
                    continue;
                }

                newObject = CreateObjectWithAssetIndex(gridIndex, column, cellValue - 1, z);
                if (name == "PlateV" || name == "PlateH")
                {
                    if (name == "PlateV")
                    {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z);
                    }
                    else
                    {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex) - offset, z);
                        newObject.tag = "Ground";
                    }
                }
                else if (name == "Object")
                {
                    newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z);
                }
                newObject.transform.rotation = Quaternion.identity;
                BoxCollider2D boxCollider = newObject.AddComponent<BoxCollider2D>();
            }
        }
    }
}
