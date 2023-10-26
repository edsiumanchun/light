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
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
        gateOpenHeight = GetPlateSprite().bounds.size.y;
    }

    public void OpenGate(bool isUp)
    {
        StartCoroutine(MoveGate(isUp));
    }

    private System.Collections.IEnumerator MoveGate(bool isUp)
    {
        GameObject gateObject = GameObject.Find("Gate");
        Vector3 startPosition = gateObject.transform.position;
        Vector3 offset = isUp ? Vector3.up * gateOpenHeight : Vector3.down * gateOpenHeight;
        Vector3 targetPosition = startPosition + offset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * gateMoveSpeed;
            gateObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
    }
}



