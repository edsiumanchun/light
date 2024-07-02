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
    ConfirmPanel = 9,
    ShortButton = 10
}

public enum UIGameButton
{
    Purify = 0,
    Down = 1,
    Story = 2,
    Gate = 3,
    Exit = 98,
    none = 99
}

public static class DataHolder
{
    // public static float scale;
    public static GameObject player;
    public static List<List<AssetData>> loadedAssets;
    public static List<AssetData> loadedSound;
    public static List<AssetData> loadedGameObject;

    public static List<Sprite> items;
    public static List<Sprite> decorations;
    public static List<Object> characters;
    public static List<Object> storyPrefabs;
    public static List<Sprite> plateH;
    public static List<Sprite> plateV;
    public static List<Sprite> backgrounds;
    public static List<Sprite> npc;
    public static List<Sprite> triggers;
    public static List<Sprite> obstacles;
    public static List<Sprite> walls;
    public static List<Sprite> products;
    public static List<AudioClip> audios;
    // ui
    public static List<Sprite> gameButtonImages;
    public static List<Sprite> menuButtonImages;
    // ui3
    public static List<Sprite> keyImages;
    public static List<Sprite> settingImages;
    public static List<Sprite> dashboardBackgroundImages;
    public static List<Sprite> gems;

    public static List<Sprite> storyImages;
    public static List<Object> enemies;
    public static List<Object> bosses;

    // common ui
    public static List<Sprite> commonImages;


    public static List<EnemyData> enemyDataList;
    public static List<ProductData> productsDataList; // List to store the loaded asset
    public static List<ItemData> itemDataList;

    public static Vector2 GetBackgroundSize()
    {
        Sprite sprite = DataHolder.walls[0];
        return new Vector2(sprite.bounds.size.x, sprite.bounds.size.y * 5);
    }
}