// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;

public class Utils : MonoBehaviour
{

    public static bool IsMobilePlatform()
    {
        return (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
    }

    static public GameObject CreateImage(Transform transform, Vector2 position, Vector2 size)
    {
        GameObject imageObj = new GameObject("Image");
        RectTransform imageTransform = imageObj.AddComponent<RectTransform>();
        imageTransform.SetParent(transform); // Set the parent transform
        imageTransform.anchoredPosition = position;
        imageTransform.sizeDelta = size;
        Image image = imageObj.AddComponent<Image>();
        return imageObj;
    }

    static public GameObject CreateText(Transform transform, Vector2 position, Vector2 size)
    {
        GameObject textObj = new GameObject("Text");
        RectTransform textTransform = textObj.AddComponent<RectTransform>();
        textTransform.SetParent(transform); // Set the parent transform
        textTransform.anchoredPosition = position;
        textTransform.sizeDelta = size;
        Text text = textObj.AddComponent<Text>();
        return textObj;
    }
}
