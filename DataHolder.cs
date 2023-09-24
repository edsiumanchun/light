using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public static class DataHolder
{
    // stages
    public static List<string> stages = new List<string>((new[] { "Egypt", "Lam", "Ding" }));

    public static string currentStage
    {
        get { return stages[0]; }
    }
    // public static float scale; // List to store the loaded assets
    public static GameObject player; // List to store the loaded assets
    public static List<List<AssetData>> loadedAssets; // List to store the loaded assets
    public static List<AssetData> loadedSound; // List to store the loaded assets
    public static List<AssetData> loadedGameObject; // List to store the loaded assets

    public static List<Sprite> objects; // List to store the loaded assets
    public static List<Sprite> plateH; // List to store the loaded assets
    public static List<Sprite> plateV; // List to store the loaded assets
    public static List<Sprite> backgrounds; // List to store the loaded assets
    // ui
    public static List<Sprite> UIButtons; // List to store the loaded assets
    public static Sprite menuButtonImage; // List to store the loaded assets
}