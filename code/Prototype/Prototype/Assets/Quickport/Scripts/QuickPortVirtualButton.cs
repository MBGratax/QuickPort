using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class QuickPortVirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Output:")]
    public UnityEvent<bool> buttonStateEvent;
    public UnityEvent buttonClickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        produceButtonClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        produceButtonStateValue(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        produceButtonStateValue(false);
    }

    void produceButtonStateValue(bool buttonState)
    {
        buttonStateEvent.Invoke(buttonState);
    }

    void produceButtonClick()
    {
        buttonClickEvent.Invoke();
    }
}
