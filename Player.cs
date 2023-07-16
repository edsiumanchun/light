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

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashDistance = 3f; // Distance to dash/flash
    public int maxHP = 3; // Maximum HP of the player
    private int currentHealth; // Current HP of the player
    private int jumpsLeft; // Number of jumps the player has left
    public Rigidbody2D rb;
    private float screenWidth;
    private bool isDead = false;
    public bool isCrouching = false;
    public bool isOnFloor = false; // Whether the player is standing on the floor or not
    private bool isOnPanel = false; // Whether the player is standing on a panel or not
    private bool isOnWall = false; // Whether the player is sliding on a wall or not
    private Vector2 wallNormal; // The normal of the wall that the player is sliding on
    private AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip panelSound;
    public AudioClip wallSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;
    private bool isFallingSlowly = false; // Whether the player is falling slowly or not
    public float fallMultiplier = 2.5f; // The multiplier for the gravity scale when falling quickly
    public float lowFallMultiplier = 2f; // The multiplier for the gravity scale when falling slowly
    public KeyCode slowFallButton;
    public JoystickController joystick; // The joystick controller

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        screenWidth = (float)Screen.width / Screen.height * Camera.main.orthographicSize * 2f;
        currentHealth = maxHP;
        jumpsLeft = 2; // Initialize the number of jumps to 2 (player can double jump)
        UpdateLifeBarUI();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isDead && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float horizontalInput = touch.position.x < Screen.width / 2 ? -1 : 1;

            // Move the player left/right
            if (!isOnWall)
            {
                rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            }
            else
            {
                // Slide the player down the wall
                rb.velocity = new Vector2(rb.velocity.x, -moveSpeed);
            }

            // Crouch if the player is on the ground and presses the crouch button
            if (touch.phase == TouchPhase.Began && rb.velocity.y == 0 && touch.position.y < Screen.height / 2)
            {
                isCrouching = true;
                transform.localScale = new Vector3(1, 0.5f, 1); // Scale down the player's sprite
            }

            // Stand up if the player releases the crouch button
            if (touch.phase == TouchPhase.Ended && isCrouching)
            {
                isCrouching = false;
                transform.localScale = Vector3.one; // Scale up the player's sprite
            }

            // Jump if the player is not crouching and taps the screen
            if (touch.phase == TouchPhase.Began && jumpsLeft > 0 && !isCrouching)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpsLeft--;
                audioSource.PlayOneShot(jumpSound);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                // Trigger the special skill when the player releases the touch
                TriggerSkill(touch.position);
            }

            // Check if the slow fall button is pressed
            if (Input.GetKeyDown(slowFallButton))
            {
                isFallingSlowly = true;
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        Vector3 playerPos = Camera.main.WorldToViewportPoint(transform.position);
        playerPos.x = Mathf.Clamp(playerPos.x, screenWidth / 2f, 1f - screenWidth / 2f);
        transform.position = Camera.main.ViewportToWorldPoint(playerPos);

        // Adjust the gravity scale based on the falling speed
        if (rb.velocity.y < 0)
        {
            if (isFallingSlowly)
            {
                rb.gravityScale = lowFallMultiplier;
            }
            else
            {
                rb.gravityScale = fallMultiplier;
            }
        }
        else
        {
            rb.gravityScale = 1f;
        }

        if (Input.GetKeyUp(slowFallButton))
        {
            isFallingSlowly = false;
        }
    }

    // Function to trigger the special skill
    void TriggerSkill(Vector2 touchPosition)
    {
        // Calculate the new position to dash/flash to
        Vector3 worldTouchPosition = Camera.main.ScreenToWorldPoint(touchPosition);
        float newX = Mathf.Clamp(worldTouchPosition.x, screenWidth / 2f, 1f - screenWidth / 2f);
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);

        // Move the player to the new position
        transform.position = newPosition + transform.right * dashDistance;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Floor"))
        {
            // Set isOnFloor to true if the player collides with the floor
            isOnFloor = true;
            jumpsLeft = 2;
            audioSource.PlayOneShot(landSound);
        }
        else if (collision.gameObject.CompareTag("Panel"))
        {
            // Set isOnPanel to true if the player collides with a panel
            isOnPanel = true;
            audioSource.PlayOneShot(panelSound);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Set isOnWall to true if the player collides with a wall
            isOnWall = true;
            wallNormal = collision.contacts[0].normal;
            audioSource.PlayOneShot(wallSound);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Deduct HP and play hurt sound when the player collides with an enemy
            currentHealth--;
            UpdateLifeBarUI();
            audioSource.PlayOneShot(hurtSound);

            if (currentHealth <= 0)
            {
                // Play die sound and set isDead to true when the player runs out of HP
                isDead = true;
                audioSource.PlayOneShot(dieSound);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            // Set isOnFloor to false when the player exits the floor
            isOnFloor = false;
        }
        else if (collision.gameObject.CompareTag("Panel"))
        {
            // Set isOnPanel to false when the player exits the panel
            isOnPanel = false;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Set isOnWall to false when the player exits the wall
            isOnWall = false;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Panel") && isOnPanel)
        {
            // Recharge the jumps when the player is standing on a panel and stays there
            jumpsLeft = 2;
        }
    }

    void UpdateLifeBarUI()
    {
        // Update the life bar UI to reflect the current HP of the player
        // ...
    }

    void Die()
    {

    }

    // new
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

    public bool isJumping(){
        return jumpsLeft != 0;
    }

    // joystick
    void FixedUpdate()
    {
        float hInput = joystick.Horizontal();
        Vector2 moveDir = new Vector2(hInput, 0f);
        rb.velocity = moveDir * moveSpeed;
    }
}