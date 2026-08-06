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
    private AnimatorController animatorController;
    private JoystickController joystickController;
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
    void Awake()
    {
        DataHolder.player = this.gameObject;
        rb = this.gameObject.AddComponent<Rigidbody2D>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
        animatorController = GetComponent<AnimatorController>();
        int layerIndex = LayerMask.NameToLayer("Player"); // Get the layer index based on the layer name.
        this.gameObject.layer = layerIndex;
    }
    void Start()
    {
        joystickController = GetComponent<JoystickController>();
        int CurrentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        if(CurrentStage >= 0){


            GameObject prefabs = (GameObject)Utils.LoadPrefabs("Characters", new List<string> { "MariaGhost" })[0];
            maria = Utils.InstantiateObject(prefabs);
            if (maria != null)
            {
                maria.name = "MariaGhostMoveJump";
                maria.transform.SetParent(transform);
                maria.transform.localPosition = new Vector2(-0.8f, 0.8f);
            }
        }
        hpTextComponent = this.gameObject.AddComponent<HPTextComponent>();
        hpTextComponent.textColor = Color.white;
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        if(Camera.main == null) return;
        GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
        if(gameUIController == null) return;
        string name = collision.gameObject.name;
        if (ContainsAny(name, "GoDown", "Story", "Gate"))
        {
            status = UIGameButton.none;
            gameUIController.HideGameButton();
        }
        if (ContainsAny(name, "Enemy", "Boss", "Gem"))
        {
            gameUIController.HideGameButton();
            gameUIController.HideEnemyButton();
        }
    }

    public void ShowStory(){
        animatorController.SetAnimationStatus(AnimationStatus.Idle);
        StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        GameObject purifyObject = joystickController.purifyObject;
        if(purifyObject != null && purifyObject.name.Contains("Trigger")) {
            storyController.LoadStoryDataFromJSON("T", true, purifyObject.name.Contains("Light") ? 0 : 1);
            return;
        }

        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        if( (CacheManager.IsCleared() && currentFloor == 2 && currentStage <= 1)){
            EventController.ShowEvent("shop");
        } else {
            storyController.LoadStoryDataFromJSON("", false, 0);
        }
        audioSource.PlayOneShot(wallSound);
        // VideoPlayerController videoController = Camera.main.gameObject.GetComponent<VideoPlayerController>();  
        // videoController.StartVideo("myvideo");
    }
    

    public void GoDown() => MoveBetweenFloors(1);
    public void GoUp() => MoveBetweenFloors(-1);
    public void GoExit() => MoveThroughExit(false);
    public void GoFakeExit() => MoveThroughExit(true);

    private bool TryGetDoorCoordinates(out int exitH, out int exitV)
    {
        exitH = exitV = 0;
        return !string.IsNullOrEmpty(doorName) && doorName.Length >= 2
            && int.TryParse(doorName[0].ToString(), out exitH)
            && int.TryParse(doorName[1].ToString(), out exitV);
    }

    private void MoveBetweenFloors(int direction)
    {
        if (!TryGetDoorCoordinates(out int exitH, out int exitV)) return;
        status = UIGameButton.none;
        CacheManager.SetCache(Cache.ExitH.ToString(), exitH >= 4 ? 0 : 4);
        CacheManager.SetCache(Cache.ExitV.ToString(), exitV);
        int offset = doorName.Contains("Revert") ? -direction : direction;
        int target = doorName.Contains("Exit")
            ? Mathf.Abs(AssetData.exit)
            : CacheManager.GetCache(Cache.CurrentFloor.ToString()) + offset;
        MainController.LoadFloorAssetsFromJson(target);
    }

    private void MoveThroughExit(bool fake)
    {
        if (!TryGetDoorCoordinates(out int exitH, out int exitV)) return;
        status = UIGameButton.none;
        CacheManager.SetCache(Cache.ExitH.ToString(), exitH);
        CacheManager.SetCache(Cache.ExitV.ToString(), exitV >= 4 ? 0 : 4);
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int target = fake
            ? (currentFloor >= 100 ? currentFloor / 100 : currentFloor * 100)
            : Mathf.Abs(doorName.Contains("Exit2") ? AssetData.exit2 : AssetData.exit);
        MainController.LoadFloorAssetsFromJson(target);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        string name = collision.gameObject.name;
        Vector3 direction = transform.position - collision.gameObject.transform.position;
        if (ContainsAny(name, "Enemy", "Boss", "Story") &&
            (Mathf.Abs(direction.y) >= Mathf.Abs(direction.x) ||
             (collision.transform.localScale.x > 0) == (direction.x > 0))) return;

        GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
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
                gameUIController.showGameButton((int)UIGameButton.Down);
                status = UIGameButton.Down;
                break;
            case string str when str.Contains("GateExit") || str.Contains("GateFake"):
                doorName = collision.gameObject.name;
                gameUIController.showGameButton((int)UIGameButton.Gate);
                status = str.Contains("GateExit") ? UIGameButton.Exit : UIGameButton.FakeExit;
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
                joystickController.SetPurifyObject(collision.gameObject);
                break;
        }
    }

    public void AttackedBySpell(string name){
        Effects.AddEffect(this.gameObject, "Stone/" + name);
    }

    private static bool ContainsAny(string value, params string[] terms)
    {
        foreach (string term in terms)
            if (value.Contains(term)) return true;
        return false;
    }

    private void OnTriggerExit2D(Collider2D collision)
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
