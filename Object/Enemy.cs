using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    // Use this for initialization
    void Start()
    {
        string[] substrings = this.gameObject.name.Split( "Enemy" );
        print(this.gameObject.name);
        enemyData = MainController.enemyDataList[int.Parse(substrings[substrings.Length - 1])];
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
        // need animation
        enemyData.hp -= CacheManager.GetCache(Cache.Damage.ToString());
        CacheManager.DecreaseCache(Cache.HP.ToString(), enemyData.damage);
        CacheUsage.cacheDestroyedObject(this.gameObject.name);
        Destroy(gameObject); // Destroy the key game object after collision
    }
}
