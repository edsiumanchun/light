using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlateController : MonoBehaviour
{
    public float plateSpeed = 2f;
    public float maxPlateHeight = 3f;
    public float minPlateHeight = 0.5f;

    private BoxCollider2D boxCollider;
    private bool isCrouching;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Move the floating plate up and down
        transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time * plateSpeed, maxPlateHeight - minPlateHeight) + minPlateHeight, transform.position.z);

        // Adjust the collider height if the player is crouching
        if (isCrouching)
        {
            boxCollider.size = new Vector2(boxCollider.size.x, 0.5f);
            boxCollider.offset = new Vector2(0f, -0.25f);
        }
        else
        {
            boxCollider.size = new Vector2(boxCollider.size.x, 1f);
            boxCollider.offset = new Vector2(0f, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player is crouching or jumping
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.isCrouching)
            {
                isCrouching = true;
            }
            else if (player.isJumping() && other.transform.position.y > transform.position.y + boxCollider.size.y / 2)
            {
                // The player is jumping and above the floating plate, so they can pass through
                Physics2D.IgnoreCollision(other, boxCollider, true);
            }
            else if (player.isJumping() && other.transform.position.y < transform.position.y && player.rb.velocity.y > 0)
            {
                // The player is jumping from below, so they can pass through
                Physics2D.IgnoreCollision(other, boxCollider, true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset the crouching flag when the player leaves the floating plate
        if (other.CompareTag("Player"))
        {
            isCrouching = false;
            Physics2D.IgnoreCollision(other, boxCollider, false);
        }
    }
}