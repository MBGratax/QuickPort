using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if QUICKPORT_INPUTSYSTEM_ENABLED
using UnityEngine.InputSystem.Layouts;
#endif

namespace QuickPortAPI
{
    public class QPButtonInput : QPInput<float>
    {

        bool _buttonState = false;
        bool _lastValue = false;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        public override void SetControlPath(string _newControlPath)
        {
            _controlPath = _newControlPath;
            _buttonControlPath = _buttonControlPath.Equals(_newControlPath) ? _buttonControlPath : _newControlPath;
        }

#if QUICKPORT_INPUTSYSTEM_ENABLED
        [InputControl(layout = "Button")]
        [SerializeField] string _buttonControlPath;
#endif
        /// <summary>
        /// Get the current button state, true if pressed, false if released.
        /// </summary>
        public bool ButtonState
        {
            get => _buttonState;
        }

#if QUICKPORT_INPUTSYSTEM_ENABLED
        void Awake()
        {
            SetControlPath(_buttonControlPath);
        }
#endif

        public delegate void ButtonStateCallback();
        /// <summary>
        /// Event that notifies if the button was pressed.
        /// </summary>
        public event ButtonStateCallback e_OnButtonDown;
        public UnityEvent onButtonDown;
        /// <summary>
        /// Event that notifies if the button was released.
        /// </summary>
        public event ButtonStateCallback e_OnButtonUp;
        public UnityEvent onButtonUp;
        public delegate void ButtonValueCallback(bool state);
        /// <summary>
        /// Event that passes the current state of the button whenever it changes.
        /// </summary>
        public event ButtonValueCallback e_OnButtonValueChange;
        public UnityEvent<bool> onButtonValueChanged;

        
        


        /// <summary>
        /// Throws Buttondown and Buttonup events. Is only called on state changes and will also notify those.
        /// </summary>
        /// <param name="value">The current state of the Button as received from a QPDevice</param>
        protected override void ProcessInput(float value)
        {
            base.ProcessInput(value);
            _buttonState = Mathf.Approximately(Mathf.Min(value,1),1);
            if (!_lastValue && _lastValue != _buttonState)
            {
                e_OnButtonDown?.Invoke();
                onButtonDown?.Invoke();
            }else if(_lastValue && _lastValue != _buttonState)
            {
                e_OnButtonUp?.Invoke();
                onButtonUp?.Invoke();
            }
            _lastValue = _buttonState;

            e_OnButtonValueChange?.Invoke(_buttonState);
            onButtonValueChanged?.Invoke(_buttonState);

        }

    }
}
