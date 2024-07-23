using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public HPTextComponent hpTextComponent;
    public float knockbackForce = 3f; // The force of knockback when taking damage

    public float moveSpeed = 1f;
    private int attack = 1; // Current HP of the player
    public Rigidbody2D rb;
    public bool isOnFloor = false; // Whether the player is standing on the floor or not
    private AudioSource audioSource;
    public AudioClip gateSound;
    public AudioClip landSound;
    public AudioClip panelSound;
    public AudioClip wallSound;
    public AudioClip heartSound;
    public AudioClip lightSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;

    public GameObject maria;

    public string doorName;

    public UIGameButton status = UIGameButton.none; // Whether the player is standing on the floor or not
    char exitPosition = '0';

    void Awake()
    {
        DataHolder.player = this.gameObject;
        rb = this.gameObject.AddComponent<Rigidbody2D>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
        int layerIndex = LayerMask.NameToLayer("Player"); // Get the layer index based on the layer name.
        this.gameObject.layer = layerIndex;
    }
    void Start()
    {
        int CurrentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        if(CurrentStage >= 0){


            List<string> myList = new List<string>();
            myList.Add("MariaGhost");
            GameObject prefabs = (GameObject)Utils.LoadPrefabs("Characters",myList)[0];
            maria = Utils.InstantiateObject(prefabs);
            maria.name = "MariaGhostMoveJump";
            maria.transform.SetParent(this.gameObject.transform);
            maria.transform.localPosition = new Vector2(-0.8f,0.8f); 
        }
        hpTextComponent = this.gameObject.AddComponent<HPTextComponent>();
        hpTextComponent.textColor = Color.white;
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        if(Camera.main == null) return;
        GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
        if(gameUIController == null) return;
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        string name = collision.gameObject.name;
        switch (name)
        {
            case string str when str.Contains("GoUp"):

                break;
            case string str when str.Contains("GoDown") ||  str.Contains("Story"):
                status = UIGameButton.none;
                gameUIController.HideGameButton();
                break;
            case string str when str.Contains("Floor"):
                break;
            case string str when str.Contains("Panel"):
                break;
            case string str when str.Contains("PlateV"):
                if(str.Contains("Gate")) {
                    status = UIGameButton.none;
                    gameUIController.HideGameButton();
                } 
                break;
            case string str when str.Contains("Enemy") || str.Contains("Boss") || str.Contains("Gem"):
                gameUIController.HideGameButton();
                gameUIController.HideEnemyButton();
                break;
        }
    }

    public void ShowStory(){
        this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Idle);
        StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        GameObject purifyObject = this.gameObject.GetComponent<JoystickController>().purifyObject;
        if(purifyObject != null && purifyObject.name.Contains("Trigger")) {
            storyController.LoadStoryDataFromJSON("T", purifyObject.name.Contains("Light") ? 0 : 1);
            return;
        }

        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        if( (CacheManager.IsCleared() && currentFloor == 2 && currentStage <= 1)){
            EventController.ShowEvent("shop");
        } else {
            storyController.LoadStoryDataFromJSON("", 0);
        }
        audioSource.PlayOneShot(wallSound);
        // VideoPlayerController videoController = Camera.main.gameObject.GetComponent<VideoPlayerController>();  
        // videoController.StartVideo("myvideo");
    }
    

    public void GoDown(){
        Debug.Log("???" + doorName);
        Debug.Log("???" + doorName);
        if (int.TryParse(doorName[1].ToString(), out int exitV))
        {
            if (int.TryParse(doorName[0].ToString(), out int exitH)){
                status = UIGameButton.none;
                CacheManager.SetCache(Cache.ExitH.ToString(), exitH >= 4 ? 0 : 4); 
                CacheManager.SetCache(Cache.ExitV.ToString(), exitV);
                int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
                MainController.LoadFloorAssetsFromJson(currentFloor + 1);
            }
        }
    }

    public void GoUp(){
        Debug.Log("???" + doorName);
        if (int.TryParse(doorName[1].ToString(), out int exitV))
        {
            if (int.TryParse(doorName[0].ToString(), out int exitH)){
                int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
                CacheManager.SetCache(Cache.ExitH.ToString(), exitH >= 4 ? 0 : 4); 
                CacheManager.SetCache(Cache.ExitV.ToString(), exitV);
                MainController.LoadFloorAssetsFromJson(currentFloor - 1);
            }
        }
    }

    public void GoExit() {
        Debug.Log("???" + doorName);
        GameObject purifyObject = this.gameObject.GetComponent<JoystickController>().purifyObject;
        if (int.TryParse(doorName[1].ToString(), out int exitV))
        {
            if (int.TryParse(doorName[0].ToString(), out int exitH)){
                status = UIGameButton.none;
                CacheManager.SetCache(Cache.ExitH.ToString(), exitH); 
                CacheManager.SetCache(Cache.ExitV.ToString(), exitV >= 4 ? 0 : 4);
                MainController.LoadFloorAssetsFromJson(Mathf.Abs(AssetData.exit));
            }
        }
    }
    public void GoFakeExit() {
        Debug.Log("???" + doorName);
        GameObject purifyObject = this.gameObject.GetComponent<JoystickController>().purifyObject;
        if (int.TryParse(doorName[1].ToString(), out int exitV))
        {
            if (int.TryParse(doorName[0].ToString(), out int exitH)){
                int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
                status = UIGameButton.none;
                CacheManager.SetCache(Cache.ExitH.ToString(), exitH); 
                CacheManager.SetCache(Cache.ExitV.ToString(), exitV >= 4 ? 0 : 4);
                MainController.LoadFloorAssetsFromJson(currentFloor >= 100 ? currentFloor / 100 : currentFloor * 100);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        string name = collision.gameObject.name;
        Vector3 direction = transform.position - collision.gameObject.transform.position;
        bool isRight = collision.gameObject.transform.localScale.x > 0;
        if(name.Contains("Enemy") || name.Contains("Boss") || name.Contains("Story")) {
            if (Mathf.Abs (direction.x) > Mathf.Abs (direction.y)) {
                if(isRight) {
                    if(direction.x>0){
                        print("collision is to the right" + name);
                        return;
                    }
                    else{
                        print("collision is to the left"  + name);
                    }
                } else {
                    if(direction.x>0){print("collision is to the right" + name);
                    }else{
                        print("collision is to the left"  + name);
                        return;
                    }
                }
            
            }else{
                if(direction.y>0){print("collision is up"  + name);}
                else{print("collision is down"  + name);}
                return;

            }
        }

        GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        string playerPosition = name.Split('_')[0];
        // Debug.Log(name);
        switch (name)
        {
            case string str when str.Contains("GoUp"):
                doorName = collision.gameObject.name;
                if(AssetData.goUp) {
                    GoDown();
                } else {
                    GoUp();  
                }
                break;
            case string str when str.Contains("GoDown"):
                doorName = collision.gameObject.name;
                exitPosition = playerPosition[1];
                gameUIController.showGameButton((int)UIGameButton.Down);
                status = UIGameButton.Down;
                break;
            case string str when str.Contains("GateGreen") || str.Contains("GateFake"):
                doorName = collision.gameObject.name;
                exitPosition = playerPosition[1];
                gameUIController.showGameButton((int)UIGameButton.Gate);
                status = str.Contains("GateGreen") ? UIGameButton.Exit : UIGameButton.FakeExit;
                audioSource.PlayOneShot(gateSound);
                break;
            case string str when str.Contains("Floor"):
                audioSource.PlayOneShot(landSound);
                break;
            case string str when str.Contains("Panel"):
                audioSource.PlayOneShot(panelSound);
                break;

            case string str when str.Contains("PlateV"):
                if(str.Contains("Gate")) {
                    gameUIController.showGameButton((int)UIGameButton.Gate);
                    status = UIGameButton.Gate;
                    audioSource.PlayOneShot(gateSound);
                } else {
                    audioSource.PlayOneShot(wallSound);
                }
                break;

            case string str when str.Contains("Light"):
                audioSource.PlayOneShot(lightSound);
                attack = attack + 1;
                Destroy(collision.gameObject);
                break;
                
            case string str when str.Contains("Story"):
                gameUIController.showGameButton((int)UIGameButton.Story);
                status = UIGameButton.Story;
                break;

            case string str when str.Contains("Enemy") || str.Contains("Boss") || str.Contains("Gem"):
                // InfoController infoController = Camera.main.gameObject.GetComponent<InfoController>();
                EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
                // infoController.avatars = enemyController.avatars;
                // infoController.enemyDataList = enemyController.enemyDatas;
                gameUIController.showGameButton((int)UIGameButton.Purify);
                gameUIController.ShowEnemyButton(enemyController.currentHP, enemyController.enemyMaxHP, enemyController.avatars[0]);
                status = UIGameButton.Purify;
                break;
        }
    }

    private void FullHp(Collider2D collision){
        int hpMax = CacheManager.GetCache(Cache.HPMax.ToString());
        CacheManager.ChangeCache(Cache.HP.ToString(), hpMax, hpMax);
        Camera.main.GetComponent<GameUIController>().DisplayHPMPInfo();
        audioSource.PlayOneShot(heartSound);
        Destroy(collision.gameObject);
    }

    private void FullMp(Collider2D collision){
        int mpMax = CacheManager.GetCache(Cache.MPMax.ToString());
        CacheManager.ChangeCache(Cache.MP.ToString(), mpMax, mpMax);
        Camera.main.GetComponent<GameUIController>().DisplayHPMPInfo();
        audioSource.PlayOneShot(heartSound);
        Destroy(collision.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.name)
        { 
            case string str when str.Contains("PotionRed"):
                FullHp(collision);
                FullMp(collision);
                break;
            case string str when str.Contains("PotionBlue"):
                FullMp(collision);
                break;
            case string str when str.Contains("PotionGreen"):
                FullHp(collision);
                break;
            case string str when str.Contains("Light") || str.Contains("Dark")
            || str.Contains("Teleport") || str.Contains("TeleportExit") :
                status = collision.name.Contains("Teleport") ? UIGameButton.Teleport : UIGameButton.Story;
                GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
                gameUIController.showGameButton((int)status);
                GameObject player = GameObject.Find("Player");
                player.GetComponent<JoystickController>().SetPurifyObject(collision.gameObject);
                break;
        }
    }

    public void AttackedBySpell(string name){
        Utils.AddEffect(this.gameObject, "Stone/" + name);
    }

    private void OnTriggerExit(Collider collision)
    {
        switch (collision.name)
        {
            case string str when str.Contains("Light") || str.Contains("Dark") 
            || str.Contains("Teleport") || str.Contains("TeleportExit") :
                GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
                gameUIController.HideGameButton();
                break;
        }
    }

}