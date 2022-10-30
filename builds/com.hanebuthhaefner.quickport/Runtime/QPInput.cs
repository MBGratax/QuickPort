using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickPortAPI
{

/// <summary>
/// The Basic implementation of the script that handles the processing and further sending of Input received by a QPDevice.
/// </summary>
/// <typeparam name="TValue">The input type this input handles. Needs to match the QPDevice.</typeparam>
#if QUICKPORT_INPUTSYSTEM_ENABLED
    public class QPInput<TValue> : UnityEngine.InputSystem.OnScreen.OnScreenControl where TValue : struct
#else
    public class QPInput<TValue> : MonoBehaviour where TValue : struct
#endif
    {
        /// <summary>
        /// Reference to the device that sends the input.
        /// </summary>
        QPDevice<TValue> quickPortDevice;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        /// <summary>
        /// Used to overwrite the control path.
        /// </summary>
        /// <param name="_newControlPath">String in the controlPath format of the Unity InputSystem.</param>
        public virtual void SetControlPath(string _newControlPath)
        {
            _controlPath = _newControlPath;
        }

        internal string _controlPath;

        /// <summary>
        /// Finds the QPDevice and subscribes the ProcessInput method to the OnInputSent event.
        /// </summary>
        protected virtual void Start()
        {
            quickPortDevice = GetComponent<QPDevice<TValue>>();
            quickPortDevice.e_OnInputSent += ProcessInput;
        }

        void OnDestroy()
        {
            quickPortDevice.e_OnInputSent -= ProcessInput;
        }

        /// <summary>
        /// The method used to process Input and notify otehr scripts of it. By default this will always notify the Unity InputSystem.
        /// In other implementations this also process input and throw events.
        /// </summary>
        /// <param name="value">The input value as received from the QPDevice.</param>
        protected virtual void ProcessInput(TValue value)
        {
#if QUICKPORT_INPUTSYSTEM_ENABLED
            SendValueToControl(value);
#endif
        }
    }
}
