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
    public List<EnemyData> enemies;
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
    public string imagePath;
    public string backgroundPath;
    public string bgmPath;
    public string[] dialogue;
}
