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
            CacheManager.IncreaseCache(this.gameObject.name.Replace("Key", ""), 1);
            Debug.Log(this.gameObject.name.Replace("Key", "") + " Key collected! Key cache: " +  CacheManager.GetCache("this.gameObject.name"));
            Destroy(gameObject); // Destroy the key game object after collision
        }
    }
}