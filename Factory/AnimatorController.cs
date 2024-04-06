using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationStatus
{
    Idle = 0,
    Attack = 1,
    Move = 2
}

public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimationStatus(AnimationStatus status)
    {
        animator.SetInteger("status", (int)status);
    }
}