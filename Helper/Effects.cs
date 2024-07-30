using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;

public enum Effect
{
    Cloud = 0,
    Star = 1,
    Fire = 2
}

public class Effects : MonoBehaviour
{
    private static void LoadAndMoveCloudPrefabs()
    {
        List<string> paths = new List<string>();
        for (int i = 0; i < 8; i++)
        {
            paths.Add($"Cloud{i}");
            Debug.Log($"Cloud{i}");
        }

        float duration = 10f; // Duration of the horizontal movement
        float distance = 20f; // Distance the clouds will move horizontally
        List<GameObject> cloudObjects = new List<GameObject>();
        List<Sprite> sprites = Utils.LoadSprites("Effects", paths);
        GameObject mainGameObject = GameObject.Find("MainGameObject");
        GameObject clouds = new GameObject();
        clouds.name = "clouds";
        clouds.transform.parent = mainGameObject.transform;

        foreach (Sprite sprite in sprites)
        {
            GameObject cloudObject = new GameObject("Cloud");
            SpriteRenderer cloudRenderer = cloudObject.AddComponent<SpriteRenderer>();
            cloudRenderer.sprite = sprite;

            // Set the cloud object's transform
            cloudObject.transform.parent = clouds.transform;
            cloudObject.transform.localPosition = new Vector3(
                Random.Range(-10f, -8f), // x-position between -2 and 2
                Random.Range(0f, 5f), // y-position between 0 and 5
                -0.04f // z-position is -9
            );
            cloudObject.transform.localScale = Vector3.one * Random.Range(0.8f, 1.3f); // Scale between 0.8 and 1.3

            // Apply the movement tween to the cloud object
            Tween cloudTween = cloudObject.transform.DOLocalMoveX(cloudObject.transform.localPosition.x + distance, duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo) // Loop the movement indefinitely
                .SetDelay(Random.Range(0f, 5f)); // Add a random delay to each cloud

            cloudObjects.Add(cloudObject);
        }
    }
    // effect
    public static GameObject AddEffect(GameObject GO, string effectName) {
        List<string> paths = new List<string>((new[] { effectName}));
        GameObject lightPrefab = (GameObject)Utils.LoadPrefabs("Effect", paths)[0];
        GameObject lightObject = Instantiate(lightPrefab);
        lightObject.name = "Effect";
        lightObject.transform.SetParent(GO.transform);
        if(GO.name.Contains("Enemy") || GO.name.Contains("Boss") || GO.name.Contains("Player")) {
            float y = GO.GetComponent<BoxCollider2D>() == null ? 0.9f : GO.GetComponent<BoxCollider2D>().size.y / 2;
            float height = y / 2;
            lightObject.transform.localPosition = new Vector3(0, height, GO.name.Contains("Player") ? 0.3f : -8f);
        } else {
            lightObject.transform.localPosition = new Vector3(0, 0f, -0.3f);
        }
        return lightObject;
    }

    public static void AddEffectIndex(int effectIndex) 
    {    
        switch (effectIndex)
        {
            case 0:
                // Effects.LoadAndMoveCloudPrefabs();
                break;
            case 1:
                Effects.AddEffect(Camera.main.gameObject, "Nature/Star");
                break;
            case 2:
                Effects.AddEffect(Camera.main.gameObject, "Nature/Fire");
                break;
        }
    }
}