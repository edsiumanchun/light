using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyController : MonoBehaviour
{
    public GameObject skull;
    public int assetIndex = 0;
    public GameObject effect;
    public List<GameObject> gemObjects;
    public List<EnemyData> enemyDatas;
    public EnemyData specialEnemyData;
    public List<Sprite> avatars;
    public int enemyMaxHP = 0;
    public int enemyDamage = 0;
    public int currentHP = 0;

    public bool sidePlayerText;

    HPTextComponent hpTextComponent;
    // Use this for initialization

    void Awake (){
        sidePlayerText = true;
        enemyDatas = new List<EnemyData>();
        avatars = new List<Sprite>();

        if (DataHolder.enemyDataList == null) return;
        foreach (var data in DataHolder.enemyDataList)
        {
            if(this.gameObject.name.Contains(data.name)) {
                enemyDatas.Add(data);
                enemyMaxHP = data.hp;
                enemyDamage = data.damage;
                SetupAssetDataNamelist(data.name);
            }
        }
    //     LaserBeamCreator.CreateLaserBeam(new Vector3(1,1,0), new Vector3(1,1,0),
    // 50f,
    // 0.2f,
    // Color.red, this.gameObject.transform);
    }
    void Start()
    {
        // hpTextComponent = this.gameObject.AddComponent<HPTextComponent>();
        // hpTextComponent.textColor = Color.red;
    }

    public void ProcessAssetIndex(int index){
        assetIndex = index;
        if(assetIndex >= 1000) { // gem
            gemObjects = new List<GameObject>();
            int digit1 = assetIndex / 1000 - 1;
            int digit2 = (assetIndex / 100) % 10 - 1;
            int digit3 = (assetIndex / 10) % 10 - 1;
            int digit4 = assetIndex % 10 - 1;
            float param = 0.5f;
            if(assetIndex == 10000) {

            } 
            Vector3[] positions = { new Vector3(param, param, 0), new Vector3(-param, param, 0), new Vector3(-param, -param, 0), new Vector3(param, -param, 0) };
            
            int[] digits = new[] { digit1, digit2, digit3, digit4 };
            if(assetIndex == 10000) {
                positions = new [] { new Vector3(0, 0, 0)};
                digits = new [] {assetIndex / 1000 - 1};
            } 
            // if(assetIndex >= 10000) {
            //     int digit1 = assetIndex / 10000000 - 1;
            //     int digit2 = (assetIndex / 1000000) % 10 - 1;
            //     int digit3 = (assetIndex / 100000) % 10 - 1;
            //     int digit4 = (assetIndex / 10000) % 10 - 1;
            //     if(digit1 == 1) {
                    
            //     }
            // }
            for (int i = 0; i < digits.Length; i++){
                int digit = digits[i];
                // Create a new GameObject and add a SpriteRenderer component
                if(digit < 0) continue;
                GameObject spriteObject = new GameObject();
                SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                if (DataHolder.gems == null || digit >= DataHolder.gems.Count || DataHolder.gems[digit] == null)
                {
                    Debug.LogError("Missing gem sprite at index " + digit);
                    Destroy(spriteObject);
                    continue;
                }
                spriteRenderer.sprite = DataHolder.gems[digit];
                spriteObject.name = spriteRenderer.sprite.name;
                // Set the position of the SpriteRenderer
                spriteObject.transform.position = positions[i];
                spriteObject.transform.SetParent(transform);
        
                foreach (var data in DataHolder.enemyDataList)
                {
                    // define type
                    if(spriteObject.name.Contains("2")) {
                        spriteObject.tag = "Tower";
                        specialEnemyData = data;
                    }
                    // define type
                    if(spriteObject.name.Contains(data.name)) {
                        if(enemyDatas.Contains(data) == false) {
                            enemyDatas.Add(data);
                            avatars.Add(spriteRenderer.sprite);
                            SetupAssetDataNamelist(data.name);
                        }
                        enemyMaxHP = enemyMaxHP + data.hp;
                        enemyDamage = enemyDamage + data.damage;
                    }
                }
                if(spriteObject.name.Contains("Boss")) {
                    this.gameObject.name = "BossGem";
                }
                gemObjects.Add(spriteObject);
            }
        } else {
            if (enemyDatas.Count == 0)
            {
                Debug.LogError("No enemy data matches prefab " + gameObject.name);
                enabled = false;
                return;
            }
            List<string> paths = new List<string>((new[] { enemyDatas[0].name}));
            List<Sprite> sprites = Utils.LoadSprites("Enemies", paths);
            if (sprites.Count == 0 || sprites[0] == null)
            {
                Debug.LogError("Missing enemy avatar for " + enemyDatas[0].name);
                enabled = false;
                return;
            }
            Sprite sprite = sprites[0];
            avatars.Add(sprite);
        }
        currentHP = enemyMaxHP;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out JoystickController joystickController))
            joystickController.SetPurifyObject(this.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out JoystickController joystickController))
            joystickController.SetPurifyObject(null);
    }


    public void DestroyEnemy()
    {
        effect = Effects.AddEffect(this.gameObject, "purify");
        Camera.main.GetComponent<GameUIController>().HideBattle(false);
        CacheManager.SyncHPMP(false);

        StartCoroutine(DecreaseHPOverTime());
    }
    
    private void BattleStep(int i = 0) {
        GameObject player = GameObject.Find("Player");
        if (i == 0) {
            CacheManager.ChangeHPMP(-1 * enemyDamage, 0);
            StartCoroutine(TowerAttack());
            // effect 
            if(this.gameObject.name.Contains("Gem")) {
                player.GetComponent<PlayerController>().AttackedBySpell("Explode");
            }
            // effect 
            currentHP = currentHP - CacheManager.GetDamage();
            currentHP = Mathf.Max(currentHP, 0);

            Camera.main.GetComponent<GameUIController>().UpdateEnemyBar(currentHP, enemyMaxHP, true);
        } else if(i==1){
            // healing
            GameObject gameObject = GameObject.Find("MariaGhost");
            if (gameObject != null)
            {
                int hpRecover = CacheManager.GetCache(Cache.HPRecover.ToString());
                Effects.AddEffect(player, "heart");
                CacheManager.ChangeHPMP(hpRecover, 0);
                Camera.main.GetComponent<GameUIController>().UpdateEnemyBar(currentHP, enemyMaxHP, true);
            }
            // healing
        }
    }
    private IEnumerator DecreaseHPOverTime()
    {
        while (currentHP > 0)
        {
            bool result = CacheManager.CheckHPMP(-1 * enemyDamage);
            if(result == false) {
                ResumeBattle();
                yield break;
            }
            BattleStep(0);
            yield return new WaitForSeconds(0.5f); 
            // healing
            BattleStep(1);
            // healing
            yield return new WaitForSeconds(0.5f);
        }
        CacheUsage.cacheDestroyedObject(this.gameObject.name);
        Die(true);
    }

    public int getXFactor() {
        sidePlayerText = !sidePlayerText;
        int xFactor = sidePlayerText ? 1 : -1;
        return xFactor;
    }

    public void ResumeBattle(){
        RemoveFloatingText();
        Camera.main.GetComponent<GameUIController>().HideBattle(true);
        GameObject player = GameObject.Find("Player");
        player.GetComponent<JoystickController>().ReleaseFighting();
        Utils.RemoveEffect(this.gameObject.transform);
        CacheManager.SyncHPMP(true); // 真係sync
    }

    public IEnumerator TowerAttack()
    {
        GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Tower");
        int i = 0;
        foreach (GameObject go in gameObjectsWithTag)
        {
            bool isFire = i == gameObjectsWithTag.Length - 1;
            EnemyController owner = go.GetComponentInParent<EnemyController>();
            EnemyData towerData = owner != null ? owner.specialEnemyData : null;
            string type = go.name.Contains("Greed") ? "Greed"
                : go.name.Contains("Hatred") ? "Hatred"
                : go.name.Contains("Delusion") ? "Delusion" : null;
            if (type != null && towerData != null)
                CreateFireBall(go, type, isFire, towerData.hp, towerData.damage);
            i++;

            // Add a 0.03 second delay
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }


    public void Die(bool inGame){
        if(effect != null) {
            Destroy(effect);
        }
        if(inGame) {
            if(this.gameObject.name.Contains("Boss")) {
                GameObject player = GameObject.Find("Player");
                player.GetComponent<PlayerController>().ShowStory();
            } else {
                if(gemObjects != null) { // gem
                    StartCoroutine(GemDie());
                } else {
                    // Effects.AddEffect(this.gameObject, "SecureAura");
                    EnemyRewardService.Grant(assetIndex);
                    Utils.Fade(this.gameObject, true);
                    StartCoroutine(DelayDestroy(this.gameObject));
                }
            }
        } else {
            // Effects.AddEffect(this.gameObject, "SecureAura");
            Utils.Fade(this.gameObject, true);
            StartCoroutine(DelayDestroy(this.gameObject));
        }
    }

    public void RemoveFloatingText(){
        Camera.main.GetComponent<GameUIController>().enemyFloatingTextController.StopAllCoroutines();
        Camera.main.GetComponent<GameUIController>().playerFloatingTextController.StopAllCoroutines();
        GameObject[] floatTextObjects = GameObject.FindGameObjectsWithTag("FloatText");
        foreach (GameObject floatTextObject in floatTextObjects)
        {
            Destroy(floatTextObject);
        }
    }

    private IEnumerator DelayDestroy(GameObject GO){
        yield return new WaitForSeconds(0.7f); // Wait for 1 second
        Destroy(GO); 
        ResumeBattle();
    }

    private IEnumerator GemDie()
    {
        int i = 0;
        foreach (GameObject gemObject in gemObjects)
        {
            Effects.AddEffect(gemObject, "purify");
            Utils.Fade(gemObject, true);
            if(i == gemObjects.Count - 1){
                EnemyRewardService.Grant(assetIndex);
                StartCoroutine(DelayDestroy(this.gameObject));
            } else {
                StartCoroutine(DelayDestroy(gemObject));
            }
            yield return new WaitForSeconds(0.3f);
            i++;
        }
    }

    private void SetupAssetDataNamelist(string name){
        AssetData.nameList.Add(name);
    }

    public void CreateFireBall(GameObject go, string type, bool isFire, int totalHP, int totalMP){
        Transform effectObject = go.transform.Find("effect");
        Transform effectObject2 = go.transform.Find("effect2");
        if(effectObject == null) {
            GameObject effect = Effects.AddEffect(go, "Stone/Skull" + type);
            effect.name = "effect";
            GameObject effect2 = Effects.AddEffect(go, "Stone/Spiral" + type);
            effect2.name = "effect2";
        } else {
            effectObject.gameObject.GetComponent<ParticleSystem>().Play();
            effectObject2.gameObject.GetComponent<ParticleSystem>().Play();
        }
        GameObject player = GameObject.Find("Player");
        GameObject effect3 = Effects.AddEffect(player, "Stone/Hit");
        effect3.name = "effect3";
        effect3.transform.rotation = Random.rotationUniform;
        CacheManager.ChangeHPWithMP(-1 * totalHP, -1 * totalMP, getXFactor());


        // GameObject fireBall = new GameObject("Fireball");
        // FireBallController fireBallController = fireBall.AddComponent<FireBallController>();
        // if(isFire) {
        //     fireBallController.totalHP = totalHP;
        //     fireBallController.totalMP = totalMP;
        // }
        // // fireBall.transform.parent = go.transform;
        // fireBall.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, -9.3f);
        // fireBallController.Fire(type);
    }
}
