using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickPortAPI
{
    /// <summary>
    /// The Dynamic stick is a Joystick that only appears if the player touches within the _baseRect of the Stick.
    /// </summary>
    public class QPDynamicJoyStick : QPStaticJoystick
    {
        /// <summary>
        /// Calls the start method of the Static stick and makes the stick inactive, waiting for the player to touch the _baseRect.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _background.gameObject.SetActive(false);
        }

        /// <summary>
        /// Puts the Stick to the touch position and makes it visible and usable, then handles input exactly as the StaticStick would.
        /// </summary>
        /// <param name="eventData">Touch Event data</param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, eventData.position, eventData.pressEventCamera, out Vector2 _position);
            _startPosition = _position;
            _background.anchoredPosition = _position;
            _background.gameObject.SetActive(true);
            base.OnPointerDown(eventData);
        }

        /// <summary>
        /// Makes the stick inactive again, then calls the StaticSticks method to handle releasing the stick.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            _background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
        }
    }
}
