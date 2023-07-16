using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string skillName; // The name of the skill that this button triggers
    public float cooldown = 1f; // The cooldown time for the skill

    private bool isPressed; // Whether the button is currently pressed
    private float timer; // The cooldown timer for the skill

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        timer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    void Update()
    {
        if (isPressed)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                // Trigger the skill
                Debug.Log("Triggering skill: " + skillName);
                timer = 0f;
            }
        }
    }
}