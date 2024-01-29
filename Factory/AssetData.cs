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
                if (assetIndex / 10 >= 1) {
                    spriteRenderer.sprite = (Sprite)DataHolder.items[formattedIndex];
                } else
                {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.characters[formattedIndex];
                    newObject = HomeController.InstantiateObject(prefab);
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
        newObject.name = pathName == "Player" ? pathName : id + "_" + pathName;
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
                // MainController.LoadFloorAssetsFromJson(currentFloor + 1, "HomeScene");
                break;
            case "GoDown":
                // MainController.LoadFloorAssetsFromJson(currentFloor - 1, "HomeScene");
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

        for (int gridIndex = 0; gridIndex < gridArrays.Count; gridIndex++)
        {
            List<int> gridArray = gridArrays[gridIndex];

            for (int column = 0; column < gridArray.Count; column++)
            {
                int cellValue = gridArray[column];
                if (cellValue == 0) continue;
                newObject = CreateObjectWithAssetIndex(cellValue - 1, z, gridIndex.ToString() + column.ToString());
                if (newObject == null) continue;
                BoxCollider2D boxCollider = newObject.AddComponent<BoxCollider2D>();
                if (!isSprite)
                {
                    boxCollider.size = GetCombinedSize(newObject);
                    boxCollider.offset = new Vector2(0, boxCollider.size.y / 2);
                }
                if (name.Contains("PlateV") || name.Contains("PlateH"))
                {
                    if (name.Contains("PlateV"))
                    {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z);
                    }
                    else
                    {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex) - offset, z);
                        newObject.tag = "Ground";
                    }
                }
                else if (name.Contains("Object"))
                {
                    float objectHeight = isSprite ? boxCollider.size.y / 2 : 0;
                    newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 1) + objectHeight, z);
                }
                newObject.transform.rotation = Quaternion.identity;
            }
        }
    }
}
