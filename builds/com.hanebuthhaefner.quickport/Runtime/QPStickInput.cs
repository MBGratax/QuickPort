using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if QUICKPORT_INPUTSYSTEM_ENABLED
using UnityEngine.InputSystem.Layouts;
#endif

namespace QuickPortAPI
{
    public class QPStickInput : QPInput<Vector2>
    {
        [Header("Settings")]
        [Tooltip("The Deadzone for the stick that is applied only to the processed input events.")]
        [SerializeField][Range(0.0f,0.5f)]float _deadzone = 0.0f;
        [Tooltip("If set to true, all processed input events will have inverted outputs.")]
        [SerializeField] bool _bShouldInvert = false;
        [Tooltip("Processed inputs are scaled by this factor, if set to one returns the standard [-1..1] Vector2 a joystick would provide")]
        [SerializeField] float _scale = 1;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        public override void SetControlPath(string _newControlPath)
        {
            _controlPath = _newControlPath;
            _stickControlPath = _stickControlPath.Equals(_newControlPath) ?_stickControlPath:_newControlPath;
        }

#if QUICKPORT_INPUTSYSTEM_ENABLED
        [InputControl(layout = "Vector2")]
        [SerializeField] string _stickControlPath;
#endif

        Vector2 _rawInput;
        Vector2 _processedInput;

        public delegate void JoystickInputCallback(Vector2 value);
        /// <summary>
        /// Event that passes the raw input received from the QPStick.
        /// </summary>
        public event JoystickInputCallback e_OnRawInputSent;
        public UnityEvent<Vector2> _OnRawInputSent;
        /// <summary>
        /// Event that passes the processed input according to settings.
        /// </summary>
        public event JoystickInputCallback e_OnProcessedInputSent;
        public UnityEvent<Vector2> _OnProcessedInputSent;

#if QUICKPORT_INPUTSYSTEM_ENABLED
        void Awake()
        {
            SetControlPath(_stickControlPath);
        }
#endif


        /// <summary>
        /// The raw input of the Stick as a Vector2
        /// </summary>
        public Vector2 RawInput
        {
            get => _rawInput;
        }

        /// <summary>
        /// The Input after processing has been applied as a Vector2
        /// </summary>
        public Vector2 ProcessedInput
        {
            get => _processedInput;
        }


        /// <summary>
        /// The function that is thrown upon receiving an input event from a QPDevice.
        /// Sets raw Input to new input, processes input and sets that as well.
        /// Throws all unity and C# events.
        /// </summary>
        /// <param name="value">The Input as received from the QPDevice</param>
        protected override void ProcessInput(Vector2 value)
        {
            base.ProcessInput(value);
            _rawInput = value;
            if (value.magnitude < _deadzone)
            {
                _processedInput = Vector2.zero;
            }
            else
            {
                _processedInput = value * _scale * (_bShouldInvert ? -1 : 1);
            }
            e_OnRawInputSent?.Invoke(_rawInput);
            e_OnProcessedInputSent?.Invoke(_processedInput);
            _OnRawInputSent?.Invoke(_rawInput);
            _OnProcessedInputSent?.Invoke(_processedInput);
        }

    }
}
