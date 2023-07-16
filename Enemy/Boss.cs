using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int maxHealth = 3; // The maximum health of the dragon
    public int currentHealth; // The current health of the dragon
    public GameObject deathEffectPrefab; // The prefab for the death effect
    public GameObject fireballPrefab; // The prefab for the fireball
    public Transform mouthTransform; // The transform for the dragon's mouth
    public float moveSpeed = 5f; // The speed of the dragon's movement
    public float flySpeed = 10f; // The speed of the dragon's flying
    public float flyHeight = 3f; // The height of the dragon's flying
    public float flyDuration = 3f; // The duration of the dragon's flying
    public float fireballInterval = 1f; // The interval between fireball launches
    public int fireballDamage = 1; // The damage of the fireball
    public int touchDamage = 1; // The damage of the dragon's touch
    public AudioClip[] movementSounds; // An array of sound effects for the dragon's movement
    public AudioClip[] attackSounds; // An array of sound effects for the dragon's attacks
    public AudioClip deathSound; // The sound effect for the dragon's death
    private float timeSinceLastFireball = 0f; // The time since the last fireball launch
    private float flyTimer = 0f; // The timer for the dragon's flying
    private bool isFlying = false; // Whether the dragon is currently flying
    private bool isFacingRight = true; // Whether the dragon is currently facing right
    private float movementSoundTimer = 0f; // The timer for playing movement sounds
    private AudioSource audioSource; // The AudioSource component of the dragon
    private Rigidbody2D rb; // The Rigidbody2D component of the dragon

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check if the dragon should fly
        if (!isFlying)
        {
            flyTimer += Time.deltaTime;
            if (flyTimer >= flyDuration)
            {
                isFlying = true;
                flyTimer = 0f;
            }
        }

        // Move the dragon horizontally
        float moveDirection = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        // Play movement sounds
        if (Mathf.Abs(moveDirection) > 0f)
        {
            movementSoundTimer += Time.deltaTime;
            if (movementSoundTimer >= 0.3f)
            {
                PlayRandomSound(movementSounds);
                movementSoundTimer = 0f;
            }
        }

        // Flip the dragon if needed
        if ((moveDirection > 0f && !isFacingRight) || (moveDirection < 0f && isFacingRight))
        {
            Flip();
        }

        // Move the dragon vertically if flying
        if (isFlying)
        {
            float flyDirection = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, flyDirection * flySpeed);

            // Clamp the dragon's height
            float currentHeight = transform.position.y;
            float targetHeight = Mathf.Clamp(currentHeight, -flyHeight, flyHeight);
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

            // End the flying if the timer exceeds the duration
            flyTimer += Time.deltaTime;
            if (flyTimer >= flyDuration)
            {
                isFlying = false;
                flyTimer = 0f;
            }
        }

        // Check if it's time to launch a new fireball
        timeSinceLastFireball += Time.deltaTime;
        if (timeSinceLastFireball >= fireballInterval)
        {
            LaunchFireball();
            timeSinceLastFireball = 0f;
        }
    }

    void LaunchFireball()
    {
        // Instantiate a new fireball and set its velocity and position
        GameObject fireballObject = Instantiate(fireballPrefab, mouthTransform.position, mouthTransform.rotation);
        Rigidbody2D fireballRigidbody = fireballObject.GetComponent<Rigidbody2D>();
        fireballRigidbody.velocity = mouthTransform.right * flySpeed;

        // Set the damage of the fireball
        FireballController fireballController = fireballObject.GetComponent<FireballController>();
        fireballController.damage = fireballDamage;

        // Play attack sound
        PlayRandomSound(attackSounds);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.TakeDamage(touchDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Instantiate the death effect and destroy the dragon object
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        }

        // Play death sound
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        Destroy(gameObject);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void PlayRandomSound(AudioClip[] sounds)
    {
        if (sounds.Length > 0)
        {
            int index = Random.Range(0, sounds.Length);
            audioSource.PlayOneShot(sounds[index]);
        }
    }
}
