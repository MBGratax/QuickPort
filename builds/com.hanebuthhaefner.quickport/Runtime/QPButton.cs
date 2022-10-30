using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickPortAPI
{
    [AddComponentMenu("QuickPort/Button")]
    /// <summary>
    /// A button on UI level.
    /// Can be pressed using a touch or pointer input.
    /// Is based upon the Unity Button, but with additional functionality.
    /// </summary>
    public class QPButton : QPDevice<float>
    {

        /// <summary>
        /// Settigns for the QPButton.
        /// _activated determines whether the button is activated on Start().
        /// </summary>
        [Header("Settings:")]
        [SerializeField]
        bool _bIsActivated = true;

        protected override float DefaultInput => 0.0f;

        /// <summary>
        /// Implements IPointerDownHandler Interface.
        /// Sends a "pressed" to control, if the button is activated.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (_bIsActivated)
            {
                SendInput(1.0f);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Implements IPointerUpHandler Interface.
        /// Sends a "not pressed" to control, if the button is activated.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (_bIsActivated)
            {
                SendInput(0.0f);
            }
            else
            {
                return;
            }
        }
    }
}
