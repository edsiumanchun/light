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
    string pathName = paths.Count == 0 ? name :paths[assetIndex];
    GameObject newObject = new GameObject(pathName + index.ToString());
    SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();
    spriteRenderer.sprite = tempAssets[assetIndex] as Sprite;
    newObject.transform.rotation = Quaternion.identity;
    if (name != "Background")
    {
        BoxCollider2D boxCollider = newObject.AddComponent<BoxCollider2D>();
    }
    return newObject;
}

    public void CreateObjects(float z)
    {
        GameObject newObject ;
        if (name == "Background") {
            newObject = CreateObjectWithAssetIndex(0, 0, z);
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

                newObject = CreateObjectWithAssetIndex(gridIndex * 10 + column, cellValue - 1, z);
                if (name == "PlateV" || name == "PlateH")
                {
                    if(name == "PlateV") {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column) + offset, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z);   
                    } else {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset, gridStartPosition.y - cellHeight * (gridIndex) - offset, z); 
                    }
                    switch (cellValue)
                    {
                        case 1:
                        case 2:
                            break;
                        case 3:
                            BoxCollider2D boxCollider = newObject.GetComponent<BoxCollider2D>();
                            boxCollider.isTrigger = true;
                            boxCollider.usedByEffector = true;
                            boxCollider.enabled = false;

                            // Add a custom effector to control the collision behavior
                            PlatformEffector2D effector = newObject.AddComponent<PlatformEffector2D>();
                            effector.useColliderMask = true;
                            effector.colliderMask = LayerMask.GetMask("Default"); // Add the appropriate layer(s) to the mask
                            effector.rotationalOffset = 180; // Adjust the rotational offset as needed
                            break;
                    }
                }
                else if (name == "Object")
                {
                    if(cellValue == 1)
                    {
                        PlayerController playerController = newObject.AddComponent<PlayerController>();
                        JoystickController joystickController = newObject.AddComponent<JoystickController>();
                        // Additional configuration for player and joystick controllers
                    }
                    newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z); 
                }
            }
        }
    }
}
