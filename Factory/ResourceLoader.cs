using System.Collections.Generic;
using UnityEngine;

public static class ResourceLoader
{
    public static List<AudioClip> LoadAudio(IEnumerable<string> paths)
    {
        List<AudioClip> assets = new List<AudioClip>();
        if (paths == null) return assets;
        foreach (string path in paths)
            assets.Add(Load<AudioClip>("Audios/" + path));
        return assets;
    }

    public static List<Sprite> LoadSprites(string folder, IEnumerable<string> paths)
        => LoadMany<Sprite>("Sprites/" + folder, paths);

    public static List<Object> LoadPrefabs(string folder, IEnumerable<string> paths)
    {
        List<Object> assets = new List<Object>();
        if (paths == null) return assets;
        foreach (string path in paths)
            assets.Add(Load<GameObject>("Prefabs/" + folder + "/" + path.Replace("_p", "")));
        return assets;
    }

    public static GameObject Instantiate(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Cannot instantiate a missing prefab.");
            return null;
        }
        return Object.Instantiate(prefab, prefab.transform.localPosition, prefab.transform.localRotation);
    }

    private static List<T> LoadMany<T>(string folder, IEnumerable<string> paths) where T : Object
    {
        List<T> assets = new List<T>();
        if (paths == null) return assets;
        foreach (string path in paths) assets.Add(Load<T>(folder + "/" + path));
        return assets;
    }

    private static T Load<T>(string path) where T : Object
    {
        T asset = Resources.Load<T>(path);
        if (asset == null) Debug.LogError("Missing resource: " + path);
        return asset;
    }
}
