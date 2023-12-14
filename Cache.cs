using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Linq;


public enum Cache
{
    CurrentStage = 0,
    CurrentFloor = 1,
    Copper = 2,
    Silver = 3,
    Gold = 4,
    Destroy = 5
}

public static class CacheUsage
{
    public static void cacheDestroyedObject(string id)
    {
        Dictionary<string, List<string>> dictionary = CacheManager.LoadDictionary(Cache.Destroy.ToString());

        // If the dictionary is null, assign an empty dictionary
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, List<string>>();
        }

        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int CurrentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        string key = CurrentStage.ToString() + "-" + currentFloor.ToString();

        List<string> list;

        // If the dictionary doesn't contain the key or the list is null, assign an empty list
        if (!dictionary.ContainsKey(key) || dictionary[key] == null)
        {
            list = new List<string>();
        }
        else
        {
            list = dictionary[key];
        }
        string lastTwoCharacters = id.Substring(id.Length - 2);
        list.Add(lastTwoCharacters);

        dictionary[key] = list;

        CacheManager.SaveDictionary(Cache.Destroy.ToString(), dictionary);
    }

    public static List<string> loadDestroyObject()
    {
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int CurrentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        string key = CurrentStage.ToString() + "-" + currentFloor.ToString();
        Dictionary<string, List<string>> dictionary = CacheManager.LoadDictionary(Cache.Destroy.ToString());
        return dictionary[key];
    }
}

public static class CacheManager
{
    public static void IncreaseCache(string key, int amount)
    {
        int currentCache = GetCache(key);
        int newCache = currentCache + amount;
        SetCache(key, newCache);
    }

    public static void DecreaseCache(string key, int amount)
    {
        int currentCache = GetCache(key);
        int newCache = currentCache - amount;
        SetCache(key, newCache);
    }

    public static void IncreaseCache(string key, int amount, int maxCache)
    {
        int currentCache = GetCache(key);
        int newCache = currentCache + amount;
        newCache = Mathf.Clamp(newCache, 0, maxCache); // Clamp the newCache value within the range of 0 to maxCache
        SetCache(key, newCache);
    }

    public static void DecreaseCache(string key, int amount, int maxCache)
    {
        int currentCache = GetCache(key);
        int newCache = currentCache - amount;
        newCache = Mathf.Clamp(newCache, 0, maxCache); // Clamp the newCache value within the range of 0 to maxCache
        SetCache(key, newCache);
    }

    public static int GetCache(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }

    public static void SetCache(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static void SaveDictionary(string key, Dictionary<string, List<string>> dictionary)
    {
        // Convert the dictionary to a serializable format
        SerializableDictionary serializableDictionary = new SerializableDictionary(dictionary);

        // Serialize the dictionary to JSON string
        string dictionaryJson = JsonUtility.ToJson(serializableDictionary);

        // Save the JSON string using PlayerPrefs
        PlayerPrefs.SetString(key, dictionaryJson);
        PlayerPrefs.Save();
    }

    public static Dictionary<string, List<string>> LoadDictionary(string key)
    {
        // Retrieve the JSON string from PlayerPrefs
        string dictionaryJson = PlayerPrefs.GetString(key);

        // Deserialize the JSON string to dictionary
        SerializableDictionary serializableDictionary = JsonUtility.FromJson<SerializableDictionary>(dictionaryJson);

        // Convert the dictionary back to the required format
        Dictionary<string, List<string>> dictionary = serializableDictionary.ToDictionary();

        // Return the dictionary
        return dictionary;
    }
}

[System.Serializable]
public class SerializableDictionary
{
    public List<string> keys;
    public List<List<string>> values;

    public SerializableDictionary(Dictionary<string, List<string>> dictionary)
    {
        keys = dictionary.Keys.ToList();
        values = dictionary.Values.ToList();
    }

    public Dictionary<string, List<string>> ToDictionary()
    {
        Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

        for (int i = 0; i < keys.Count; i++)
        {
            dictionary.Add(keys[i], values[i]);
        }

        return dictionary;
    }
}