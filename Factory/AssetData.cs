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
    public List<Vector2> positions; // List of positions for the asset
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
    public GameObject CreateObject(int index){
        return CreateObjectWithAssetIndex(index, 0);
    }

    public Sprite GetBackgroundSprite(){
        return DataHolder.loadedAssets[0].tempAssets[0] as Sprite;
    }

    public Sprite GetPlateSprite(){
        return DataHolder.loadedAssets[1].tempAssets[0] as Sprite;
    }

    public Sprite GetDoor(){
        return DataHolder.loadedAssets[2] as AssetData;
    }

    public GameObject CreateObjectWithAssetIndex(int index, int assetIndex){
        GameObject newObject = new GameObject(name + index.ToString());
        SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tempAssets[assetIndex] as Sprite;
        newObject.AddComponent<BoxCollider2D>();
        newObject.transform.rotation = Quaternion.identity;
        return newObject;
        // if(parent != null) {
        //     newObject.transform.SetParent(parent.transform);   
        // }
    }

    public void Create2dObjects()
    {
        Sprite plateSprite = GetPlateSprite();
        Sprite backgroundSprite = GetBackgroundSprite();
        Vector2 gridStartPosition = backgroundSprite.bounds.center - new Vector3(backgroundSprite.bounds.size.x / 2f, -backgroundSprite.bounds.size.y / 2f);
        float cellHeight = (backgroundSprite.bounds.size.y - plateSprite.bounds.size.x ) / numRows;
        float cellWidth = (backgroundSprite.bounds.size.x - plateSprite.bounds.size.x ) / numColumns;
        List<List<int>> gridArrays = new List<List<int>> { gridArray0, gridArray1, gridArray2, gridArray3, gridArray4};

        float offset = plateSprite.bounds.size.x / 2;
        // float offset = 0;

        for (int gridIndex = 0; gridIndex < gridArrays.Count; gridIndex++)
        {
            List<int> gridArray = gridArrays[gridIndex];

            for (int column = 0; column < gridArray.Count; column++)
            {
                int cellValue = gridArray[column];
                int door = gridArray[column];
                if (cellValue == 0){
                    continue;
                }
                if(name == "Plate"){
                    if (cellValue == 1 || cellValue == 3)
                    {
                        GameObject newObject = CreateObjectWithAssetIndex(gridIndex * 10 + column, 0);
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column) + offset, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, -3);
                    }
                    if (cellValue == 2 || cellValue == 3)
                    {
                        GameObject newObject = CreateObjectWithAssetIndex(gridIndex * 10 + column, 1);
                        // SpriteRenderer spriteRenderer = newObject.GetComponent<SpriteRenderer>();
                        // float offset = spriteRenderer.bounds.size.y / 2;
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset, gridStartPosition.y - cellHeight * (gridIndex) - offset, -3.3f);
                    }
                } else if(name == "Object"){
                    GameObject newObject = CreateObjectWithAssetIndex(gridIndex * 10 + column, 0);
                    newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f), gridStartPosition.y - cellHeight * (gridIndex + 0.5f), -3.9f);
                }
            }
        }
    }

    public void CreateObjects() {
        int i = 0;
        if(name == "Door"){
            return;
        }
        if(name == "Plate" || name == "Object"){
            Create2dObjects();
            return;
        }
        foreach (Vector2 position in positions)
        {
            GameObject newObject = CreateObject(i);
            SpriteRenderer spriteRenderer = newObject.GetComponent<SpriteRenderer>();
            // position
            newObject.transform.position = new Vector3(position.x, position.y, name == "Background" ? 0 : -1);
            if (name == "Background") {
                // DataHolder.background = newObject;
                Camera.main.GetComponent<CameraController>().AssignBackgroundEdges(newObject);
            } else if (name == "Player")
            {
                DataHolder.player = newObject;
            } else if(name == "Door"){
                // SpriteRenderer backgroundSprite = DataHolder.background.GetComponent<SpriteRenderer>();
                // spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                // float width = backgroundSprite.sprite.bounds.size.x;
                // float height = backgroundSprite.sprite.bounds.size.y;
                // float screenWidth = Camera.main.orthographicSize * 2f * width / height;
                // float screenHeight = Camera.main.orthographicSize * 2f;


// for door, I want it to have switch 9 case by vector position
//(-1,1),(0,1) this will rotate,(1,1), 
//(-1,0),(1,0), 
//(-1,-1),(0,-1),(1,-1).

//(-1,1), (-1,-1), (1,1) and (1,-1)

                // if (name == "Door")
                // {
                //     DataHolder.doors = new List<GameObject>();
                //     float doorOffsetY = 0f;
                //     float doorOffsetX = 0f;
                //     if (position == new Vector2(1, -1) || position == new Vector2(-1, -1) || position == new Vector2(1, 1) || position == new Vector2(-1, 1)){
                //         doorOffsetY = -1 * spriteRenderer.sprite.bounds.size.y / 2 + -1 * spriteRenderer.sprite.bounds.size.x;
                //     }
                //     if (position == new Vector2(0, 1) || position == new Vector2(0, -1)) { 
                //         newObject.transform.Rotate(0f, 0f, 90f);
                //         doorOffsetY += -1 * spriteRenderer.sprite.bounds.size.x / 2f ;
                //     } else {
                //         doorOffsetX = -1 * spriteRenderer.sprite.bounds.size.x * position.x / 2f;
                //     }
                //     // Set the position
                //     newObject.transform.position = new Vector3(screenWidth / 2f * position.x + doorOffsetX, (screenHeight / 2f + doorOffsetY) * position.y, -3.8f);
                //     DataHolder.doors.Add(newObject);
                // }
                // else
                // {
                    // SpriteRenderer doorSpriteRenderer = DataHolder.doors[0].GetComponent<SpriteRenderer>();
                    // float wallThickness = doorSpriteRenderer.bounds.size.x;
                    // float offset = i % 2 == 0 ? spriteRenderer.size.y / 2　: spriteRenderer.size.x / 2;
                    // offset -= wallThickness ;
                    // switch (i)
                    // {
                    //     case 3:
                    //         // Create a hole at the left bottom with the default size
                    //         newObject.transform.position = new Vector3(-screenWidth / 2f - offset, 0, -3);
                    //         spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);
                    //         break;
                    //     case 1:
                    //         // Default behavior for index 1
                    //         newObject.transform.position = new Vector3(screenWidth / 2f + offset, 0f, -3);
                    //         spriteRenderer.size = new Vector2(spriteRenderer.size.x, height);
                    //         break;
                    //     case 0:
                    //         // Default behavior for index 0
                    //         newObject.transform.position = new Vector3(0f, -screenHeight / 2f - offset, -3.5f);
                    //         spriteRenderer.size = new Vector2(width, spriteRenderer.size.y);
                    //         break;
                    //     case 2:
                    //         // Default behavior for index 2
                    //         newObject.transform.position = new Vector3(0f, screenHeight / 2f + offset, -3.5f);
                    //         spriteRenderer.size = new Vector2(width, spriteRenderer.size.y);
                    //         break;
                    //     default:
                    //         // code block
                    //         break;
                    // }
                // }
            }
            i++;
        }
    }
}