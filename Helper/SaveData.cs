using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CacheUsage
{
    private static string FloorKey =>
        CacheManager.GetCache(Cache.CurrentStage.ToString()) + "/" +
        CacheManager.GetCache(Cache.CurrentFloor.ToString());

    public static void cacheDestroyedObject(string id)
    {
        Dictionary<string, string> saved = CacheManager.LoadDictionary(Cache.Destroy.ToString());
        List<string> destroyed = saved.TryGetValue(FloorKey, out string json)
            ? CacheManager.ConvertJsonToList(json)
            : new List<string>();
        string objectId = id.Split('_')[0];
        if (!destroyed.Contains(objectId)) destroyed.Add(objectId);
        saved[FloorKey] = CacheManager.ConvertListToJson(destroyed);
        CacheManager.SaveDictionary(Cache.Destroy.ToString(), saved);
    }

    public static bool isDestroyObject(string id)
    {
        Dictionary<string, string> saved = CacheManager.LoadDictionary(Cache.Destroy.ToString());
        return saved.TryGetValue(FloorKey, out string json) &&
               CacheManager.ConvertJsonToList(json).Contains(id);
    }
}

[System.Serializable]
public class SerializableDictionary
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public SerializableDictionary() { }

    public SerializableDictionary(Dictionary<string, string> dictionary)
    {
        keys = dictionary.Keys.ToList();
        values = dictionary.Values.ToList();
    }

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        int count = Mathf.Min(keys != null ? keys.Count : 0, values != null ? values.Count : 0);
        for (int i = 0; i < count; i++) result[keys[i]] = values[i];
        return result;
    }
}
