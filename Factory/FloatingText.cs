using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class FloatingTextController : MonoBehaviour
{
    public List<Object> texts;

    private void Start()
    {
        List<string> paths = new List<string>((new[] { "RedText", "GreenText", "BlueText" }));
        texts = Utils.LoadPrefabs("Text", paths);
    }

    public void CreateAndAnimateFloatingText(string text, int index)
    {
        var textGO = Utils.InstantiateObject((GameObject)texts[index]);
        textGO.name = "FloatingText";

        TMP_Text textComponent = textGO.GetComponent<TMP_Text>();
        // Get the Text component
        float offset = index == 0 ? 0 : index == 1 ? 38 : -38;
        textComponent.outlineColor = index == 0 ? new Color32(46, 13, 12, 255) : index == 1 ? new Color32(22, 57 , 96, 255) : new Color32(0, 50, 230, 255);
        textComponent.outlineWidth = 0.23f;
         
        textGO.transform.SetParent(this.gameObject.transform);
        textGO.transform.localPosition = new Vector3(0, offset, textGO.transform.localPosition.z);
        // Set the text, font style, and size
        textComponent.text = text;
        textComponent.fontStyle = FontStyles.Bold;
        textComponent.fontSize = index == 0 ? 57 : 39;
        textGO.transform.localScale = new Vector3(0.78f, 1f, 1f);


        // Animate the text
        StartCoroutine(MoveAndFadeText(textGO));
    }

    private IEnumerator MoveAndFadeText(GameObject textGO)
    {
        TMP_Text textComponent = textGO.GetComponent<TMP_Text>();
        RectTransform rectTransform = textGO.GetComponent<RectTransform>();
        float startPosition = rectTransform.anchoredPosition.y;
        float endPosition = startPosition + 30f; // Move the text 100 units upward
        float duration = 1.8f; // 2 seconds
        float fadeOutStartTime = 1.5f; // Start fading out at 1.5 seconds
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;

            // Lerp the position
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(startPosition, endPosition, normalizedTime));

            // Fade out the text after 1.5 seconds
            if (elapsedTime >= fadeOutStartTime)
            {
                float fadeOutProgress = (elapsedTime - fadeOutStartTime) / (duration - fadeOutStartTime);
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f - fadeOutProgress);
            }
            else
            {
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy the text object after the animation is complete
        Destroy(textGO);
    }

}