using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class JoystickController : MonoBehaviour
{
    public bool isFighting = false;
    [SerializeField] private float moveSpeed = 5f;
    // [SerializeField] private float jumpForce = 6.8f;
    [SerializeField] private float jumpForce = 7f;
    private Rigidbody2D rb;
    private float xBounds;
    public GameObject purifyObject;
    private int jumpCount = 0;
    private int jumpCap = 2;
    // [TBD]only foot can trigger

    private bool isJumpButtonPressed = false;
    private bool[] isUserDraggingArray = new bool[64];

    private bool isPlayerMoving = false;
    private int moveDirection = 0;

    private void FixedUpdate()
    {
        if (IsJumpButtonDown())
        {
            Jump();
        }
        if(isPlayerMoving) {
            MovePlayer(moveDirection);
        }
        if (Application.isMobilePlatform)
        {
            Touch touch = Input.GetTouch(0);


            if (touch.phase == TouchPhase.Began) {
                isPlayerMoving = true;

            } else if (touch.phase == TouchPhase.Moved)
            {
                // Start moving the player
                float touchDelta = touch.deltaPosition.x;
                if (touchDelta < 0)
                {
                    moveDirection = -1;
                }
                else if (touchDelta > 0)
                {
                    moveDirection = 1;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isPlayerMoving = false;
                this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Idle);
            }
        }
        else
        {
            // for edit mode
            float horizontalInput = Input.GetAxis("Horizontal");
            MovePlayer(horizontalInput);
        }
    }
    
    private bool IsJumpButtonDown()
    {
        if (Application.isMobilePlatform)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                bool isOverUIElement = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                if (isOverUIElement)
                {
                    // The touch is on a UI element, so we don't want to process it further
                    continue;
                }
                if (touch.phase == TouchPhase.Began && touch.deltaPosition.magnitude < 10f)
                {
                    isUserDraggingArray[i] = false;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    return !isUserDraggingArray[i];
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    isUserDraggingArray[i] = true;
                }
            }
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
        if(jumpCount >= jumpCap || isFighting == true){
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


    private void MovePlayer(float moveDirection)
    {
        if(isFighting){
            return;
        }
        if (moveDirection > 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(moveDirection < 0)
        {
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


    public void ReleaseFighting(){
        isFighting = false;
        this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Idle);
        Utils.showChildrenWithName("move", transform);
    }
    public void Crouch()
    {
    }

    public void Purify()
    {
        PlayerController playerController = this.gameObject.GetComponent<PlayerController>();
        if(playerController.status == UIGameButton.FakeExit){
            playerController.GoFakeExit();
            return;
        } else if(playerController.status == UIGameButton.Exit){
            playerController.GoExit();
            return;
        } else if(playerController.status == UIGameButton.Down){
            if(AssetData.goUp) {
                playerController.GoUp();
            } else {
                playerController.GoDown();  
            }
            return;
        } else if(playerController.status == UIGameButton.Story){
            playerController.ShowStory();
            return;
        } 
        if (purifyObject == null) return;
        if(playerController.status == UIGameButton.Gate){
            if(purifyObject.gameObject.name.Contains("GateExit")) {
            } else {
                Utils.showChildrenWithName("move", transform);
                purifyObject.GetComponent<GateController>().OpenGateAndDestroy(true);
                this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Action);
            }
        } else if(playerController.status == UIGameButton.Teleport){
            Debug.Log("purifyObject" + purifyObject.name);
            int teleport = purifyObject.name.Contains("TeleportExit") ? AssetData.teleportExit : AssetData.teleport;
            int index = purifyObject.name.Contains("TeleportExit") ? 1 : 2;
            CacheManager.SetCache(Cache.Teleport.ToString(), index);
            MainController.LoadFloorAssetsFromJson(teleport);
            return;
        } else {
            isFighting = true;
            Utils.showChildrenWithName("move", transform);
            this.gameObject.GetComponent<AnimatorController>().SetAnimationStatus(AnimationStatus.Attack);
            purifyObject.GetComponent<EnemyController>().DestroyEnemy();
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