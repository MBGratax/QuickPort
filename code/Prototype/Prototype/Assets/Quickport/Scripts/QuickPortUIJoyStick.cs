using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class QuickPortUIJoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("RectReferences:")]
    public RectTransform knobRect;
    public RectTransform panelRect;

    [Header("Settings:")]
    public float joystickRange = 50f;

    [Header("Output:")]
    public UnityEvent<Vector2> joystickOutputEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Touch registered");
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
        //position = ApplySizeDelta(position);
        Vector2 clampedPosition = Vector2.ClampMagnitude(position, 1f);

        ProduceOutput(position);
        if (knobRect)
        {
            UpdateKnobPosition(clampedPosition * joystickRange);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Touch Start");
        ProduceOutput(Vector2.zero);

        if (knobRect)
        {
            UpdateKnobPosition(Vector2.zero);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Init");
        if (knobRect)
        {
            UpdateKnobPosition(Vector2.zero);
        }
    }

    void UpdateKnobPosition(Vector2 position)
    {
        knobRect.anchoredPosition = position;
    }
    
    void ProduceOutput(Vector2 currentPosition)
    {
        joystickOutputEvent.Invoke(currentPosition);
    }
}
