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
        print(this.gameObject.name);
        foreach (var data in DataHolder.enemyDataList)
        {
            if(this.gameObject.name.Contains(data.name)) {
                enemyData = data;
            }
        }
        if( this.gameObject.name.Contains("Boss")){
            this.gameObject.name = "Boss";
        }
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
        bool result = CacheManager.CheckChangeHP(enemyData.damage);
        if(result == false) {
            StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
            storyController.LoadStoryDataFromJSONWithPath("Special/Die");
            return;
        }
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
            bool result = CacheManager.ChangeHP(enemyData.damage);
            if(result == false) {
                StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
                storyController.LoadStoryDataFromJSONWithPath("Special/Die");
                yield break;
            }
            yield return new WaitForSeconds(0.8f); // Wait for 1 second
            currentHP -= CacheManager.GetCache(Cache.Damage.ToString());
            Camera.main.GetComponent<GameUIController>().UpdateHPBar(currentHP / enemyData.hp);
            if (currentHP <= 0)
            {
                Camera.main.GetComponent<GameUIController>().HideHPBar(true);
                CacheUsage.cacheDestroyedObject(this.gameObject.name);
                DataHolder.player.GetComponent<JoystickController>().ReleaseFighting();
                Die();
            }
        }
    }

    public void Die(){
        Utils.Fade(this.gameObject);
        StartCoroutine(DelayDestroy());

    }

    private IEnumerator DelayDestroy(){
        yield return new WaitForSeconds(0.3f); // Wait for 1 second
        Destroy(gameObject); 
    }
}
