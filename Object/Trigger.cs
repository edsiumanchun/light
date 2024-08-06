using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public string name = "";
    public GameObject targetObject; // The object you want to affect when the trigger is activated

    void Start(){
        Rigidbody2D rb2D = gameObject.AddComponent<Rigidbody2D>();

        // Customize the Rigidbody 2D properties as needed
        rb2D.mass = 1f;
        rb2D.linearDamping = 0f;
        rb2D.angularDamping = 0.05f;
        rb2D.useAutoMass = false;
        rb2D.isKinematic = false;
        rb2D.freezeRotation = true;

        // Set the Rigidbody 2D to be unaffected by gravity
        rb2D.gravityScale = 0f;
    }    
}