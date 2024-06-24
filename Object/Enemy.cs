using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyController : MonoBehaviour
{
    public int assetIndex = 0;
    public List<EnemyData> enemyDatas;
    public List<Sprite> avatars;
    public int enemyHP = 0;
    public int enemyDamage = 0;
    int currentHP = 0;
    HPTextComponent hpTextComponent;
    // Use this for initialization

    void Awake (){
        enemyDatas = new List<EnemyData>();
        avatars = new List<Sprite>();

        foreach (var data in DataHolder.enemyDataList)
        {
            if(this.gameObject.name.Contains(data.name)) {
                enemyDatas.Add(data);
                enemyHP = data.hp;
                enemyDamage = data.damage;
                SetupAssetDataNamelist(data.name);
            }
        }
        if( this.gameObject.name.Contains("Boss")){
            this.gameObject.name = "Boss";
        }
    }
    void Start()
    {
        hpTextComponent = this.gameObject.AddComponent<HPTextComponent>();
        hpTextComponent.textColor = Color.red;
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
            int[] digits = new[] { digit1, digit2, digit3, digit4 };
            for (int i = 0; i < 4; i++){
                int digit = digits[i];
                // Create a new GameObject and add a SpriteRenderer component
                GameObject spriteObject = new GameObject();
                SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = DataHolder.gems[digit];
                spriteObject.name = spriteRenderer.sprite.name;
                // Set the position of the SpriteRenderer
                spriteObject.transform.position = positions[i];
                spriteObject.transform.SetParent(transform);
        
                foreach (var data in DataHolder.enemyDataList)
                {
                    if(spriteObject.name.Contains(data.name)) {
                        if(enemyDatas.Contains(data) == false) {
                            enemyDatas.Add(data);
                            avatars.Add(spriteRenderer.sprite);
                            SetupAssetDataNamelist(data.name);
                        }
                        enemyHP = enemyHP + data.hp;
                        enemyDamage = enemyDamage + data.damage;
                    }
                }
                if(spriteObject.name.Contains("Boss")) {
                    this.gameObject.name = "BossGem";
                }
            }
        } else {
            List<string> paths = new List<string>((new[] { enemyDatas[0].name}));
            Sprite sprite = Utils.LoadSprites("Enemies", paths)[0];
            avatars.Add(sprite);
        }
    }

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
        currentHP = enemyHP;
        Utils.AddEffect(this.gameObject, "purify");
        Camera.main.GetComponent<GameUIController>().HideEnemyHPBar(false);
        //if(Utils.isUAT) {
            Die(true);
            return;
        //}
        StartCoroutine(DecreaseHPOverTime());
    }

    private IEnumerator DecreaseHPOverTime()
    {
        while (currentHP > 0)
        {
            bool result = CacheManager.ChangeHPMP(-1 * enemyDamage, false);
            if(result == false) {
                ResumeBattle();
                yield break;
            }
            currentHP -= CacheManager.GetCache(Cache.Damage.ToString());
            Camera.main.GetComponent<GameUIController>().UpdateAllHPMPBar(currentHP, enemyHP);
            yield return new WaitForSeconds(0.4f); // Wait for 1 second
            GameObject gameObject = GameObject.Find("MariaGhost");
            if (gameObject != null)
            {
                int hpRecover = CacheManager.GetCache(Cache.HPRecover.ToString());
                Utils.AddEffect(DataHolder.player, "heart");
                CacheManager.ChangeHPMP(hpRecover, false);
                Camera.main.GetComponent<GameUIController>().UpdateAllHPMPBar(currentHP, enemyHP);
            }
            yield return new WaitForSeconds(0.4f);
            if (currentHP <= 0)
            {
                CacheUsage.cacheDestroyedObject(this.gameObject.name);
                ResumeBattle();
                Die(true);
            }
        }
    }

    public void ResumeBattle(){
        Camera.main.GetComponent<GameUIController>().HideEnemyHPBar(true);
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().ReleaseFighting();
        Utils.RemoveEffect(this.gameObject.transform);
    }

    public void Die(bool inGame){
        if(inGame) {
            ResumeBattle();
            if(this.gameObject.name.Contains("Boss")) {
                GameObject player = GameObject.Find("Player");
                player.GetComponent<PlayerController>().ShowStory();
            } else {
                Utils.Fade(this.gameObject, true);
                StartCoroutine(DelayDestroy());
            }
        } else {
            Utils.Fade(this.gameObject, true);
            StartCoroutine(DelayDestroy());
        }
    }

    private IEnumerator DelayDestroy(){
        yield return new WaitForSeconds(0.3f); // Wait for 1 second
        Destroy(gameObject); 
    }

    private void SetupAssetDataNamelist(string name){
        AssetData.nameList.Add(name);
    }
}
