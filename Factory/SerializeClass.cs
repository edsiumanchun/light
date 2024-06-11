using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

[System.Serializable]
public class EnemyDataWrapper
{
    public List<EnemyData> targets;
}

[System.Serializable]
public class EnemyData
{
    public int coin;
    public int hp;
    public int damage;
    public string name;
    public int defend;
}

[System.Serializable]
public class StoryDataWrapper
{
    public List<StoryData> storyArray;
}

[System.Serializable]
public class StoryData
{
    public string name;
    public string leftPrefab;
    public string rightPrefab;
    public string centerPrefab;
    public string backgroundPath;
    public string bgmPath;
    public string[] dialogue;
}



[System.Serializable]
public class ProductDataWrapper
{
    public List<ProductData> products;
}

[System.Serializable]
public class ProductData
{
    public int coin;
    public string name;
}
