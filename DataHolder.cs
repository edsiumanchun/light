using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public enum UICommon
{
    Close = 0,
    Left = 1,
    Right = 2,
    BigCircleButton = 3,
    CircleButton = 4,
    SelCircleButton = 5,
    SelectionBar= 6,
    Panel=7,
    LongButton = 8,
    ConfirmPanel = 9
}

public enum UIGameButton
{
    Purify = 0,
    Down = 1,
    Story = 2,
    Gate = 3,
    none = 99
}

public static class DataHolder
{
    // public static float scale; // List to store the loaded assets
    public static GameObject player; // List to store the loaded assets
    public static List<List<AssetData>> loadedAssets; // List to store the loaded assets
    public static List<AssetData> loadedSound; // List to store the loaded assets
    public static List<AssetData> loadedGameObject; // List to store the loaded assets

    public static List<Sprite> items; // List to store the loaded assets
    public static List<Sprite> decorations; // List to store the loaded assets
    public static List<Object> characters; // List to store the loaded assets
    public static List<Object> storyPrefabs; // List to store the loaded assets
    public static List<Sprite> plateH; // List to store the loaded assets
    public static List<Sprite> plateV; // List to store the loaded assets
    public static List<Sprite> backgrounds; // List to store the loaded assets
    // ui
    public static List<Sprite> gameButtonImages; // List to store the loaded assets
    public static List<Sprite> menuButtonImages; // List to store the loaded assets
    // ui3
    public static List<Sprite> keyImages; // List to store the loaded assets
    public static List<Sprite> settingImages; // List to store the loaded assets
    public static List<Sprite> dashboardBackgroundImages;

    public static List<Sprite> storyImages;

    // common ui
    public static List<Sprite> commonImages; // List to store the loaded assets


    public static List<EnemyData> enemyDataList;

    public static Vector2 GetBackgroundSize()
    {
        Sprite sprite = DataHolder.backgrounds[0];
        return new Vector2(sprite.bounds.size.x, sprite.bounds.size.y * 4);
    }
}