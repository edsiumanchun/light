using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler
{
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float jumpForce = 6.8f;
    private Rigidbody2D rb;
    private bool isJumping = false;
    private float xBounds;

    void Update()
    {
        // jump
        if (IsJumpButtonDown())
        {
            Jump();
        }
    }

    private bool IsJumpButtonDown()
    {
        if (Application.isMobilePlatform)
        {
            return false; 
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                return true;
            }
        }

        return false;
    }

    public void Jump()
    {
        if(isJumping || IsCollidingWithUpperObject()){
            return;
        }
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        isJumping = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // The player is touching the ground, so they can jump again
            isJumping = false;
        }
    }

    private bool IsCollidingWithUpperObject()
{
    // Get all the colliders involved in the collision
    ContactPoint2D[] contacts = new ContactPoint2D[10];
    int numContacts = rb.GetContacts(contacts);

    int groundCount = 0;

    for (int i = 0; i < numContacts; i++)
    {
        // Check if the contact point is with an upper object
        if (contacts[i].collider.CompareTag("Ground"))
        {
            groundCount++;
        }
    }
    return groundCount >= 4;
}
    // other than jump
    public Sprite GetBackgroundSprite()
    {
        return DataHolder.backgrounds[0];
    }

    public Sprite GetPlateSprite()
    {
        return DataHolder.plateV[0];
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //xBounds = GetBackgroundSprite().bounds.size.x - GetPlateSprite().bounds.size.x * 2;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition = transform.position + (Vector3)eventData.delta;

        float clampedX = Mathf.Clamp(newPosition.x, -xBounds, xBounds);
        float yPosition = transform.position.y;
        float zPosition = transform.position.z;

        transform.position = new Vector3(clampedX, yPosition, zPosition);
    }

    private void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float touchDelta = touch.deltaPosition.x;

                if (touchDelta < 0)
                {
                    MovePlayer(-1);
                }
                else if (touchDelta > 0)
                {
                    MovePlayer(1);
                }
                else
                {
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
            }
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            MovePlayer(horizontalInput);
        }
    }

    private void MovePlayer(float moveDirection)
    {
        transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);
    }


    public void Crouch()
    {
    }

    public void Purify()
    {
        GameObject[] objectsToPurify = GameObject.FindGameObjectsWithTag("Purify");
        print("purify");
        foreach (GameObject obj in objectsToPurify)
        {
            print("purify");
            obj.GetComponent<CacheObjectController>().PurifyCacheData();
            Destroy(obj);
        }
    }
}