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

    static public GameObject CreateImage(Transform transform, Vector2 position)
    {
        GameObject imageObj = new GameObject("Image");
        imageObj.transform.SetParent(transform); // Set the parent transform
        imageObj.transform.position = position;
        Image image = imageObj.AddComponent<Image>();
        return imageObj;
    }

    static public GameObject CreateText(Transform transform, Vector2 position)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(transform); // Set the parent transform
        textObj.transform.position = position;
        Text text = textObj.AddComponent<Text>();
        return textObj;
    }
}
