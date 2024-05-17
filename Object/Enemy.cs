using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    HPTextComponent hpTextComponent;
    // Use this for initialization
    void Start()
    {
        hpTextComponent = this.gameObject.AddComponent<HPTextComponent>();
        hpTextComponent.textColor = Color.red;
        string[] substrings = this.gameObject.name.Split( "Enemy" );
        print(this.gameObject.name);
        enemyData = DataHolder.enemyDataList[int.Parse(substrings[substrings.Length - 1])];
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
        Utils.AddEffect(this.gameObject, "purify");
        DataHolder.player.GetComponent<PlayerController>().SetupHPText();
        hpTextComponent.SetText(enemyData.hp.ToString());
        StartCoroutine(DecreaseHPOverTime());
    }

    private IEnumerator DecreaseHPOverTime()
    {
        while (enemyData.hp > 0)
        {
            yield return new WaitForSeconds(0.88f); // Wait for 1 second
            enemyData.hp -= CacheManager.GetCache(Cache.Damage.ToString());
            hpTextComponent.SetText(enemyData.hp.ToString());

            CacheManager.DecreaseCache(Cache.HP.ToString(), enemyData.damage);
            DataHolder.player.GetComponent<PlayerController>().SetupHPText();

            if (enemyData.hp <= 0)
            {
                CacheUsage.cacheDestroyedObject(this.gameObject.name);
                Destroy(gameObject); 
            }
        }
    }
}
