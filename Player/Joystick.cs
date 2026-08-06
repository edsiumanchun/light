using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class JoystickController : MonoBehaviour
{
    public bool isFighting = false;
    [SerializeField] private float moveSpeed = 5f;
    // [SerializeField] private float jumpForce = 6.8f;
    [SerializeField] private float jumpForce = 7f;
    private Rigidbody2D rb;
    private AnimatorController animatorController;
    private PlayerController playerController;
    public GameObject purifyObject;
    private int jumpCount = 0;
    private int jumpCap = 2;
    // [TBD]only foot can trigger

    private readonly HashSet<int> draggingFingers = new HashSet<int>();

    private bool isPlayerMoving = false;
    private int moveDirection = 0;

    private void FixedUpdate()
    {
        if (IsJumpButtonDown()) Jump();
        if (isPlayerMoving) MovePlayer(moveDirection);
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);


            if (touch.phase == TouchPhase.Began)
                isPlayerMoving = true;
            else if (touch.phase == TouchPhase.Moved)
            {
                moveDirection = (int)Mathf.Sign(touch.deltaPosition.x);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isPlayerMoving = false;
                animatorController.SetAnimationStatus(AnimationStatus.Idle);
            }
        }
        else
        {
            // for edit mode
            MovePlayer(Input.GetAxis("Horizontal"));
        }
    }
    
    private bool IsJumpButtonDown()
    {
        if (Application.isMobilePlatform)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                bool isOverUIElement = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                if (isOverUIElement)
                {
                    // The touch is on a UI element, so we don't want to process it further
                    continue;
                }
                if (touch.phase == TouchPhase.Began && touch.deltaPosition.magnitude < 10f)
                {
                    draggingFingers.Remove(touch.fingerId);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    bool wasDragging = draggingFingers.Remove(touch.fingerId);
                    return !wasDragging;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    draggingFingers.Add(touch.fingerId);
                }
            }
        }

        else
        {
            return Input.GetButtonDown("Jump");
        }

        return false;
    }

    public void Jump()
    {
        if (jumpCount >= jumpCap || isFighting) return;
        animatorController.SetAnimationStatus(AnimationStatus.Jump);
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
            animatorController.SetAnimationStatus(AnimationStatus.Idle);
            Utils.showChildrenWithName("move", transform);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<AnimatorController>();
        playerController = GetComponent<PlayerController>();
    }


    private void MovePlayer(float moveDirection)
    {
        if (isFighting) return;
        if (moveDirection > 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(moveDirection < 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            if(jumpCount == 0){
                if (animatorController.animator.GetInteger("status") == (int)AnimationStatus.Action) return;
                animatorController.SetAnimationStatus(AnimationStatus.Idle);
                Utils.showChildrenWithName("move", transform);
            }
            return;
        }
        transform.Translate(Vector3.right * moveDirection * moveSpeed * Time.deltaTime);
        if(jumpCount == 0){
            animatorController.SetAnimationStatus(AnimationStatus.Move);
            Utils.showChildrenWithName("move", transform);
        }
    }


    public void ReleaseFighting(){
        isFighting = false;
        animatorController.SetAnimationStatus(AnimationStatus.Idle);
        Utils.showChildrenWithName("move", transform);
    }
    public void Crouch()
    {
    }

    public void Purify()
    {
        switch (playerController.status)
        {
            case UIGameButton.FakeExit: playerController.GoFakeExit(); return;
            case UIGameButton.Exit: playerController.GoExit(); return;
            case UIGameButton.Down:
                if (AssetData.goUp) playerController.GoUp(); else playerController.GoDown();
                return;
            case UIGameButton.Story: playerController.ShowStory(); return;
        } 
        if (purifyObject == null) return;
        if(playerController.status == UIGameButton.Gate){
            if(purifyObject.gameObject.name.Contains("GateExit")) {
            } else {
                Utils.showChildrenWithName("move", transform);
                GateController gate = purifyObject.GetComponent<GateController>();
                if (gate == null) return;
                gate.OpenGateAndDestroy(true);
                animatorController.SetAnimationStatus(AnimationStatus.Action);
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
            animatorController.SetAnimationStatus(AnimationStatus.Attack);
            EnemyController enemy = purifyObject.GetComponent<EnemyController>();
            if (enemy == null)
            {
                isFighting = false;
                return;
            }
            enemy.DestroyEnemy();
        }
    }


    public void SetPurifyObject(GameObject gameObject)
    {
        purifyObject = gameObject;
    }
}
