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

    static public Image CreateImage(Transform transform)
    {
        GameObject imageObj = new GameObject("Image");
        imageObj.transform.SetParent(transform); // Set the parent transform
        Image image = imageObj.AddComponent<Image>();
        return image;
    }

    static public Text CreateText(Transform transform)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(transform); // Set the parent transform
        Text text = textObj.AddComponent<Text>();
        return text;
    }
}
