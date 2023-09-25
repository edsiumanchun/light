using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 3; // The maximum health of the player
    public float invincibilityTime = 1f; // The duration of invincibility after taking damage
    private bool isInvincible = false; // Whether the player is currently invincible
    private float invincibilityTimer = 0f; // The timer for invincibility
    public float knockbackForce = 10f; // The force of knockback when taking damage

    public float moveSpeed = 1f;
    public int maxHP = 3; // Maximum HP of the player
    private int currentHealth; // Current HP of the player
    public Rigidbody2D rb;
    public bool isOnFloor = false; // Whether the player is standing on the floor or not
    private AudioSource audioSource;
    public AudioClip landSound;
    public AudioClip panelSound;
    public AudioClip wallSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;
    public JoystickController joystick; // The joystick controller

    void Awake()
    {
        DataHolder.player = this.gameObject;
        rb = this.gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        currentHealth = maxHP;
        UpdateLifeBarUI();
        audioSource = GetComponent<AudioSource>();
        int layerIndex = LayerMask.NameToLayer("Player"); // Get the layer index based on the layer name.
        this.gameObject.layer = layerIndex;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string name = collision.gameObject.name;
        switch (name)
        {
            case string str when str.Contains("Floor"):
                audioSource.PlayOneShot(landSound);
                break;

            case string str when str.Contains("Panel"):
                audioSource.PlayOneShot(panelSound);
                break;

            case string str when str.Contains("Wall"):
                audioSource.PlayOneShot(wallSound);
                break;

            case string str when str.Contains("Story"):
                //Camera.main.gameObject.GetComponent<StoryController>();
                //if(storyController == null) storyController = Camera.main.gameObject.AddComponent<StoryController>();
                //storyController.LoadStoryDataFromJSON(name);
                GameObject newObject = Instantiate(DataHolder.storyCanvas);
                audioSource.PlayOneShot(wallSound);
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
                    Vector2 direction = (transform.position - collision.transform.position).normalized;
                    rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

                    // Set invincibility
                    isInvincible = true;
                    invincibilityTimer = invincibilityTime;
                }
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