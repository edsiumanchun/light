using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 5.7f;
    Rigidbody2D rb;
    private float xBounds;

    public Sprite GetBackgroundSprite()
    {
        return DataHolder.loadedAssets[0].tempAssets[0] as Sprite;
    }

    public Sprite GetPlateSprite()
    {
        return DataHolder.loadedAssets[1].tempAssets[0] as Sprite;
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

    void Update()
    {
        // jump
        if (IsJumpButtonDown())
        {
            Jump();
        }
    }

    private void MovePlayer(float moveDirection)
    {
        transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        // transform.Translate(Vector3.up * jumpForce * Time.deltaTime);
        // Implement your jump logic here
        // Apply jumpForce to the player's rigidbody or perform any other desired action
    }

    private bool IsJumpButtonDown()
    {
        // Check if the jump button is pressed for mobile or desktop
        // Modify this method according to your input setup
        if (Application.isMobilePlatform)
        {
            // Check for mobile touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    return true;
                }
            }
        }
        else
        {
            // Check for desktop input
            if (Input.GetButtonDown("Jump"))
            {
                return true;
            }
        }

        return false;
    }
}