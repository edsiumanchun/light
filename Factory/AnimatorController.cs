using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationStatus
{
    Idle = 0,
    Attack = 1,
    Move = 2,
    Action = 3,
    Die = 4,
    Jump = 5,
    Point = 6,
    Kneel = 7,
    Wake = 8,
    Down = 9,
    Hurt = 10,
    Cry = 11
}

// public enum ComponentType
// {
//     Head,
//     Dress,
//     LHand,
//     RHand,
//     LTool,
//     RTool,
//     LFoot,
//     RFoot
// }


public class AnimatorController : MonoBehaviour
{
    // Dictionary<AnimationStatus, List<Sprite>> animationSprites = new Dictionary<AnimationStatus, List<Sprite>>();
    public Animator animator;
    private CharacterVisual3D visual3D;
    public bool FacingRight { get; private set; }

    private void Awake()
    {
        visual3D = CharacterVisual3D.FindOrCreate(gameObject);
        animator = visual3D != null && visual3D.ModelAnimator != null
            ? visual3D.ModelAnimator
            : GetComponent<Animator>();
        FacingRight = transform.localScale.x < 0f;
        
        // animationSprites.Add(AnimationStatus.Idle, attackSprites);
        
        // List<string> attackPaths = new List<string>((new[] { "Head", "Dress", "LHand", "RHand", "LTool", "RTool", "LFoot", "RFoot"}));
        // List<Sprite> attackSprites = LoadSprites("PrefabAnimation/" + this.gameObject.name, attackPaths);
        // animationSprites.Add(AnimationStatus.Attack, attackSprites);
    
        // List<string> movePaths = new List<string>((new[] { "Head", "Dress", "LHand", "RHand", "LTool", "RTool", "LFoot", "RFoot"}));
        // List<Sprite> moveSprites = LoadSprites("PrefabAnimation/" + this.gameObject.name, movePaths);
        // animationSprites.Add(AnimationStatus.Move, moveSprites);
    }

    public void SetAnimationStatus(AnimationStatus status)
    {
        // ReplacePrefab(status);
        if (visual3D != null && visual3D.IsConfigured)
            visual3D.SetAnimation(status);
        else if (HasStatusParameter())
            animator.SetInteger("status", (int)status);
    }

    public void SetFacing(bool faceRight)
    {
        FacingRight = faceRight;
        if (visual3D != null && visual3D.IsConfigured)
        {
            visual3D.SetFacing(faceRight);
            return;
        }
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (faceRight ? -1f : 1f);
        transform.localScale = scale;
    }

    public void TurnAround() => SetFacing(!FacingRight);

    public bool IsInStatus(AnimationStatus status)
        => HasStatusParameter() && animator.GetInteger("status") == (int)status;

    private bool HasStatusParameter()
    {
        if (animator == null) return false;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (parameter.name == "status" && parameter.type == AnimatorControllerParameterType.Int)
                return true;
        return false;
    }

    public static List<Sprite> LoadSprites(string folder, List<string> paths)
    {
        return Utils.LoadSprites(folder, paths);
    }

    // Sprite GetSpriteForComponent(ComponentType componentType, AnimationStatus status)
    // {
    //     List<Sprite> spriteList = animationSprites[status];
    //     int index = (int)componentType;
    //     if (index >= 0 && index < spriteList.Count)
    //     {
    //         return spriteList[index];
    //     }
    //     else
    //     {
    //         return null; // or any default sprite
    //     }
    // }

    // void ReplacePrefab(){
    //     GameObject prefab = (GameObject)DataHolder.characters[formattedIndex];
    //     newObject = Utils.InstantiateObject(prefab);
    //     prefab.AddComponent<AnimatorController>();
    //     // Assuming you have a reference to the original GameObject and the new prefab
    //     GameObject originalObject;
    //     GameObject newPrefab;

    //     // Step 1: Get the position and other necessary properties of the original GameObject
    //     Vector3 position = originalObject.transform.position;
    //     Quaternion rotation = originalObject.transform.rotation;
    //     Vector3 scale = originalObject.transform.localScale;
    //     Transform parentTransform = originalObject.transform.parent;

    //     // Step 2: Instantiate the new prefab at the same position
    //     GameObject newObject = Instantiate(newPrefab, position, rotation, parentTransform);

    //     // Step 3: Transfer properties from the original GameObject to the instantiated prefab
    //     newObject.transform.localScale = scale;

    //     // Step 4: Destroy the original GameObject
    //     Destroy(originalObject);
    // }

    // void ReplaceSprite(AnimationStatus status){
    //     foreach (Transform child in this.gameObject.transform)
    //     {
    //         // Get the component name from the child's GameObject
    //         string componentName = child.gameObject.name;
    //         Debug.Log(componentName);
    //         // Convert the component name string to an enum value
    //         if (ComponentType.TryParse(componentName, out ComponentType componentType))
    //         {
    //         //     // Replace the corresponding index in the sprite list
            
    //             Sprite sprite = GetSpriteForComponent(componentType, status);
    //             if(sprite != null) {
    //                 child.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    //             }
    //         }
    //     }
    // }
}
