using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
#if QUICKPORT_INPUTSYSTEM_ENABLED
using UnityEngine.InputSystem;
#endif




namespace QuickPortAPI
{
    /// <summary>
    /// ActionMapReader supplies AutoInputMapper with additional functionality.
    /// The class reads all bindings from a specific ActionMap within a given ActionInputAsset.
    /// It only reads bindings, that are of a certain controller type and sorts them to joystick or button bindings.
    /// Those bindings are stored in List<string> objects, that have public getters.
    /// </summary>
    public  class ActionMapReader 
    {
        /// <summary>
        /// _joystickBindings holds all input bindings, that likely require a joystick.
        /// It has a public getter.
        /// </summary>
        public List<string> _joystickBindings
        {
            get;
            private set;
        }

        /// <summary>
        /// _buttonBindings holds all bindings that are not joystick specific and therefore buttons.
        /// It has a public getter.
        /// </summary>
        public List<string> _buttonBindings
        {
            get;
            private set;
        }

#if QUICKPORT_INPUTSYSTEM_ENABLED
        InputActionAsset _inputActionScript;
#endif
        string _actionMapName;
        string _controllerName;
#if QUICKPORT_INPUTSYSTEM_ENABLED
        InputActionMap _playerActionMap;
#endif

        /// <summary>
        /// The constructor of ActionMapReader takes in several parameters and immediately assigns there values to its own variables and starts the reading process.
        /// The constructor itself has no error handling as it itself doesn't need it, but the subsequent methods do.
        /// </summary>
        /// <param name="_newInputActionScript"></param>
        /// <param name="_newActionMapName"></param>
        /// <param name="_newControllerName"></param>
#if QUICKPORT_INPUTSYSTEM_ENABLED
        public ActionMapReader(InputActionAsset _newInputActionScript, string _newActionMapName, string _newControllerName)
        {
            _inputActionScript = _newInputActionScript;
            _actionMapName = _newActionMapName;
            _controllerName = _newControllerName;

            _joystickBindings = new List<string>();
            _buttonBindings = new List<string>();

            FindActionMap();
            FindApplicableBindings();
        }
#else
        public ActionMapReader()
        {
            QuickPortLogger.QPLogErrorNative("Without the new Input System there are no action maps to read and Automapping is not available!");
        }

#endif
        /// <summary>
        /// FindActionMap() simply gets the ActionMap specified in the string _actionMapName and saves it to a local variable.
        /// Should that variable not be set to a proper value after it is assigned a value, then an error is thrown.
        /// </summary>
#if QUICKPORT_INPUTSYSTEM_ENABLED
        void FindActionMap()
        {
            _playerActionMap = _inputActionScript.FindActionMap(_actionMapName);

            if (_playerActionMap is null)
            {
                QuickPortLogger.QPLogErrorNative( "The _actionMapName: " + _actionMapName + " is not found within the InputActionAsset! \n " +
                                "This is not possible if the InputActionAsset has been scanned correctly! \n" +
                                "Try scanning the InputActionAsset, verify that the correct asset is linked.");
            }
        }

        /// <summary>
        /// FindApplicableBindings() looks for all bindings in an ActionMap that are either expecting a "Vector2" or "Stick" and sorts those to the _joystickBindings.
        /// All other bindings are handled as buttons.
        /// Each binding is checked for what kind of controller it expects.
        /// If it matches the desired controller, the binding is written into the designated List<string>.
        /// </summary>
        
        void FindApplicableBindings()
        {
            if(_playerActionMap is null)
            {
                QuickPortLogger.QPLogErrorNative("_playerActionMap was null in FindApplicableBindings()! \n Cannot read inputs!");
            }
            else
            {
                foreach (InputAction _action in _playerActionMap)
                {
                    if (_action.expectedControlType.Equals("Vector2") || _action.expectedControlType.Equals("Stick"))
                    {
                        foreach (InputBinding _binding in _action.bindings)
                        {
                            if (_binding.ToString().Contains(_controllerName))
                            {
                                _joystickBindings.Add(_binding.path);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (InputBinding _binding in _action.bindings)
                        {
                            if (_binding.ToString().Contains(_controllerName))
                            {
                                _buttonBindings.Add(_binding.path);
                                break;
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}
