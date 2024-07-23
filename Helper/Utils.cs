// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
public class Utils : MonoBehaviour
{
    public static bool isUAT = true;
    public static float referenceResolution = 1200;

    public static bool IsMobilePlatform()
    {
        return (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
    }

    static public Vector2 ScaleToUI(Vector2 size)
    {
        Camera mainCamera = Camera.main;
        Vector2 screenPos = mainCamera.WorldToScreenPoint(size);
        return screenPos;
    }
    static public Vector2 ScaleToWorld(Vector2 size)
    {
        Camera mainCamera = Camera.main;
        Vector2 screenPos = mainCamera.ScreenToWorldPoint(size);
        return screenPos;
    }

    static public void MovePrefab(GameObject prefab, float x, float y) // 角位
    {
        RectTransform imageRectTransform = prefab.GetComponent<RectTransform>();
        float halfHeight = imageRectTransform.rect.height * y * imageRectTransform.localScale.y;
        float halfWidth = imageRectTransform.rect.width * x * imageRectTransform.localScale.x;
        Vector2 currentPosition = imageRectTransform.localPosition;
        currentPosition += new Vector2(halfWidth, halfHeight);
        imageRectTransform.localPosition = currentPosition;
    }

    static public GameObject CreateImage(Sprite sprite, Vector2 position, Transform parent)
    {
        GameObject imageObj = new GameObject("Image");
        RectTransform imageTransform = imageObj.AddComponent<RectTransform>();
        imageTransform.SetParent(parent); // Set the parent transform
        imageTransform.anchoredPosition = position;
        // imageTransform.sizeDelta = new Vector2(30,30);
        Image image = imageObj.AddComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();


        return imageObj;
    }

    static public GameObject CreateText(Vector2 size, Vector2 position, Transform parent, TextAnchor textAnchor)
    {
        GameObject textObj = new GameObject("Text");
        RectTransform textTransform = textObj.AddComponent<RectTransform>();
        textTransform.SetParent(parent); // Set the parent transform
        textTransform.anchoredPosition = position;
        textTransform.sizeDelta = size;
        Text text = textObj.AddComponent<Text>();

        text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");
        text.fontSize = 26;
        text.color = Color.white;
        text.alignment = textAnchor;
        text.lineSpacing = 1.08f;
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        return textObj;
    }

    static public GameObject CreateButton(Sprite sprite, Vector2 position, Transform parent, UnityAction onClickAction)
    {
        GameObject buttonObject = CreateImage(sprite, position, parent);
        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(onClickAction);
        return buttonObject;
    }



    static public GameObject CreateTextButton(Sprite sprite, Vector2 imagePosition, Vector2 textPosition, Transform parent, UnityAction onClickAction)
    {
        GameObject imageGameObject = CreateImage(sprite, imagePosition, parent);
        GameObject rowTextObject = CreateText(new Vector2(100, 50), textPosition, imageGameObject.transform, TextAnchor.MiddleCenter);
        Button button = rowTextObject.AddComponent<Button>();
        button.onClick.AddListener(onClickAction);
        Button button2 = imageGameObject.AddComponent<Button>();
        button2.onClick.AddListener(onClickAction);
        return rowTextObject;
    }

    static public GameObject CreateImageWithText(Sprite sprite, Vector2 imagePosition, Vector2 textPosition, Transform parent, TextAnchor textAnchor)
    {
        GameObject imageGameObject = CreateImage(sprite, imagePosition, parent);
        if(sprite == null) {
            Destroy(imageGameObject.GetComponent<Image>());
        }
        GameObject rowTextObject = CreateText(new Vector2(200, 50), textPosition, imageGameObject.transform, textAnchor);
        
        return rowTextObject;
    }

    static public GameObject CreateTextImage(Sprite sprite, Vector2 position, Transform parent)
    {
        return CreateImageWithText(sprite, position, Vector2.zero, parent, TextAnchor.MiddleCenter);
    }

    static public GameObject CreateCanvas(string name, bool haveMask){
        return Utils.CreateCanvas(name, haveMask, 0.88f);
    }
    static public GameObject CreateCanvas(string name, bool haveMask, float opacity)
    {
        GameObject canvasObject = new GameObject(name);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(referenceResolution, referenceResolution);

        // Set the Screen Match Mode and Match Width or Height
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
        //canvasScaler.matchWidthOrHeight = 0.5f;
        if(haveMask)
        {
            // aspect ratio
            RectTransform canvasRectTransform = canvasObject.GetComponent<RectTransform>();
            // Create a new GameObject for the background image
            GameObject backgroundImageObject = Utils.CreateImage(null, new Vector2(0, 0), canvasObject.transform);
            backgroundImageObject.name = "BackgroundImageObject";
            RectTransform rectTransform = backgroundImageObject.GetComponent<RectTransform>();                               // Set the Image component
            Image backgroundImage = backgroundImageObject.GetComponent<Image>();
            backgroundImage.color = new Color(0f, 0f, 0f, opacity);

            //set size
            RectTransform childRect = backgroundImageObject.GetComponent<RectTransform>();
            childRect.anchorMin = Vector2.zero;
            childRect.anchorMax = Vector2.one;
            childRect.offsetMin = Vector2.zero;
            childRect.offsetMax = Vector2.zero;
        }
        return canvasObject;
    }

    static public Vector2 GetCanvasSize(GameObject gameObject)
    {
        CanvasScaler canvasScaler = gameObject.GetComponent<CanvasScaler>();
        float scaleFactor = canvasScaler.scaleFactor;
        //Vector2 referenceResolution = canvasScaler.referenceResolution;
        //Vector2 actualScreenSize = referenceResolution * scaleFactor;
        // aspect ratio
        RectTransform canvasRectTransform = gameObject.GetComponent<RectTransform>();
        //Debug.Log(referenceResolution);
        //return new Vector2(canvasRectTransform.sizeDelta.x / canvasRectTransform.localScale.x, canvasRectTransform.sizeDelta.y / canvasRectTransform.localScale.y);
        Vector2 canvasSize = canvasRectTransform.sizeDelta;
        return new Vector2(referenceResolution, referenceResolution * canvasSize.y / canvasSize.x);
    }

    static public GameObject CreateNavigationBar(string title, Vector2 position, Transform parent, UnityAction onClickAction)
    {
        GameObject NavBar = CreateText(new Vector2(200, 50), position, parent, TextAnchor.MiddleCenter);
        NavBar.GetComponent<Text>().text = title;
        NavBar.GetComponent<Text>().fontSize = 26;
        GameObject closeButton = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.Close)], new Vector2(260, 0), NavBar.transform, onClickAction);
        return NavBar;
    }

    static public GameObject CreateSelectionBar(string title, Vector2 position, Transform parent, UnityAction onClickActionLeft, UnityAction onClickActionRight)
    {
        GameObject NavBar = CreateImageWithText(DataHolder.commonImages[((int)UICommon.SelectionBar)], position, new Vector2(0, 0), parent, TextAnchor.MiddleCenter);
        NavBar.GetComponent<Text>().text = title;
        NavBar.GetComponent<Text>().fontSize = 23;
        GameObject rightButton = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.Right)], new Vector2(144, 0), NavBar.transform, onClickActionRight);
        GameObject leftButton = Utils.CreateButton(DataHolder.commonImages[((int)UICommon.Left)], new Vector2(-144, 0), NavBar.transform, onClickActionLeft);
        return NavBar;
    }

    static public GameObject CreateAvatarAndRows(Sprite sprite, List<Sprite> sprites, List<string> values, Vector2 position, Transform parent)
    {
        bool isTriple = sprites[0] == null;
        GameObject enemyInfo = Utils.CreateImage(sprite, position, parent.transform);
        for (int i = 0; i < values.Count; i++)
        {
            GameObject cell;
            int offset = isTriple ? -42 : 30;
            int topImageY = 94 - offset;
            if(i == 0){
                cell = CreateImageWithText(sprites[i], new Vector2(0, -1 * i * 14 + topImageY), new Vector2(0, topImageY - 163 + offset), enemyInfo.transform, TextAnchor.MiddleCenter);
                Image uiImage = cell.transform.parent.gameObject.GetComponent<Image>();
                RectTransform uiImageRectTransform = uiImage.rectTransform;
                uiImageRectTransform.sizeDelta = new Vector2(80f, 80f);
            } else {
                cell = CreateImageWithText(sprites[i], new Vector2(-44, -1 * i * 44 + (topImageY - 72)), new Vector2(0, 0), enemyInfo.transform, TextAnchor.MiddleRight);
            }
            cell.GetComponent<Text>().text = values[i];
        }
        return enemyInfo;
    }
    static public GameObject CreateRows(List<string> titles, List<string> values, Vector2 position, Transform parent)
    {
        GameObject info = new GameObject();
        info.transform.SetParent(parent.transform);
        info.transform.localPosition = position;
        for (int i = 0; i < values.Count; i++){
            GameObject title, value;
            title = CreateText(new Vector2(100, 50), new Vector2(-26, -1 * i * 44 ), info.transform, TextAnchor.MiddleLeft);
            title.GetComponent<Text>().text = titles[i];
            value = CreateText(new Vector2(100, 50), new Vector2(26, -1 * i * 44 ), info.transform, TextAnchor.MiddleRight);
            value.GetComponent<Text>().text = values[i];
        }
        return info;
    }

    public static List<AudioClip> LoadAudios(List<string> paths)
    {
        List<AudioClip> temp = new List<AudioClip>();
        foreach (string path in paths)
        {
            AudioClip asset = (AudioClip)Resources.Load("Audios/" + path, typeof(AudioClip));
            print("Audios/" + path);
            if (asset != null)
            {
                temp.Add(asset);
            } else {
                temp.Add(null);
            }
        }
        return temp;
    }
    public static List<Sprite> LoadSprites(string folder, List<string> paths)
    {
        List<Sprite> temp = new List<Sprite>();
        foreach (string path in paths)
        {
            Sprite asset = (Sprite)Resources.Load("Sprites/" + folder + "/" + path, typeof(Sprite));
            //print("Sprites/" + folder + "/" + path);
            if (asset != null)
            {
                temp.Add(asset);
            } else {
                temp.Add(null);
            }
        }
        return temp;
    }

    static public GameObject InstantiateObject(GameObject prefab)
    {
        GameObject gameObject = Instantiate(prefab, Vector2.zero, Quaternion.identity);

        return gameObject;
    }

    static public void showChildrenWithName(string name, Transform transform) {
        foreach (Transform child in transform)
        {
            // Check if the child object's name contains the word "jump"
            if (child.name.ToLower().Contains(name))
            {
                child.gameObject.SetActive(true);  // Enable the child object if its name contains "jump"
            }
            else
            {
                child.gameObject.SetActive(false); // Disable the child object if its name doesn't contain "jump"
            }
        }
    }

    static public void changeAlpha(GameObject GO, float alpha) {
        if(GO == null) return;
        Color imageColor = GO.GetComponent<Image>().color;
        imageColor.a = alpha;
        GO.GetComponent<Image>().color = imageColor;
    }
    
    public static List<Object> LoadPrefabs(string folder, List<string> paths)
    {
        List<Object> temp = new List<Object>();
        foreach (string path in paths)
        {
            Object asset = Resources.Load<GameObject>("Prefabs/" + folder + "/" + path.Replace("_p", ""));
            //print("Sprites/" + folder + "/" + path);
            if (asset != null)
            {
                temp.Add(asset);
            }
        }
        return temp;
    }

    public static GameObject LoadPrefabAndInstantObject(string folder, string name, Vector3 vector3){
        List<string> prefabPaths = new List<string>((new[] {name}));
        GameObject o = (GameObject)LoadPrefabs(folder, prefabPaths)[0];
        GameObject o2 = Utils.InstantiateObject(o);
        o2.transform.localPosition = vector3;
        o2.name = name;
        return o2;
    }

    public static void ChangeAudioClipByName(int sourceIndex)
    {
        PersistentAudioSource.Instance.PlayAudio(sourceIndex);
    }

    public static void Fade(GameObject gameObject, bool isFadeOut){
        
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Set the alpha channel of the color
                Color color = spriteRenderer.color;
                color.a = isFadeOut ? 1f : 0f; // Set the alpha value between 0 and 1
                spriteRenderer.color = color;
            }
        }

        foreach (Transform child in children)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.DOFade(isFadeOut ? 0f : 1f, 0.3f);
            }
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

    public static void RemoveEffect(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                Destroy(child);
            }
        }
    }

    public static void Spawn(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                Destroy(child);
            }
        }
    }

}
