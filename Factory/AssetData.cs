using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AssetData 
{
    public static bool showStory = false;
    public static bool goUp = false;
    public static int exit = 0;
    public static int exit2 = 0;
    public static int teleport = 0;
    public static int teleportExit = 0;
    public static List<int> wallSkip;
    public static GameObject gameObject;
    public static HashSet<string> nameList;
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
        return DataHolder.plateV != null && DataHolder.plateV.Count > 0 ? DataHolder.plateV[0] : null;
    }

    public GameObject CreateObjectWithAssetIndex(int assetIndex, float z, string id)
    {
        if(DataHolder.player != null) {
            if(name == "Object" && assetIndex == 0) {
                return null;
            }
        }
        isSprite = true;
        GameObject newObject = new GameObject();
        AssetData.SetParent(newObject);
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
                    enemyController.ProcessAssetIndex(assetIndex + 1);
                    return newObject;
                } else if (assetIndex / 90 >= 1) {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.bosses[formattedIndex];
                    newObject = Utils.InstantiateObject(prefab);
                    AssetData.SetParent(newObject);
                } else if (assetIndex / 50 >= 1) {
                    Debug.Log("formattedIndex:" + formattedIndex.ToString());
                    spriteRenderer.sprite = (Sprite)DataHolder.triggers[formattedIndex];
                    TriggerController triggerController = newObject.AddComponent<TriggerController>();
                    triggerController.name = spriteRenderer.sprite.name;
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
                    AssetData.SetParent(newObject);
                } else {
                    isSprite = false;
                    GameObject prefab = (GameObject)DataHolder.characters[formattedIndex];
                    newObject = Utils.InstantiateObject(prefab);
                    newObject.transform.localPosition = prefab.transform.localPosition;
                    AssetData.SetParent(newObject);
                    //newObject.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
                    //offsetY = GetCombinedHeight(newObject) / 2;
                    //newObject = Instantiate(prefab, transform.position, transform.rotation);
                }
                break;
            case "Background":
                int index = gridArray0[0];
                spriteRenderer.sprite = DataHolder.backgrounds[index];
                // spriteRenderer.drawwMode = index == 0 ? SpriteDrawMode.Tiled : SpriteDrawMode.Simple;
                //spriteRenderer.size = DataHolder.GetBackgroundSize();
                newObject.transform.localScale = new Vector3(1.21f, 1.21f, 1f);
                break;
            case "Audio":
                int audioIndex = gridArray0[0];
                Utils.ChangeAudioClipByName(audioIndex);
                return null;
            case "Exit":
                int exitIndex = gridArray0[0];
                AssetData.exit = exitIndex;
                return null;
            case "Exit2":
                int exit2Index = gridArray0[0];
                AssetData.exit2 = exit2Index;
                return null;
            case "Teleport":
                int teleportIndex = gridArray0[0];
                AssetData.teleport = teleportIndex;
                return null;
            case "TeleportExit":
                int teleportExitIndex = gridArray0[0];
                AssetData.teleportExit = teleportExitIndex;
                return null;
            case "WallSkip":
                AssetData.wallSkip = gridArray0;
                return null;
            case "Effect":
                List<int> effectIndexes = gridArray0;
                for (int i = 0; i < effectIndexes.Count; i++)
                {
                    int effectIndex = effectIndexes[i];
                    Effects.AddEffectIndex(effectIndex);
                }
                return null;
        }
        // --------- //
        // Debug.Log("????" + spriteRenderer.sprite + assetIndex.ToString());
        if (newObject == null || (isSprite && spriteRenderer.sprite == null))
        {
            Debug.LogError("Unable to create asset " + name + " at index " + assetIndex);
            if (newObject != null) Object.Destroy(newObject);
            return null;
        }
        string pathName = isSprite ? spriteRenderer.sprite.name : newObject.name.Replace("(Clone)","");
        if(name.Contains("PlateV") || name.Contains("PlateH")) {
            newObject.name = id + "_" + name + "_" + pathName;
        } else if(pathName.Contains("Player") || pathName.Contains("Rosia") || pathName.Contains("Sion")){
            newObject.name = pathName;
        } else if( pathName.Contains("Story")){
            if(!Utils.isUAT) {
                int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
                if(CacheManager.IsCleared() && currentFloor != 2){
                    newObject.SetActive(false);
                    return null;
                } 
            }
            if(pathName.Contains("Sprite")) { // maria sick
                isSprite = true;
            }
            newObject.name = "Story";
        } else if( pathName.Contains("Boss")){
            newObject.name = pathName;
            newObject.tag = "Boss";
        } else {
            newObject.name = id + "_" + pathName;
        }
        switch (pathName)
        {
            case "Player":
            case "PlayerRosia":
                if(pathName == "PlayerRosia") {
                    newObject.name = "Player";
                    pathName = "Player";
                }
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
            case "GateExit":
            case "GateFake":
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
        if(newObject.name.Contains("Fake") && !newObject.name.Contains("Gate") ){
            return;
        }
        string[] nameParts = newObject.name.Split('_');
        string lastPart = nameParts[nameParts.Length - 1];
            // Get the BoxCollider component and set the Y size
        float multipler = 1f;
        if (float.TryParse(lastPart, out float heightMultiplier)){
            multipler = heightMultiplier;
        }

        BoxCollider2D boxCollider = newObject.AddComponent<BoxCollider2D>();
        bool isTrigger = (cellValue - 1 >= 50 && cellValue <= 60);
        if (!isSprite)
        {
            if(newObject.name.Contains("Player")) {
                boxCollider.size = new Vector2(0.5f, 1.8f);
            } else {
                boxCollider.size = new Vector2(1.5f, 1.8f * multipler);
            }
            boxCollider.offset = new Vector2(0, boxCollider.size.y / 2);
        } else if(newObject.name.Contains("Gem")){
            boxCollider.size = new Vector2(2f, 2f);
        } else if(isTrigger){
            boxCollider.isTrigger = true;
        }
        if (name.Contains("Object"))
        {
            if (( (cellValue <= 90  && cellValue - 1 >= 40 ) && !isTrigger) || newObject.name.Contains("NoBox")){ // add box collider for non-solid decoration
                boxCollider.enabled = false;
            } 
        }
        
    }


    public void CreateObject(float z, float subStage, int column, int gridIndex, int cellValue, List<int> gridArray, int teleport){
        GameObject newObject;
        Sprite plateSprite = GetPlateSprite();
        if (plateSprite == null || DataHolder.backgrounds == null || DataHolder.backgrounds.Count == 0 || DataHolder.backgrounds[0] == null)
        {
            Debug.LogError("Cannot generate floor without a plate and background sprite.");
            return;
        }
        Vector2 backgroundSize = DataHolder.GetBackgroundSize();
        float cellHeight = (backgroundSize.y - plateSprite.bounds.size.x) / numRows;
        float cellWidth = (backgroundSize.x - plateSprite.bounds.size.x) / numColumns;
        float offsetX = +subStage * (backgroundSize.x - plateSprite.bounds.size.x); // useless
        float offset = plateSprite.bounds.size.x / 2;

        if (name == "Background") {
            newObject = CreateObjectWithAssetIndex(0, z, "");
            int index = gridArray[gridIndex];
            newObject.transform.position = new Vector3(offsetX, cellHeight / 2 + plateSprite.bounds.size.x / 2, z);
            return;
        } 

        int margin = name == "PlateV" ? 7 : 6;
        bool isFirstPage = gridArray.Count >= margin;
        float startPositionX = isFirstPage ? backgroundSize.x / 2f * 6f / 5f : backgroundSize.x / 2f;
        Vector2 gridStartPosition = DataHolder.backgrounds[0].bounds.center - new Vector3(startPositionX, -backgroundSize.y / 2f);
        string id = gridIndex.ToString() + column.ToString();
        bool isRight = gridArray[column] >= 0;
        if(name.Contains("Object") && column == 0 && !isFirstPage) {
            bool showWall = false;
            if(AssetData.wallSkip != null) {
                if(AssetData.wallSkip.Contains(gridIndex)) {
                } else {
                    showWall = true;
                }
            } else {
                showWall = true;
            }
            if(showWall) {
                Vector3 vector3 = new Vector3(offsetX, gridStartPosition.y - cellHeight * (gridIndex + 0.5f) - plateSprite.bounds.size.x / 2 , z * -0.01f - 1);
                GameObject wall = new GameObject();
                wall.name = "Wall" + id;
                SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = (Sprite)DataHolder.walls[gridIndex % 3];
                wall.transform.position = vector3;
                AssetData.SetParent(wall);
            }
        }
        if (cellValue == 0) { 
            // wall
            return;  
        } else if (cellValue == 1 && name.Contains("Object")){  // player
            if(teleport >= 1) {
                return;
            }
            int exitH = CacheManager.GetCache(Cache.ExitH.ToString());
            int exitV = CacheManager.GetCache(Cache.ExitV.ToString());
            bool collumnCheck = exitV < 0 ? true : column == exitV;
            if(exitH == -2) { // 入口
                exitH = AssetData.goUp ? 4 : 0;
            }
            if(teleport >= 0) {

                if( (collumnCheck && gridIndex == exitH) 
                || (collumnCheck && gridIndex == exitH - 1) 
                || (collumnCheck && gridIndex == exitH - 2)){
                } else {
                    return;
                }
            }
        }
        newObject = CreateObjectWithAssetIndex(cellValue - 1, z, id);
        if (newObject == null) return;
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
                newObject.transform.position = new Vector3( gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX, gridStartPosition.y - cellHeight * (gridIndex) - offset, newZ);
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
            newObject.transform.position = new Vector3(newObject.transform.localPosition.x + gridStartPosition.x + cellWidth * (column + 0.5f) + offset + offsetX,newObject.transform.localPosition.y + gridStartPosition.y - cellHeight * (gridIndex + 1) + objectHeight, newZ);
            if(isRight) { // flip
                AnimatorController characterAnimator = newObject.GetComponent<AnimatorController>();
                if (characterAnimator != null)
                    characterAnimator.SetFacing(true);
                else
                    newObject.transform.localScale = new Vector3(-newObject.transform.localScale.x, newObject.transform.localScale.y, newObject.transform.localScale.z);
            }
        }
        // create player for teleport
        if(newObject.name.Contains("Teleport")) {
            if(teleport == 1 && newObject.name.Contains("Teleport")
            || teleport == 2 && newObject.name.Contains("TeleportExit")) {
                CreateObject(z, subStage, column, gridIndex, 1, gridArray, -8);
                CacheManager.SetCache(Cache.Teleport.ToString(), 0); 
            } 
        }
    }

    public void CreateObjects(float z, float subStage)
    {
        if( name == "ShowStory") {
            // if(!Utils.isUAT) {
                AssetData.showStory = true;
                return;
            // }
        }
        if (name == "GoUp") {
            AssetData.goUp = true;
            return;
        }
        if (name == "Audio" || name == "Exit" || name == "Exit2" || name == "Teleport" || name == "TeleportExit" || name == "WallSkip" || name == "Effect")
        {
            CreateObjectWithAssetIndex(0, z, "");
            return;
        }
        int teleport = CacheManager.GetCache(Cache.Teleport.ToString());
        List<List<int>> gridArrays = new List<List<int>> { gridArray0, gridArray1, gridArray2, gridArray3, gridArray4, gridArray5, gridArray6 };
        for (int gridIndex =  gridArrays.Count - 1; gridIndex >= 0; gridIndex--)
        {
            List<int> gridArray = gridArrays[gridIndex];
            if (gridArray == null)
            {
                continue;
            }
            for (int column = 0; column < gridArray.Count; column++)
            {
                CreateObject(z, subStage, column, gridIndex, Mathf.Abs(gridArray[column]), gridArray, teleport);
            }
        }
    }

    public static void SetParent(GameObject GO){
        GO.transform.SetParent(AssetData.gameObject.transform);
    }
}
