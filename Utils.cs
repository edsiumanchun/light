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
}
