using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class FloatingTextController : MonoBehaviour
{

    public void CreateAndAnimateFloatingText(string text, int type, int xFactor) // type == 
    {
        int x = xFactor * 38;
        List<Object> texts = DataHolder.textPrefabs;
        var textGO = Utils.InstantiateObject((GameObject)texts[type]);

        TMP_Text textComponent = textGO.GetComponent<TMP_Text>();
        // Get the Text component
        float offset = type == 0 ? 0 : type == 1 ? 38 : -38;
        textComponent.outlineColor = type == 0 ? new Color32(46, 13, 12, 255) : type == 1 ? new Color32(22, 57 , 96, 255) : new Color32(0, 50, 230, 255);
        textComponent.outlineWidth = 0.23f;
         
        textGO.transform.SetParent(this.gameObject.transform);
        textGO.transform.localPosition = new Vector3(x, offset, textGO.transform.localPosition.z);
        // Set the text, font style, and size
        textComponent.text = text;
        textComponent.fontStyle = FontStyles.Bold;
        textComponent.fontSize = type == 0 ? 57 : 39;
        if(xFactor != 0) {
            textComponent.fontSize = textComponent.fontSize * 0.5f;
        }
        textGO.transform.localScale = new Vector3(0.78f, 1f, 1f);
        textGO.name = "FloatingText" + type.ToString() + ":" + text;
        textGO.tag = "FloatText";

        // Animate the text
        if(xFactor == 0) {
            StartCoroutine(MoveAndFadeText(textGO));
        } else {
            StartCoroutine(MoveAndFadeText2(textGO, xFactor == 1));
        }
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

    private IEnumerator MoveAndFadeText2(GameObject textGO, bool moveRight)
    {
        TMP_Text textComponent = textGO.GetComponent<TMP_Text>();
        RectTransform rectTransform = textGO.GetComponent<RectTransform>();
        float startPositionX = rectTransform.anchoredPosition.x;
        float startPositionY = rectTransform.anchoredPosition.y;
        float endPositionX = moveRight ? startPositionX + 30f : startPositionX - 30f; // Move the text 100 units to the right or left
        float endPositionY = startPositionY + 30f; // Move the text 50 units upward (for the parabolic motion)
        float duration = 0.8f; // 2 seconds
        float fadeOutStartTime = 0.3f; // Start fading out at 1.5 seconds
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;

            // Calculate the parabolic motion
            float parabolicY = Mathf.Lerp(startPositionY, endPositionY, normalizedTime) - Mathf.Pow(normalizedTime, 2) * 50f;

            // Lerp the position
            rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(startPositionX, endPositionX, normalizedTime), parabolicY);

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