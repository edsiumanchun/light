using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform thumb; // The thumb object of the joystick
    public float sensitivity = 1f; // The sensitivity of the joystick movement
    public float maxRadius = 50f; // The maximum radius of the joystick movement

    private Vector2 startPos; // The starting position of the joystick
    private Vector2 inputVector; // The input vector of the joystick

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / maxRadius) * sensitivity;
            pos.y = 0f;
            inputVector = pos.normalized;
            thumb.anchoredPosition = new Vector2(inputVector.x * maxRadius, 0f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        thumb.anchoredPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        return inputVector.x;
    }
}
