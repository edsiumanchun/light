using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyController : MonoBehaviour
{
    public int assetIndex = 0;
    public EnemyData enemyData;
    int currentHP = 0;
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

    public void ProcessAssetIndex(int index){
        assetIndex = index;
        if(assetIndex >= 1000) { // gem
            int digit1 = assetIndex / 1000 - 1;
            int digit2 = (assetIndex / 100) % 10 - 1;
            int digit3 = (assetIndex / 10) % 10 - 1;
            int digit4 = assetIndex % 10 ;
            float param = 0.5f;
            Vector3[] positions = { new Vector3(-param, param, 0), new Vector3(param, param, 0), new Vector3(param, -param, 0), new Vector3(-param, -param, 0) };
            for (int i = 0; i < 4; i++){
                // Create a new GameObject and add a SpriteRenderer component
                GameObject spriteObject = new GameObject("Digit " + (i + 1));
                SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                // Set the sprite based on the digit
                int digit = new[] { digit1, digit2, digit3, digit4 }[i];
                spriteRenderer.sprite = DataHolder.gems[digit];
                // Set the position of the SpriteRenderer
                spriteObject.transform.position = positions[i];
                spriteObject.transform.SetParent(transform);
            }
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
            Camera.main.GetComponent<GameUIController>().UpdateHPBar(currentHP, enemyData.hp);
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
        if(this.gameObject.name.Contains("Boss")) {
            GameObject player = GameObject.Find("Player");
            player.GetComponent<PlayerController>().ShowStory();
        }
    }

    private IEnumerator DelayDestroy(){
        yield return new WaitForSeconds(0.3f); // Wait for 1 second
        Destroy(gameObject); 
    }
}
