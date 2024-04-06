using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 3; // The maximum health of the player
    public float invincibilityTime = 1f; // The duration of invincibility after taking damage
    private bool isInvincible = false; // Whether the player is currently invincible
    private float invincibilityTimer = 0f; // The timer for invincibility
    public float knockbackForce = 3f; // The force of knockback when taking damage

    public float moveSpeed = 1f;
    public int maxHP = 3; // Maximum HP of the player
    private int currentHealth; // Current HP of the player
    public Rigidbody2D rb;
    public bool isOnFloor = false; // Whether the player is standing on the floor or not
    private AudioSource audioSource;
    public AudioClip gateSound;
    public AudioClip landSound;
    public AudioClip panelSound;
    public AudioClip wallSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;


    public UIGameButton status = UIGameButton.none; // Whether the player is standing on the floor or not
    char exitPosition = '0';

    void Awake()
    {
        DataHolder.player = this.gameObject;
        rb = this.gameObject.AddComponent<Rigidbody2D>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        currentHealth = maxHP;
        UpdateLifeBarUI();
        audioSource = GetComponent<AudioSource>();
        int layerIndex = LayerMask.NameToLayer("Player"); // Get the layer index based on the layer name.
        this.gameObject.layer = layerIndex;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
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
            case string str when str.Contains("Enemy"):
                gameUIController.hideGameButton();
                break;
        }
    }

    public void ShowStory(){
        StoryController storyController = Camera.main.gameObject.GetComponent<StoryController>();
        //if(storyController == null) storyController = Camera.main.gameObject.AddComponent<StoryController>();
        storyController.LoadStoryDataFromJSON();
        audioSource.PlayOneShot(wallSound);

    }

    public void GoDown(){
        status = UIGameButton.none;
        print(exitPosition);
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        CacheManager.SetStringCache(Cache.Exit.ToString(), "0" + exitPosition);
        MainController.LoadFloorAssetsFromJson(currentFloor - 1, "HomeScene");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameUIController gameUIController = Camera.main.GetComponent<GameUIController>();
        int currentFloor = CacheManager.GetCache(Cache.CurrentFloor.ToString());
        string name = collision.gameObject.name;
        string playerPosition = name.Split('_')[0];
        Debug.Log(name);
        switch (name)
        {
            case string str when str.Contains("GoUp"):
                CacheManager.SetStringCache(Cache.Exit.ToString(), "4" + playerPosition[1]);
                MainController.LoadFloorAssetsFromJson(currentFloor + 1, "HomeScene");
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

            case string str when str.Contains("Story"):

                gameUIController.showGameButton((int)UIGameButton.Story);
                status = UIGameButton.Story;
                break;

            case string str when str.Contains("Enemy"):

                // Deduct HP and play hurt sound when the player collides with an enemy
                currentHealth--;
                UpdateLifeBarUI();
                audioSource.PlayOneShot(hurtSound);

                if (currentHealth <= 0)
                {
                    // Play die sound
                    audioSource.PlayOneShot(dieSound);
                }
                else
                {
                    // Apply knockback force
                    
                    Vector2 forwardDirection = transform.forward;
                    Vector2 backwardDirection = -forwardDirection;
                    rb.AddForce(backwardDirection * knockbackForce, ForceMode2D.Impulse);

                    // Set invincibility
                    isInvincible = true;
                    invincibilityTimer = invincibilityTime;
                }
                gameUIController.showGameButton((int)UIGameButton.Purify);
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                isInvincible = true;
                invincibilityTimer = invincibilityTime;
                rb.velocity = Vector2.zero;
                rb.AddForce(transform.up * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    void Die()
    {

    }

    void UpdateLifeBarUI()
    {
        // Update the life bar UI based on currentHealth and maxHealth
    }
}