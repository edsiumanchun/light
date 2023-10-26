using UnityEngine;
using System.Collections;

public class KeyController : MonoBehaviour
{
	private void Start()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            CacheController.IncreaseCache(this.gameObject.name, 1);
            Debug.Log("Key collected! Key cache: " + CacheController.GetCache("KeyCache"));
            Destroy(gameObject); // Destroy the key game object after collision
        }
    }
}