using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour
{
    private float gateOpenHeight; // Height at which the gate will open
    public float gateMoveSpeed = 1f; // Speed at which the gate moves up

    public Sprite GetPlateSprite()
    {
        return DataHolder.plateV[0];
    }


    private void Start()
    {
        gateOpenHeight = GetPlateSprite().bounds.size.y;
        if (this.gameObject.name == "Gate")
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }
        }
    }

    public void OpenGate(bool isUp)
    {
        StartCoroutine(MoveGate(isUp));
    }

    private System.Collections.IEnumerator MoveGate(bool isUp)
    {
        Vector3 startPosition = this.gameObject.transform.position;
        Vector3 offset = isUp ? Vector3.up * gateOpenHeight : Vector3.down * gateOpenHeight;
        Vector3 targetPosition = startPosition + offset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * gateMoveSpeed;
            this.gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
    }

    public void DestoryGate()
    {
        if (this.gameObject.name == "Gate")
        {
            return;
        }
        string[] splitArray = this.gameObject.name.Split("Gate");
        string cacheKey = splitArray[splitArray.Length - 1];

        int copperKeyCount = CacheManager.GetCache(cacheKey);
        print(cacheKey);
        if (copperKeyCount <= 0)
        {
            return;
        }
        
        CacheManager.DecreaseCache(cacheKey, 1);
        CacheUsage.cacheDestroyedObject("|" + this.gameObject.name);
        Debug.Log(this.gameObject.name.Replace("Gate", "") + " Key collected! Key cache: " + cacheKey);
        Destroy(gameObject); // Destroy the key game object after collision
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().SetPurifyObject(this.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().SetPurifyObject(null);
    }
}



