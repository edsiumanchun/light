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
    public List<string> items;
    public List<string> characters;
    public List<string> backgrounds;
    public List<string> decorations;
    public List<string> enemies;
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
    bool isSprite = true;

    Vector2 GetCombinedSize(GameObject parentGameObject)
    {
        Bounds combinedBounds = new Bounds();

        foreach (Transform childTransform in parentGameObject.transform)
        {
            SpriteRenderer spriteRenderer = childTransform.GetComponent<SpriteRenderer>();
            if(spriteRenderer == null) { continue; }
            Sprite sprite = spriteRenderer.sprite;
            Bounds objectBounds = sprite.bounds;
            Vector2 localPosition = childTransform.localPosition;
            combinedBounds.Encapsulate(new Vector2(objectBounds.size.x + localPosition.x , objectBounds.size.y + localPosition.y ));
        }
        return combinedBounds.size;
    }

    public Sprite GetPlateSprite()
    {
        return DataHolder.plateV[0];
    }

    public GameObject CreateObjectWithAssetIndex(int assetIndex, float z, string id)
    {
        isSprite = true;
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
                int formattedIndex = assetIndex % 10;
                if (assetIndex / 30 >= 1) {
                    spriteRenderer.sprite = (Sprite)DataHolder.decorations[formattedIndex];
                } else if (assetIndex / 20 >= 1) {
                    spriteRenderer.sprite = (Sprite)DataHolder.items[formattedIndex];
                } else if (assetIndex / 10 >= 1) {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.enemies[formattedIndex];
                    newObject = Utils.InstantiateObject(prefab);
                } else 
                {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.characters[formattedIndex];
                    newObject = Utils.InstantiateObject(prefab);
                    //newObject.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
                    //offsetY = GetCombinedHeight(newObject) / 2;
                    //newObject = Instantiate(prefab, transform.position, transform.rotation);
                }
                break;
            case "Background":
                spriteRenderer.sprite = DataHolder.backgrounds[assetIndex];
                spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                spriteRenderer.size = DataHolder.GetBackgroundSize();
                break;
        }
        // --------- //
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        string pathName = isSprite ? spriteRenderer.sprite.name : newObject.name.Replace("(Clone)","");
        if(name.Contains("PlateV") || name.Contains("PlateH")) {
            newObject.name = id + "_" + name + "_" + pathName;
        } else if(pathName.Contains("Player")){
            newObject.name = pathName;
        } else {
            newObject.name = id + "_" + pathName;
        }
        switch (pathName)
        {
            case "Player":
                newObject.AddComponent<PlayerController>();
                newObject.AddComponent<JoystickController>();
                Camera.main.gameObject.GetComponent<CameraViewController>().player = newObject;
                break;
            case "Trigger":
                newObject.AddComponent<GateTriggerController>(); // Create a new script GateTrigger and attach it to the object
                break;
            case "KeyGold":
            case "KeySilver":
            case "KeyCopper":
                if (CacheUsage.isDestroyObject(id))
                {
                    return null;
                }
                newObject.AddComponent<KeyController>();
                break;
            case "GoUp":
                break;
            case "GoDown":
                break;
            case "Gate":
            case "GateGold":
            case "GateSilver":
            case "GateCopper":
                if (CacheUsage.isDestroyObject("|" + id))
                {
                    return null;
                }
                newObject.AddComponent<GateController>();
                break;
        }
        if (pathName.Contains("Enemy"))
        {
            if (CacheUsage.isDestroyObject(id))
            {
                return null;
            }
            newObject.AddComponent<EnemyController>();
        } else if (name.Contains("Event"))
        {

        }
        return newObject;
    }

    public void CreateObjects(float z, float subStage)
    {
        if (name == "Background")
        {
            CreateObjectWithAssetIndex(0, z, "");
            return;
        }
        GameObject newObject;
        Sprite plateSprite = GetPlateSprite();
        Vector2 backgroundSize = DataHolder.GetBackgroundSize();
        Vector2 gridStartPosition = DataHolder.backgrounds[0].bounds.center - new Vector3(backgroundSize.x / 2f, -backgroundSize.y / 2f);
        float cellHeight = (backgroundSize.y - plateSprite.bounds.size.x) / numRows;
        float cellWidth = (backgroundSize.x - plateSprite.bounds.size.x) / numColumns;
        List<List<int>> gridArrays = new List<List<int>> { gridArray0, gridArray1, gridArray2, gridArray3, gridArray4, gridArray5, gridArray6 };
        float offset = plateSprite.bounds.size.x / 2;
        float offsetX = +subStage * (backgroundSize.x - plateSprite.bounds.size.x);

        string playerPosition = CacheManager.GetStringCache(Cache.Exit.ToString());
        playerPosition = playerPosition == "" ? "40" : playerPosition; 
        for (int gridIndex = 0; gridIndex < gridArrays.Count; gridIndex++)
        {
            List<int> gridArray = gridArrays[gridIndex];
            for (int column = 0; column < gridArray.Count; column++)
            {
                string id = gridIndex.ToString() + column.ToString();
                int cellValue = gridArray[column];
                if (cellValue == 0) {
                    if(playerPosition == id && name.Contains("Object")){
                        newObject = CreateObjectWithAssetIndex(0, z, id);
                    } else {
                        continue;
                    }
                } else {
                    newObject = CreateObjectWithAssetIndex(cellValue - 1, z, id);
                }
                if (newObject == null) continue;
                BoxCollider2D boxCollider = newObject.AddComponent<BoxCollider2D>();
                if (!isSprite)
                {
                    if(newObject.name.Contains("Player")) {
                        boxCollider.size = new Vector2(0.5f, 1.8f);
                    } else {
                        boxCollider.size = new Vector2(1.5f, 1.8f);
                    }
                    boxCollider.offset = new Vector2(0, boxCollider.size.y / 2);
                }
                if (name.Contains("PlateV") || name.Contains("PlateH"))
                {
                    if (name.Contains("PlateV"))
                    {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z * 0.01f - 4);
                    }
                    else
                    {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex) - offset, z * 0.01f - 7);
                        newObject.tag = "Ground";
                    }
                }
                else if (name.Contains("Object"))
                {
                    if ((cellValue - 1 >= 40 && cellValue - 1 < 60) && !newObject.name.Contains("Solid")){ // add box collider for non-solid decoration
                        boxCollider.enabled = false;
                    } 
                    float objectHeight = isSprite ? boxCollider.size.y / 2 : 0;
                    float newZ = newObject.name.Contains("Player") ? -9.3f : z * 0.01f - 1 ;
                    newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 1) + objectHeight, newZ);
                }
                newObject.transform.rotation = Quaternion.identity;
            }
        }
    }
}
