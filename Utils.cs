// using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class Utils : MonoBehaviour
{

    public static bool IsMobilePlatform()
    {
        return (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
    }

    static public GameObject CreateImage(Sprite sprite, Vector2 position, Transform parent)
    {
        GameObject imageObj = new GameObject("Image");
        RectTransform imageTransform = imageObj.AddComponent<RectTransform>();
        imageTransform.SetParent(parent); // Set the parent transform
        imageTransform.anchoredPosition = position;
        Image image = imageObj.AddComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
        return imageObj;
    }

    static public GameObject CreateText(Vector2 size, Vector2 position, Transform transform, TextAnchor textAnchor)
    {
        GameObject textObj = new GameObject("Text");
        RectTransform textTransform = textObj.AddComponent<RectTransform>();
        textTransform.SetParent(transform); // Set the parent transform
        textTransform.anchoredPosition = position;
        textTransform.sizeDelta = size;
        Text text = textObj.AddComponent<Text>();

        text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");
        text.fontSize = 33;
        text.color = Color.white;
        text.alignment = textAnchor;
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
        GameObject buttonObject = CreateImageWithText(sprite, imagePosition, textPosition, parent, TextAnchor.MiddleCenter);
        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(onClickAction);
        GameObject imageObject = button.transform.parent.gameObject;
        Button button2 = imageObject.AddComponent<Button>();
        button2.onClick.AddListener(onClickAction);
        return buttonObject;
    }

    static public GameObject CreateImageWithText(Sprite sprite, Vector2 imagePosition, Vector2 textPosition, Transform parent, TextAnchor textAnchor)
    {
        GameObject imageGameObject = CreateImage(sprite, imagePosition, parent);
        GameObject rowTextObject = CreateText(new Vector2(200, 50), textPosition, imageGameObject.transform, textAnchor);
        
        return rowTextObject;
    }

    static public GameObject CreateTextImage(Sprite sprite, Vector2 position, Transform parent)
    {
        return CreateImageWithText(sprite, position, Vector2.zero, parent, TextAnchor.MiddleCenter);
    }

    static public GameObject CreateCanvas(string name, bool haveMask)
    {
        float referenceResolution = 1200;
        GameObject canvasObject = new GameObject(name);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(referenceResolution, referenceResolution);

        // Set the Screen Match Mode and Match Width or Height
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        if(haveMask)
        {
            // aspect ratio
            RectTransform canvasRectTransform = canvasObject.GetComponent<RectTransform>();
            // Create a new GameObject for the background image
            GameObject backgroundImageObject = Utils.CreateImage(null, new Vector2(0, 0), canvasObject.transform);
            backgroundImageObject.name = "BackgroundImageObject";
            RectTransform rectTransform = backgroundImageObject.GetComponent<RectTransform>();                               // Set the Image component
            Image backgroundImage = backgroundImageObject.GetComponent<Image>();
            backgroundImage.color = new Color(0f, 0f, 0f, 0.88f);

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
        Vector2 referenceResolution = canvasScaler.referenceResolution;
        Vector2 actualScreenSize = referenceResolution * scaleFactor;

        // aspect ratio
        RectTransform canvasRectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRectTransform.sizeDelta;
        return new Vector2(actualScreenSize.x, actualScreenSize.y * canvasSize.y / canvasSize.x);
    }
}
