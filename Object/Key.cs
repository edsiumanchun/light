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
            string keyName = this.gameObject.name.Split("_")[1].Replace("Key", "");
            print(keyName);
            CacheManager.IncreaseCache(keyName, 1);
            print(CacheManager.GetCache(keyName));
            Debug.Log(keyName + " Key collected! Key cache: " +  CacheManager.GetCache("this.gameObject.name"));
            Destroy(gameObject); // Destroy the key game object after collision

            DashboardController dashboardController = Camera.main.gameObject.GetComponent<DashboardController>();
            dashboardController.SetText();
        }
    }
}