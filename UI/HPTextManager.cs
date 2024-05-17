using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPTextComponent : MonoBehaviour
{
    public Color textColor;

    public TextMeshPro newTextMeshPro;

    public GameObject textObject;

    private void Start()
    {
        textObject = new GameObject("TextMeshPro");
        AddTextToGameObject();
    }

    public void AddTextToGameObject()
    {
        // Make the targetGameObject the parent of the textObject
        textObject.transform.SetParent(this.gameObject.transform);

        // Set the position and scale of the textObject
        textObject.transform.localPosition = new Vector3(0, 1.88f, 0);
        textObject.transform.localScale = Vector3.one;

        // Attach a TextMeshPro component to the textObject
        newTextMeshPro = textObject.AddComponent<TextMeshPro>();
        newTextMeshPro.name = "TextMeshPro";
        newTextMeshPro.fontSize = 6;
        newTextMeshPro.color = textColor;
        newTextMeshPro.alignment = TextAlignmentOptions.Center;

        // Set the sorting layer and order of the TextMeshPro object
        newTextMeshPro.sortingOrder = 33;
    }

    public void SetText(string textContent)
    {
        textObject.SetActive(true);
        newTextMeshPro.text = textContent;
    }
}