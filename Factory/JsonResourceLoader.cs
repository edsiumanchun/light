using UnityEngine;

public static class JsonResourceLoader
{
    public static T Load<T>(string resourcePath) where T : class
    {
        TextAsset file = Resources.Load<TextAsset>(resourcePath);
        if (file == null)
        {
            Debug.LogError("Missing JSON resource: " + resourcePath);
            return null;
        }

        try
        {
            return JsonUtility.FromJson<T>(file.text);
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Invalid JSON resource " + resourcePath + ": " + exception.Message);
            return null;
        }
    }
}
