using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour
{
    private float gateOpenHeight; // Height at which the gate will open
    public float gateMoveSpeed = 0f; // Speed at which the gate moves up
    public float fadeSpeed = 1f; // Speed at which the gate fades out

    private SpriteRenderer gateSpriteRenderer;
    private Color originalColor;

    public Sprite GetPlateSprite()
    {
        return DataHolder.plateV[0];
    }

    private void Start()
    {
        gateOpenHeight = GetPlateSprite().bounds.size.y;

        if (gameObject.name == "Gate")
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }
        }

        gateSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = gateSpriteRenderer.color;
    }

    public void ResumeBattle(){
        Camera.main.GetComponent<GameUIController>().HideEnemyHPBar(true);
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().ReleaseFighting();
        Utils.RemoveEffect(this.gameObject.transform);
    }


    public void OpenGateAndDestroy(bool isUp)
    {
        string keyName = this.gameObject.name.Split("Gate")[1];
        print("???" + keyName);
        bool result = CacheManager.ChangeKey(keyName, -1);
        if(result == false) {
            return;
        }
        StartCoroutine(MoveGate(isUp));
        StartCoroutine(FadeOutGate());
    }

    private System.Collections.IEnumerator MoveGate(bool isUp)
    {
        Vector3 startPosition = transform.position;
        Vector3 offset = isUp ? Vector3.up * gateOpenHeight : Vector3.down * gateOpenHeight;
        Vector3 targetPosition = startPosition + offset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * gateMoveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
    }

    private System.Collections.IEnumerator FadeOutGate()
    {
        float t = 0f;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            gateSpriteRenderer.color = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }

        // Fading complete, destroy the game object
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().SetPurifyObject(gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().SetPurifyObject(null);
    }
}