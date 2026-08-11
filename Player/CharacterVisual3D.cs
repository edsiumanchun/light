using UnityEngine;

/// <summary>
/// Adapts an optional 3D model child to the existing 2D gameplay root.
/// Rigidbody2D, Collider2D and gameplay scripts stay on the parent object.
/// </summary>
public class CharacterVisual3D : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private float rightFacingY = 90f;
    [SerializeField] private float leftFacingY = -90f;
    [SerializeField] private float depthOffset;
    [SerializeField] private bool hideSpriteVisuals = true;

    public bool IsConfigured => modelRoot != null;
    public Animator ModelAnimator => modelAnimator;

    private void Awake()
    {
        FindModelIfNeeded();
        if (modelRoot != null)
        {
            Vector3 position = modelRoot.localPosition;
            position.z = depthOffset;
            modelRoot.localPosition = position;
            if (hideSpriteVisuals)
                foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>(true))
                    sprite.enabled = false;
        }
    }

    public static CharacterVisual3D FindOrCreate(GameObject owner)
    {
        CharacterVisual3D visual = owner.GetComponent<CharacterVisual3D>();
        if (visual == null && HasMeshVisual(owner)) visual = owner.AddComponent<CharacterVisual3D>();
        return visual;
    }

    public void SetFacing(bool faceRight)
    {
        if (!IsConfigured) return;
        Vector3 angles = modelRoot.localEulerAngles;
        angles.y = faceRight ? rightFacingY : leftFacingY;
        modelRoot.localEulerAngles = angles;
    }

    public void SetAnimation(AnimationStatus status)
    {
        if (HasStatusParameter(modelAnimator)) modelAnimator.SetInteger("status", (int)status);
    }

    private void FindModelIfNeeded()
    {
        if (modelRoot == null)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                {
                    modelRoot = renderer.transform;
                    while (modelRoot.parent != null && modelRoot.parent != transform)
                        modelRoot = modelRoot.parent;
                    break;
                }
            }
        }
        if (modelAnimator == null && modelRoot != null)
            modelAnimator = modelRoot.GetComponentInChildren<Animator>() ?? modelRoot.GetComponent<Animator>();
        if (modelAnimator != null) modelAnimator.applyRootMotion = false;
    }

    private static bool HasMeshVisual(GameObject owner)
    {
        Renderer[] renderers = owner.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
            if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer) return true;
        return false;
    }

    private static bool HasStatusParameter(Animator target)
    {
        if (target == null) return false;
        foreach (AnimatorControllerParameter parameter in target.parameters)
            if (parameter.name == "status" && parameter.type == AnimatorControllerParameterType.Int)
                return true;
        return false;
    }
}
