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
    public List<string> obstacles;
    public List<string> walls;
    public List<string> enemies;
    public List<string> gems;
    public List<string> bosses;
    public List<string> audios;
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
                if (assetIndex / 1000 >= 1) {
                    newObject.name = "Gem";
                    EnemyController enemyController = newObject.AddComponent<EnemyController>();
                    enemyController.ProcessAssetIndex(assetIndex);
                    return newObject;
                } else if (assetIndex / 90 >= 1) {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.bosses[formattedIndex];
                    newObject = Utils.InstantiateObject(prefab);
                } else if (assetIndex / 40 >= 1) {
                    spriteRenderer.sprite = (Sprite)DataHolder.decorations[formattedIndex];
                } else if (assetIndex / 30 >= 1) {
                    spriteRenderer.sprite = (Sprite)DataHolder.obstacles[formattedIndex];
                } else if (assetIndex / 20 >= 1) {
                    spriteRenderer.sprite = (Sprite)DataHolder.items[formattedIndex];
                } else if (assetIndex / 10 >= 1) {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.enemies[formattedIndex];
                    newObject = Utils.InstantiateObject(prefab);
                } else {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.characters[formattedIndex];
                    newObject = Utils.InstantiateObject(prefab);
                    //newObject.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
                    //offsetY = GetCombinedHeight(newObject) / 2;
                    //newObject = Instantiate(prefab, transform.position, transform.rotation);
                }
                break;
            case "Background":
                int index = gridArray0[0];
                spriteRenderer.sprite = DataHolder.backgrounds[index];
                // spriteRenderer.drawMode = index == 0 ? SpriteDrawMode.Tiled : SpriteDrawMode.Simple;
                spriteRenderer.size = DataHolder.GetBackgroundSize();
                newObject.transform.localScale = new Vector3(1.27f, 1.27f, 1f);
                break;
            case "Audio":
                int audioIndex = gridArray0[0];
                Utils.ChangeAudioClipByName(audioIndex);
                return null;
        }
        // --------- //
        string pathName = isSprite ? spriteRenderer.sprite.name : newObject.name.Replace("(Clone)","");
        if(name.Contains("PlateV") || name.Contains("PlateH")) {
            newObject.name = id + "_" + name + "_" + pathName;
        } else if(pathName.Contains("Player")){
            newObject.name = pathName;
        } else if( pathName.Contains("Story")){
            if(!Utils.isUAT) {
                int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
                if(CacheManager.IsCleared() && currentFloor != 2){
                    newObject.SetActive(false);
                    return null;
                } 
            }
            newObject.name = "Story";
        } else if( pathName.Contains("Boss")){
            // newObject.name = "Boss"; do it in enemy.js
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
        if (pathName.Contains("Enemy") || pathName.Contains("Boss"))
        {
            if (CacheUsage.isDestroyObject(id))
            {
                return null;
            }
            EnemyController enemyController = newObject.AddComponent<EnemyController>();
            enemyController.ProcessAssetIndex(assetIndex);
        }
        return newObject;
    }

    public void SetupCollider(GameObject newObject, bool isSprite, int cellValue){
        BoxCollider2D boxCollider = newObject.AddComponent<BoxCollider2D>();
        if (!isSprite)
        {
            if(newObject.name.Contains("Player")) {
                boxCollider.size = new Vector2(0.5f, 1.8f);
            } else {
                boxCollider.size = new Vector2(1.5f, 1.8f);
            }
            boxCollider.offset = new Vector2(0, boxCollider.size.y / 2);
        } else if(newObject.name.Contains("Gem")){
            boxCollider.size = new Vector2(2f, 2f);
        }
        if (name.Contains("Object"))
        {
            if (( cellValue <= 90  && cellValue - 1 >= 40 ) && !newObject.name.Contains("Solid")){ // add box collider for non-solid decoration
                boxCollider.enabled = false;
            } 
        }
    }

    public void CreateObjects(float z, float subStage)
    {
        if (name == "Audio" )
        {
            CreateObjectWithAssetIndex(0, z, "");
            return;
        }
        GameObject newObject;
        Sprite plateSprite = GetPlateSprite();
        Vector2 backgroundSize = DataHolder.GetBackgroundSize();
        float cellHeight = (backgroundSize.y - plateSprite.bounds.size.x) / numRows;
        float cellWidth = (backgroundSize.x - plateSprite.bounds.size.x) / numColumns;
        if (name == "Background") {
            newObject = CreateObjectWithAssetIndex(0, z, "");
            int index = gridArray0[0];
            newObject.transform.position = new Vector3(0, cellHeight / 2 + plateSprite.bounds.size.x / 2, z);
            return;
        }
        List<List<int>> gridArrays = new List<List<int>> { gridArray0, gridArray1, gridArray2, gridArray3, gridArray4, gridArray5, gridArray6 };
        float offset = plateSprite.bounds.size.x / 2;
        float offsetX = +subStage * (backgroundSize.x - plateSprite.bounds.size.x);
        for (int gridIndex = 0; gridIndex < gridArrays.Count; gridIndex++)
        {
            List<int> gridArray = gridArrays[gridIndex];
            for (int column = 0; column < gridArray.Count; column++)
            {
                bool isFirstPage = gridArray.Count > 7;
                float startPositionX = isFirstPage ? backgroundSize.x / 2f * 1.58f : backgroundSize.x / 2f;
                Vector2 gridStartPosition = DataHolder.backgrounds[0].bounds.center - new Vector3(startPositionX, -backgroundSize.y / 2f);
                string id = gridIndex.ToString() + column.ToString();
                int cellValue = Mathf.Abs(gridArray[column]);
                bool isRight = gridArray[column] >= 0;
                if(name.Contains("Object") && column == 0 && !isFirstPage) {
                    Vector3 vector3 = new Vector3(0, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - plateSprite.bounds.size.x / 2 , z * -0.01f - 1);
                    
                    GameObject wall = new GameObject();
                    wall.name = "Wall" + id;
                    SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = (Sprite)DataHolder.walls[gridIndex % 3];
                    wall.transform.position = vector3;
                }
                if (cellValue == 0) { 
                    // wall
                    continue;  
                } else if (cellValue == 1 && name.Contains("Object")){
                    int playerPosition = CacheManager.GetCache(Cache.Exit.ToString());
                    if(
                        (playerPosition == 0 && gridIndex >= 3) 
                        || (playerPosition == 2 && gridIndex < 3) ) {
                    } else {
                        continue;
                    }
                }
                newObject = CreateObjectWithAssetIndex(cellValue - 1, z, id);
                if (newObject == null) continue;
                SetupCollider(newObject, isSprite, cellValue);
                if (name.Contains("PlateV") || name.Contains("PlateH"))
                {
                    if (name.Contains("PlateV"))
                    {
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - offset, z * 0.01f - 4);
                    }
                    else
                    {
                        float newZ = z * 0.01f - 7 ;
                        if(newObject.name.Contains("GoDown")) {
                            newZ = -1.8f;
                        } 
                        newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex) - offset, newZ);
                        newObject.tag = "Ground";
                    }
                }
                else if (name.Contains("Object"))
                {
                    BoxCollider2D boxCollider = newObject.GetComponent<BoxCollider2D>();
                    float objectHeight = isSprite ? boxCollider.size.y / 2 : 0;
                    float newZ = z * 0.01f - 1 ;
                    if (newObject.name.Contains("Player")) {
                        newZ = -9.3f;
                    }
                    newObject.transform.position = new Vector3(gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex + 1) + objectHeight, newZ);
                    if(isRight) { // flip
                        newObject.transform.localScale = new Vector3(-newObject.transform.localScale.x, newObject.transform.localScale.y, newObject.transform.localScale.z);
                    }
                }
                newObject.transform.rotation = Quaternion.identity;
            }
        }
    }
}
