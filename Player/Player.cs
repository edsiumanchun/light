using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public HPTextComponent hpTextComponent;
    public float knockbackForce = 3f; // The force of knockback when taking damage

    public float moveSpeed = 1f;
    private int currentHealth = 10; // Current HP of the player
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

    public UIGameButton status = UIGameButton.none; // Whether the player is standing on the floor or not
    char exitPosition = '0';

    void Awake()
    {
        DataHolder.player = this.gameObject;
        rb = this.gameObject.AddComponent<Rigidbody2D>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        UpdateLifeBarUI();
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
                gameUIController.hideGameButton();
                break;
            case string str when str.Contains("Floor"):
                break;
            case string str when str.Contains("Panel"):
                break;
            case string str when str.Contains("PlateV"):
                if(str.Contains("Gate")) {
                    status = UIGameButton.none;
                    gameUIController.hideGameButton();
                } 
                this.gameObject.GetComponent<JoystickController>().stopMove = 0;
                break;
            case string str when str.Contains("Enemy") || str.Contains("Boss"):
                gameUIController.hideGameButton();
                break;
        }
    }

    public void ShowStory(){
        StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        if( (CacheManager.IsCleared() && currentFloor == 2) ){
            EventController.ShowEvent("shop");
        } else {
            storyController.LoadStoryDataFromJSON();
        }
        audioSource.PlayOneShot(wallSound);
        // VideoPlayerController videoController = Camera.main.gameObject.GetComponent<VideoPlayerController>();  
        // videoController.StartVideo("myvideo");
    }
    

    public void GoDown(){
        status = UIGameButton.none;
        CacheManager.SetCache(Cache.Exit.ToString(), 2);
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        int offset = currentStage == 0 || currentStage == 1 ? -1 : 1; 
        MainController.LoadFloorAssetsFromJson(currentFloor - offset, "HomeScene");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
        int currentStage = CacheManager.GetCache(Cache.CurrentStage.ToString());
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        int offset = currentStage == 0 || currentStage == 1 ? -1 : 1; 
        string name = collision.gameObject.name;
        string playerPosition = name.Split('_')[0];
        Debug.Log(name);
        switch (name)
        {
            case string str when str.Contains("GoUp"):
                CacheManager.SetCache(Cache.Exit.ToString(), 0);
                MainController.LoadFloorAssetsFromJson(currentFloor + offset, "HomeScene");
                break;
            case string str when str.Contains("GoDown"):
                exitPosition = playerPosition[1];
                gameUIController.showGameButton((int)UIGameButton.Down);
                status = UIGameButton.Down;
                break;
            case string str when str.Contains("Floor"):
                audioSource.PlayOneShot(landSound);
                break;
            case string str when str.Contains("Panel"):
                audioSource.PlayOneShot(panelSound);
                break;

            case string str when str.Contains("PlateV"):
                if(str.Contains("Gate")) {
                    status = UIGameButton.Gate;
                    gameUIController.showGameButton((int)UIGameButton.Gate);
                    audioSource.PlayOneShot(gateSound);
                } else {
                    audioSource.PlayOneShot(wallSound);
                }
                this.gameObject.GetComponent<JoystickController>().SetupStopMove();
                break;

            case string str when str.Contains("Heart"):
                audioSource.PlayOneShot(heartSound);
                currentHealth = currentHealth + 10;
                Destroy(collision.gameObject);
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

            case string str when str.Contains("Enemy") || str.Contains("Boss"):
                gameUIController.showGameButton((int)UIGameButton.Purify);
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        // Deduct HP and play hurt sound when the player collides with an enemy
        int trial = currentHealth - damage;
        audioSource.PlayOneShot(hurtSound);

        if (trial <= 0)
        {
            // Play die sound
            audioSource.PlayOneShot(dieSound);
            Vector2 forwardDirection = transform.forward;
            Vector2 backwardDirection = -forwardDirection;
            rb.AddForce(backwardDirection * knockbackForce, ForceMode2D.Impulse);
        }
        else
        {
            // Apply knockback force
            UpdateLifeBarUI();
            currentHealth = trial;
            
            Vector2 forwardDirection = transform.forward;
            Vector2 backwardDirection = -forwardDirection;
            rb.AddForce(backwardDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    void AboutToDie()
    {

    }

    void UpdateLifeBarUI()
    {
        // Update the life bar UI based on currentHealth and maxHealth
    }

    public void SetupHPText(){
    }
}