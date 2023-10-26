using UnityEngine;

public class GateTriggerController : MonoBehaviour
{
    public float gateMoveSpeed = 1f; // Speed at which the gate moves up
    public float objectMoveSpeed = 1f; // Speed at which the game object moves down

    private bool gateOpened = false; // Flag to track if the gate has been opened
    private float gateOpenHeight; // Height at which the gate will open


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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gateOpened && other.gameObject.name == "Player")
        {
            gateOpened = true;
            OpenGate(true);
            StartCoroutine(MoveObject(false));
        }
    }

    public void ResetPositions()
    {
        OpenGate(false);
        StartCoroutine(MoveObject(true));
        gateOpened = false;
    }

    private void OpenGate(bool isUp)
    {
        GameObject gateObject = GameObject.Find("Gate");
        gateObject.GetComponent<GateController>().OpenGate(isUp);
    }

    private System.Collections.IEnumerator MoveObject(bool isUp)
    {
        Vector3 startPosition = transform.position;
        Vector3 offset = isUp ? Vector3.up * gateOpenHeight : Vector3.down * gateOpenHeight;
        Vector3 targetPosition = startPosition + offset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * objectMoveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
    }
}