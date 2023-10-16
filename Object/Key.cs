using UnityEngine;
using System.Collections;

public class KeyController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CacheController.IncreaseCache("KeyCache", 1);
            Debug.Log("Key collected! Key cache: " + CacheController.GetCache("KeyCache"));
            Destroy(gameObject); // Destroy the key game object after collision
        }
    }
}