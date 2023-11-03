using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{
    static public Button CreateButton(Sprite sprite, Vector2 position, Transform parent)
    {
        GameObject buttonObject = new GameObject("Button", typeof(RectTransform));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.sprite = sprite;

        Button button = buttonObject.AddComponent<Button>();

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        return button;
    }

    static public Text CreateRow(Sprite sprite, Vector2 position, Transform parent)
    {
        GameObject rowImageObject = new GameObject("RowImage", typeof(RectTransform));
        rowImageObject.transform.SetParent(parent, false);
        Image image = rowImageObject.AddComponent<Image>();
        image.sprite = sprite;
        RectTransform rectTransform = rowImageObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(-200 + position.x, position.y);

        GameObject rowTextObject = new GameObject("RowText", typeof(RectTransform));
        rowTextObject.transform.SetParent(parent, false);
        rectTransform = rowTextObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(200 + position.x, position.y);
        Text text = rowTextObject.AddComponent<Text>();
        text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");
        text.fontSize = 88;
        text.color = Color.red;
        text.alignment = TextAnchor.MiddleRight;

        Debug.Log(text);
        return text;
    }
}
