using UnityEngine;
using System.Collections;

public static class CacheController
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

    public static int GetCache(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }

    public static void SetCache(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    // array
    public static void SetObjectCache(string key, CacheObjectData value)
    {
        string serializedValue = JsonUtility.ToJson(value);
        PlayerPrefs.SetString(key, serializedValue);
        PlayerPrefs.Save();
    }

    public static CacheObjectData GetObjectCache(string key)
    {
        string serializedValue = PlayerPrefs.GetString(key);
        CacheObjectData value = JsonUtility.FromJson<CacheObjectData>(serializedValue);
        return value;
    }
}