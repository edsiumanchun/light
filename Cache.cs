using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Linq;


public enum Cache
{
    CurrentStage = 0,
    CurrentFloor = 1,
    Exit = 2,
    Copper = 3,
    Silver = 4,
    Gold = 5,
    HPRecover = 6,
  //  MPRecover = 7,
    Destroy = 8,
    HP = 9,
    MP = 10,
    HPMax = 11,
    MPMax = 12,
    Damage = 13,
    Defend = 14,
    ClearStage = 15,
    ClearFloor = 16,
    Greed = 17,
    Angry = 18,
    Silly = 19,
    ShowTutor = 20
}

public static class CacheUsage
{
    public static void cacheDestroyedObject(string id)
    {
        Dictionary<string, string> dictionary = CacheManager.LoadDictionary(Cache.Destroy.ToString());

        // If the dictionary is null, assign an empty dictionary
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, string>();
        }

        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int CurrentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        string key = CurrentStage.ToString() + "/" + currentFloor.ToString();

        List<string> list;

        // If the dictionary doesn't contain the key or the list is null, assign an empty list
        if (!dictionary.ContainsKey(key) || dictionary[key] == null)
        {
            list = new List<string>();
        }
        else
        {
            list = CacheManager.ConvertJsonToList(dictionary[key]);
        }
        string [] firstTwoCharacters = id.Split("_");
        list.Add(firstTwoCharacters[0]);

        dictionary[key] = CacheManager.ConvertListToJson(list);
        CacheManager.SaveDictionary(Cache.Destroy.ToString(), dictionary);
    }

    public static bool isDestroyObject(string id)
    {
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int CurrentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        string key = CurrentStage.ToString() + "/" + currentFloor.ToString();
        Dictionary<string, string> dictionary = CacheManager.LoadDictionary(Cache.Destroy.ToString());
        if (dictionary.ContainsKey(key) == true)
        {
            return CacheManager.ConvertJsonToList(dictionary[key]).Contains(id);
        } else
        {
            return false;
        }
        
    }
}

public static class CacheManager
{
    public static bool ChangeCoin(int amount, Cache type){
        int coin = CacheManager.GetCache(type.ToString());
        if(coin - amount <= 0) {
            return false;
        } else {
            int margin = amount >= 0 ? 9999999 : 0;
            CacheManager.ChangeCache(type.ToString(), amount, margin);
            return true;
        }
    }

    public static bool CheckChangeCache(int amount, Cache type){
        int value = CacheManager.GetCache(type.ToString());
        if(value + amount < 0) {
            return false;
        } else {
            return true;
        }
    }
    public static bool ChangeHPMP(int hpAmount, bool isCheck){
        bool hp = CheckChangeCache(hpAmount, Cache.HP);
        bool mp = CheckChangeCache(-1, Cache.MP);
        if(hpAmount > 0){
            mp = true;
        }
        if(!isCheck) {
            if(hp == true && mp == true) {
                int hpMax = CacheManager.GetCache(Cache.HPMax.ToString());
                CacheManager.ChangeCache(Cache.HP.ToString(), hpAmount, hpMax);
                if(hpAmount > 0){
                } else {
                    int mpMax = CacheManager.GetCache(Cache.MPMax.ToString());
                    // Debug.Log("?????" + hpAmount + "?????" + mpMax);
                    CacheManager.ChangeCache(Cache.MP.ToString(), -1, mpMax);
                }
            } 
        }
        if(hp == false){
            StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
            storyController.LoadStoryDataFromText("", "體力不足，無法作戰。");
        } else if(mp == false){
            StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
            storyController.LoadStoryDataFromText("", "靈力不足，無法作戰。");
        }
        return hp == true && mp == true;
    }

    public static bool ChangeKey(string cacheKey, int amount){
        int key = CacheManager.GetCache(cacheKey);
        Debug.Log("KEY: " + cacheKey + key.ToString() + "AMOUNT:" + amount + key);
        if(key + amount <= 0) {
            StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
            storyController.LoadStoryDataFromText("", "鑰匙不足。");
            return false;
        } else {
            int margin = amount >= 0 ? 999 : 0;
            CacheManager.ChangeCache(cacheKey, amount, margin);
            return true;
        }
    }
    public static string ConvertListToJson(List<string> stringList)
    {
        Wrapper wrapper = new Wrapper { List = stringList };
        return JsonUtility.ToJson(wrapper);
    }

    public static List<string> ConvertJsonToList(string json)
    {
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
        return wrapper.List;
    }

    public static void ChangeCache(string key, int amount, int maxCache)
    {
        int currentCache = GetCache(key);
        int newCache = currentCache + amount;
        newCache = Mathf.Clamp(newCache, 0, maxCache); // Clamp the newCache value within the range of 0 to maxCache
        SetCache(key, newCache);
    }

    public static int GetCache(string key)
    {
        int defaultValue = 0;
        if(key == Cache.HPMax.ToString() || key == Cache.HP.ToString() || key == Cache.MPMax.ToString() || key == Cache.MP.ToString()){
            defaultValue = 5;
        } else if (key == Cache.HPRecover.ToString() || key == Cache.Damage.ToString()){
            defaultValue = 1;
        } else if (key == Cache.ClearFloor.ToString() || key == Cache.ClearStage.ToString()){
            defaultValue = -1;
        }
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetClearFloor(){
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        CacheManager.SetCache(Cache.ClearFloor.ToString(),currentFloor);
        CacheManager.SetCache(Cache.ClearStage.ToString(),currentStage);
    }

    public static bool IsCleared() {
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        int clearFloor = CacheManager.GetCache(Cache.ClearFloor.ToString());
        int clearStage = CacheManager.GetCache(Cache.ClearStage.ToString());
        if(clearFloor >= currentFloor && clearStage >= currentStage) {
            return true;
        } else {
            return false;
        }
    }

    public static void SetCache(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }


    public static string GetStringCache(string key)
    {
        return PlayerPrefs.GetString(key, "");
    }

    public static void SetStringCache(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static void SaveDictionary(string key, Dictionary<string, string> dictionary)
    {
        // Convert the dictionary to a serializable format
        SerializableDictionary serializableDictionary = new SerializableDictionary(dictionary);

        // Serialize the dictionary to JSON string
        string dictionaryJson = JsonUtility.ToJson(serializableDictionary);
        Debug.Log(key);
        Debug.Log(dictionaryJson);

        // Save the JSON string using PlayerPrefs
        PlayerPrefs.SetString(key, dictionaryJson);
        PlayerPrefs.Save();
    }

    public static Dictionary<string, string> LoadDictionary(string key)
    {
        // Caching.ClearCache();
        // Retrieve the JSON string from PlayerPrefs
        string dictionaryJson = PlayerPrefs.GetString(key);
        Debug.Log(key);
        Debug.Log(dictionaryJson);
        SerializableDictionary serializableDictionary = JsonUtility.FromJson<SerializableDictionary>(dictionaryJson);
        Dictionary<string, string> dictionary;
        if (serializableDictionary == null)
        {
            dictionary = new Dictionary<string, string>();
        } else
        {
            dictionary = serializableDictionary.ToDictionary();
        }

        // Return the dictionary
        return dictionary;
    }


    [System.Serializable]
    private class Wrapper
    {
        public List<string> List;
    }
}

[System.Serializable]
public class SerializableDictionary
{
    public List<string> keys;
    public List<string> values;

    public SerializableDictionary(Dictionary<string, string> dictionary)
    {
        keys = dictionary.Keys.ToList();
        values = dictionary.Values.ToList();
    }

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary.Add(keys[i], values[i]);
        }

        return dictionary;
    }
}