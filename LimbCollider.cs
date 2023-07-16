using UnityEngine;

public class GenerateBoxColliders : MonoBehaviour
{
    void Start()
    {
        // Loop through all child objects
        foreach (Transform child in transform)
        {
            // Add a box collider to each child object
            BoxCollider2D collider = child.gameObject.AddComponent<BoxCollider2D>();

            // Set the size of the box collider to match the sprite size
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                collider.size = spriteRenderer.sprite.bounds.size;
            }
        }
    }
}