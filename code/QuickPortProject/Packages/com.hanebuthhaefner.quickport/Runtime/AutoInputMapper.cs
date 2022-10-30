using System.Collections.Generic;
using UnityEngine;
#if QUICKPORT_INPUTSYSTEM_ENABLED
using UnityEngine.InputSystem;
#endif

namespace QuickPortAPI
{
    /// <summary>
    /// This script is concerned with automatically mapping existing control paths of a chosen controller type and ActionMap to UI controls.
    /// Assets required for input and output can be linked through the Unity Inspector.
    /// All output assets (i.e. buttons and joysticks) need to adhere to the specific Quickport guidlines in order to function.
    /// Refer to the documentation to ensure that custom asset made using Quickport are set up properly in order to be used by the AutoInputMapper.
    /// 
    /// !This script needs to be attached to a Canvas within a game scene!
    /// The created UI elements are not strictly tied to that canvas and can be used and customized in every way.
    /// </summary>
    public class AutoInputMapper : MonoBehaviour
    {
#if QUICKPORT_INPUTSYSTEM_ENABLED
        /// <summary>
        /// The _inputActionScript holds the entire input system of a game.
        /// All required data will be read from it.
        /// </summary>
        public InputActionAsset _inputActionScript;
#endif
        /// <summary>
        /// The _button needs to be a button with the QPButton script attached.
        /// It needs to be a Unity Button or variant of it.
        /// </summary>
        public GameObject _button;

        /// <summary>
        /// The _joystick needs to be a joystick with the QuickportJoystick script attached.
        /// The _joystick knob needs to be nested in a Unity Image/Panel.
        /// Also the _joystick needs to be a kind of Unity Button.
        /// </summary>
        public GameObject _joystick;

        /// <summary>
        /// _actionMapName is the specific name of a desired ActionMap.
        /// It is not typed manually, but selected via a drop down menu.
        /// </summary>
        string _actionMapName;

        /// <summary>
        /// The _controllerName is the specific name of a desired controller type.
        /// It is not typed manually, but selected via a drop down menu.
        /// </summary>
        string _controllerName;

        /// <summary>
        /// GenerateUIControls() is mainly called through the Unity Inspector.
        /// The method starts the process of mapping the controlpaths from the InputActionAsset to UI controls.
        /// It is checked whether the script is attached to a canvas, as that is required.
        /// After passing the check variables will be set and Init() will be called.
        /// </summary>
        /// <param name="_controllerType">The Type of Controller UI should be generated for</param>
        /// <param name="_actionMap">The actionmap asset used to generate UI</param>
        public void GenerateUIControls(string _controllerType, string _actionMap)
        {
            if (this.transform != null && this.GetComponent<Canvas>())
            {
                _controllerName = _controllerType;
                _actionMapName = _actionMap;

                QuickPortLogger.QPLog(this, "Selected controller: " + _controllerName + "; Selected map: " + _actionMapName + " \n " +
                                      "Starting Init()");

                Init();
            }
            else
            {
                QuickPortLogger.QPLogError(this, "The parent object of the ActionMapReader is not a Canvas or not existing! \n The ActionMapReader script requires a Canvas as parent!");
            }
        }

        /// <summary>
        /// Init() checks whether the _inputActionScript is linked and holds at least one ActionMap.
        /// The ActionMap is then bound using _actionMapName as key.
        /// In case _actionMapName is invalid, the process will stop. This cannot happen if the script is used through the UnityInspector as intended!
        /// </summary>
#if QUICKPORT_INPUTSYSTEM_ENABLED
        void Init()
        {
            //check whether _inputActionScript is missing.
            if (_inputActionScript is null)
            {
                QuickPortLogger.QPLogError(this, $"There is no InputActionAsset linked to {this.name}!\n No inputs found!");
                return;
            }
            //check whether _inputActionScript is empty
            else if (_inputActionScript.actionMaps.Count == 0)
            {
                QuickPortLogger.QPLogError(this, $"There are no ActionMaps defined for {_inputActionScript.name}!");
                return;
            }

            //Give necessary parameters to  an object of ActionMapReader, in order to get a refined lists of available actions.
            ActionMapReader _autoReader = new ActionMapReader(_inputActionScript, _actionMapName, _controllerName);

            List<string> _joystickBindings = _autoReader._joystickBindings;
            List<string> _buttonBindings = _autoReader._buttonBindings;

            QuickPortLogger.QPLog(this, "" + _joystickBindings.Count + " bindings for joysticks and \n " +
                                  "" + _buttonBindings.Count + " bindings for buttons found. \n" +
                                  "Starting AutoMap()");

            AutoMap(_joystickBindings, _buttonBindings);
        }
#else
        private void Init()
        {
            
        }
#endif
        /// <summary>
        /// AutoMap() creates assets of type of the linked assets and maps the previously acquired bindings to them.
        /// </summary>
        void AutoMap(List<string> _joystickBindings, List<string> _buttonBindings)
        {
            
            foreach( string _joystickBinding in _joystickBindings)
            {
                
                if(_joystick.GetComponent<QPStickInput>())
                {
                    Instantiate(_joystick, new Vector3(), Quaternion.identity, this.transform);
                    _joystick.GetComponent<QPStickInput>().SetControlPath(_joystickBinding);
                    _joystick.name = _joystickBinding;

                    QuickPortLogger.QPLog(this, " Instantiated  joystick for binding: " + _joystickBinding);
                }
                else
                {
                    QuickPortLogger.QPLog(this, "Unable to map binding: " + _joystickBinding + " to joystick.  Please check whether the prefab is cómpatible with AtuoInputMapper.");
                }

                
            }

            foreach(string _buttonBinding in _buttonBindings)
            {
                if (_button.GetComponent<QPButtonInput>())
                {
                    Instantiate(_button, new Vector3(), Quaternion.identity, this.transform);
                    _button.GetComponent<QPButtonInput>().SetControlPath(_buttonBinding);
                    _button.name = _buttonBinding;

                    QuickPortLogger.QPLog(this, " Instantiated  button for binding: " + _buttonBinding);
                }
                else
                {
                    QuickPortLogger.QPLog(this, "Unable to map binding: " + _buttonBinding + " to button. Please check whether the prefab is cómpatible with AtuoInputMapper.");
                }
                

                
            }

        }

        /// <summary>
        /// AutoReadMaps() utilises the ActionMapScanner to scan for ActionMaps.
        /// The method simply delegates functionality to a different script rather than holding all functionality in one script.
        /// </summary>
        /// <returns>Names of the actionmaps</returns>
#if QUICKPORT_INPUTSYSTEM_ENABLED
         public List<string> AutoReadMaps()
        {
            //Check whether an InputActionAsset is set. This check is technically not required when using this script through the Unity Inspector. This check comes in effect, when someone wants to use this script on a codign level.
            if(_inputActionScript is null)
            {
                QuickPortLogger.QPLogError(this, "The _inputActionScript must be initialised before traing to read values from it! \n " +
                                "A null value will be returned instead! \n" +
                                "Please initialise _inputActionScript before calling AutoReadMaps().");
                return null;
            }
            else
            {
                ActionMapScanner _mapScanner = new ActionMapScanner(_inputActionScript);
                return _mapScanner._actionMapNames;
            }
        }
#else
        public List<string> AutoReadMaps()
        {
            return null;
        }
#endif
    }
}
