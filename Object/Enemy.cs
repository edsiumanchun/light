using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    public void DestroyEnemy()
    {
        CacheUsage.cacheDestroyedObject(this.gameObject.name);
        Destroy(gameObject); // Destroy the key game object after collision
    }
}
