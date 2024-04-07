using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationStatus
{
    Idle = 0,
    Attack = 1,
    Move = 2,
    Action = 3,
    Die = 4
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
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
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
        // ReplaceSprite(status);
        animator.SetInteger("status", (int)status);
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