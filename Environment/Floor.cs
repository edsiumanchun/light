using UnityEngine;

public class FloatingPlateController : MonoBehaviour
{
    public LayerMask playerLayer;
    public LayerMask passThroughLayers;

    private BoxCollider2D boxCollider;
    private PlatformEffector2D platformEffector;

    private void Start()
    {
        boxCollider = this.gameObject.AddComponent<BoxCollider2D>();
        boxCollider.usedByEffector = true;
        boxCollider.enabled = false;
        platformEffector = this.gameObject.AddComponent<PlatformEffector2D>();
        platformEffector.useColliderMask = true;
        platformEffector.colliderMask = LayerMask.GetMask("Player"); 
        // effector.rotationalOffset = 180; // Adjust the rotational offset as needed
    }

    private void Update()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            SetCollisionMask(passThroughLayers);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            SetCollisionMask(playerLayer);
        }
    }

    private void SetCollisionMask(LayerMask collisionMask)
    {
        platformEffector.colliderMask = collisionMask;
    }
}
