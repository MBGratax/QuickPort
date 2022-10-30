using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace QuickPortAPI
{
    [AddComponentMenu("QuickPort/Static Joystick")]

    /// <summary>
    /// A joystick control on UI level.
    /// Can be moved with touch or pointer input
    /// Is always returned to its anchored position.
    /// 
    /// The way this joystick is meant to be implemented is as the child of a Unity Image/Panel.
    /// That Unity Image/Panel serves as the background of the joystick.
    /// Additionally this nesting of the joystick allows for easy placement and scaling within any UI.
    /// 
    /// The joystick also requires a component of type button.
    /// </summary>
    public class QPStaticJoystick : QPDevice<Vector2>, IDragHandler
    {

        /// <summary>
        /// The settings of a quickport joystick.
        /// _joystickRange determines the length a knob can move from its original anchored position.
        /// _activated determines, whether a joystick is enabled on Start().
        /// </summary>
        [Header("Settings:")]
        [Tooltip("Determines how far the stick may move from the center of the joystick base.")]
        [FormerlySerializedAs("Joystick Range")]
        [SerializeField] private float _joystickRange = 50f;

        [Header("Components")]
        [Tooltip("The joysticks background. Usually a component of type image. This should be the object holding this script.")]
        [SerializeField] protected RectTransform _background;
        [Tooltip("The stick component. Usually an image with a button component. This should be the child of the current object.")]
        [SerializeField] RectTransform _handle;

        /// <summary>
        /// _startPosition is set by the script itself on Start()
        /// </summary>
        protected Vector2 _startPosition;

        /// <summary>
        /// The Canvas/UI Zone that contains the stick, important if you are using a dynamic stick and want to limit where it appears.
        /// </summary>
        protected RectTransform _baseRect = null;

        /// <summary>
        /// Public getter and setter for the _joystickRange.
        /// </summary>
        public float JoystickRange
        {
            get => _joystickRange;
            set => _joystickRange = value;
        }
        protected override Vector2 DefaultInput 
        { 
            get => new Vector2(0.0f,0.0f); 
        }

        protected override void Start()
        {
            _baseRect = GetComponent<RectTransform>();
            _startPosition = _handle.anchoredPosition;
            _handle.anchoredPosition = _startPosition;
        }

        /// <summary>
        /// Implements IPointerDownHandler interface.
        /// Checks whether the eventData exists and whether the joystick is activated.
        /// If both checks are passed, the event data is forwarded to OnDrag().
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!_bIsActive)
            {
                return;
            }
            else if (eventData == null)
            {
                System.ArgumentNullException exception = new System.ArgumentNullException(nameof(eventData));
                QuickPortLogger.QPLogException(this, exception);

            }
            else
            {
                OnDrag(eventData);
            }
        }

        /// <summary>
        /// Implements IDragHandler interface.
        /// Checks whether the eventData exists and whether the joystick is activated.
        /// If both checks are passed, the stick will be set to the position of the registered input, if that is within its range.
        /// Should the drag go outside of the range, the joystick will simply stop at the maximum of its range.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!_bIsActive)
            {
                return;
            }
            else if (eventData == null)
            {
                throw new System.ArgumentNullException(nameof(eventData));
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, eventData.pressEventCamera, out Vector2 _position);

                Vector2 _clampedPosition = Vector2.ClampMagnitude(_position, _joystickRange);
                _handle.anchoredPosition = _clampedPosition;

                //This brings the x and y values to something between -1 and 1. Which is the expected range in the event system.
                Vector2 _newPosition = new Vector2(_clampedPosition.x / _joystickRange, _clampedPosition.y / _joystickRange);
                SendInput(_newPosition);
            }
        }

        /// <summary>
        /// Implements IPointerUpHandler interface.
        /// Resets the position of the joystick.
        /// Sends a value of zero to the event system.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!_bIsActive)
            {
                return;
            }
            else
            {
                _handle.anchoredPosition = _startPosition;
                SendInput(DefaultInput);
            }
        }        
    }
}
