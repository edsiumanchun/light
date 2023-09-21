using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class AssetsData
{
    public List<AssetData> assets;
}

[System.Serializable]
public class AssetData
{
    public string name; // Name of the asset
    public List<string> paths; // Path to the asset
    public List<int> gridArray0;
    public List<int> gridArray1;
    public List<int> gridArray2;
    public List<int> gridArray3;
    public List<int> gridArray4;
    int numRows = 4;
    int numColumns = 10;

    [System.NonSerialized]
    public List<object> tempAssets; // The loaded asset
    public Sprite GetBackgroundSprite()
    {
        return DataHolder.loadedAssets[0].tempAssets[0] as Sprite;
    }

    public Sprite GetPlateSprite()
    {
        return DataHolder.loadedAssets[1].tempAssets[0] as Sprite;
    }

    public GameObject CreateObjectWithAssetIndex(int index, int assetIndex, float z)
    {
        GameObject newObject = new GameObject(name + index.ToString());
        SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tempAssets[assetIndex] as Sprite;
        newObject.transform.rotation = Quaternion.identity;
        newObject.transform.position = new Vector3(newObject.transform.position.x, newObject.transform.position.y, z);

        if (name != "Background")
        {
            newObject.AddComponent<BoxCollider2D>();
        }

        if (name == "Object" && index > 1)
        {
            if(index == 1)
            {
                PlayerController playerController = newObject.AddComponent<PlayerController>();
                JoystickController joystickController = newObject.AddComponent<JoystickController>();
                // Additional configuration for player and joystick controllers
            }
        }

        return newObject;
    }

    public void CreateObjects(float z)
    {
        if (name == "Background") {
            GameObject newObject = CreateObjectWithAssetIndex(0, 0, 0);
            Camera.main.GetComponent<CameraController>().AssignBackgroundEdges(newObject);
            return;
        }
        Sprite plateSprite = GetPlateSprite();
        Sprite backgroundSprite = GetBackgroundSprite();
        Vector2 gridStartPosition = backgroundSprite.bounds.center - new Vector3(backgroundSprite.bounds.size.x / 2f, -backgroundSprite.bounds.size.y / 2f);
        float cellHeight = (backgroundSprite.bounds.size.y - plateSprite.bounds.size.x) / numRows;
        float cellWidth = (backgroundSprite.bounds.size.x - plateSprite.bounds.size.x) / numColumns;
        List<List<int>> gridArrays = new List<List<int>> { gridArray0, gridArray1, gridArray2, gridArray3, gridArray4 };
        float offset = plateSprite.bounds.size.x / 2;

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

                if (name == "PlateV" || name == "PlateH")
                {
                    switch (cellValue)
                    {
                        case 1:
                            GameObject newObject1 = CreateObjectWithAssetIndex(gridIndex * 10 + column, cellValue, z);
                            newObject1.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column) + offset, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z);
                            break;
                        case 2:
                            GameObject newObject2 = CreateObjectWithAssetIndex(gridIndex * 10 + column, cellValue, z);
                            newObject2.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset, gridStartPosition.y - cellHeight * (gridIndex) - offset, z);
                            break;
                        case 3:
                            // Handle additional cases if needed
                            break;
                    }
                }
                else if (name == "Object")
                {
                    GameObject newObject = CreateObjectWithAssetIndex(gridIndex * 10 + column, 0, z);
                    newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f), gridStartPosition.y - cellHeight * (gridIndex + 0.5f), z);
                }
            }
        }
    }
}
