using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    float currentHP = 0;
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
        currentHP = enemyData.hp;
        Utils.AddEffect(this.gameObject, "purify");
        DataHolder.player.GetComponent<PlayerController>().SetupHPText();
        // hpTextComponent.SetText(enemyData.hp.ToString());
        Camera.main.GetComponent<GameUIController>().HideHPBar(false);
        StartCoroutine(DecreaseHPOverTime());
    }

    private IEnumerator DecreaseHPOverTime()
    {
        while (enemyData.hp > 0)
        {
            if(Cache.HP - enemyData.damage <= 0) {

                StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
                storyController.LoadStoryDataFromJSONWithPath("Special/Die.json");
                yield break;
            }
            yield return new WaitForSeconds(0.8f); // Wait for 1 second
            currentHP -= CacheManager.GetCache(Cache.Damage.ToString());
            CacheManager.DecreaseCache(Cache.HP.ToString(), enemyData.damage);
            Debug.Log(enemyData.damage);
            Camera.main.GetComponent<GameUIController>().UpdateHPBar(currentHP / enemyData.hp);
            if (currentHP <= 0)
            {
                Camera.main.GetComponent<GameUIController>().HideHPBar(true);
                CacheUsage.cacheDestroyedObject(this.gameObject.name);
                DataHolder.player.GetComponent<JoystickController>().ReleaseFighting();
                Destroy(gameObject); 
            }
        }
    }
}
