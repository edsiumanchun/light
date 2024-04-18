using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler
{
    [SerializeField] private float moveSpeed = 6;
    // [SerializeField] private float jumpForce = 6.8f;
    [SerializeField] private float jumpForce = 7f;
    private Rigidbody2D rb;
    private float xBounds;
    public GameObject purifyObject;
    private int jumpCount = 0;
    private int jumpCap = 2;
    // [TBD]only foot can trigger
    public float stopMove = 0;
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
        if(jumpCount >= jumpCap){
            return;
        }
        this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Jump);
        Utils.showChildrenWithName("jump", transform);
        
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Vector2 collisionNormal = collision.contacts[0].normal;
        if (collisionNormal == Vector2.up){
            // The player is touching the ground, so they can jump again
            jumpCount = 0;
            print("joystickenter2d");
            this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Idle);
            Utils.showChildrenWithName("move", transform);
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

    public Sprite GetPlateSprite()
    {
        return DataHolder.plateV[0];
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

    public void SetupStopMove(){
        stopMove = transform.localScale.x;
    }

    private void MovePlayer(float moveDirection)
    {
        if (moveDirection > 0)
        {
            if(stopMove < 0){
                return;
            }
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(moveDirection < 0)
        {
            if(stopMove > 0){
                return;
            }
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            if(jumpCount == 0){
                if(this.gameObject.GetComponent<AnimatorController>().animator.GetInteger("status") == (int)AnimationStatus.Action) {
                    return;
                }
                this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Idle);
                Utils.showChildrenWithName("move", transform);
            }
            return;
        }
        transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);
        if(jumpCount == 0){
            this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Move);
            Utils.showChildrenWithName("move", transform);
        }
    }


    public void Crouch()
    {
    }

    public void Purify()
    {
        PlayerController playerController = this.gameObject.GetComponent<PlayerController>();
        if(playerController.status == UIGameButton.Down){
            playerController.GoDown();
            return;
        } else if(playerController.status == UIGameButton.Story){
            playerController.ShowStory();
            return;
        }

        if (purifyObject == null) return;
        print(purifyObject);
        if(playerController.status == UIGameButton.Gate){
            purifyObject.GetComponent<GateController>().OpenGateAndDestroy(true);
            this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Action);
            Utils.showChildrenWithName("move", transform);
        } else  {
            purifyObject.GetComponent<EnemyController>().DestroyEnemy();
            SetPurifyObject(null);
        }
    }

    public void SetPurifyObject(GameObject gameObject)
    {
        this.purifyObject = gameObject;
        if (gameObject == null)
        {
            // hide button
        } else
        {
            // show button
        }

    }
}