using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace QuickPortAPI
{
    /// <summary>
    /// This script is concerned with modifying the standard Unity Inspector for the AutoInputMapper script.
    /// It supplies the script with required buttons and some error handling.
    /// 
    /// Through this script the AutoInputMapper and its funcitonality in Quickport can be used without coding knowledge.
    /// </summary>
    [CustomEditor(typeof(AutoInputMapper))]
    public class AutoMapperEditor : Editor
    {
        /// <summary>
        /// A list of the supported controller types.
        /// Can be expanded if the controller's functionality is supported by the Quickport assets.
        /// </summary>
        string[] _controllerOptions = { "Gamepad", "Xbox", "PS4", "Switch Pro" };
        int _selectedController = -1;

        /// <summary>
        /// The List of available ActionMaps.
        /// Linking a InputActionAsset and scanning for ActionMaps will fill this list with all ActionMaps within that Asset.
        /// </summary>
        List<string> _actionMapOptions = new List<string>();
        int _selectedMap = -1;

        /// <summary>
        /// OnInspectorGUI() calls the standard inspector for every AutoInputMapper script.
        /// 
        /// Two drop down menus are added, the first to select a controller type, the second for selecting an ActionMap.
        /// 
        /// Below that two buttons are added, the first for scanning a linked InputActionAsset, the second for mapping controls to UI.
        /// Pressing the buttons calls methods from the AutoInputMapper, that in turn calls the other required classes of the Quickport Package.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

#if QUICKPORT_INPUTSYSTEM_ENABLED
            AutoInputMapper _script = (AutoInputMapper)target;

            _selectedController = EditorGUILayout.Popup("Controller Type", _selectedController, _controllerOptions);
            _selectedMap = EditorGUILayout.Popup("Selected ActionMap", _selectedMap, _actionMapOptions.ToArray());

            if(GUILayout.Button("Find ActionMaps"))
            {
                //Check whether an InputActionAssert is attached. That is the only requirement for this method
                if(_script._inputActionScript is null)
                {
                    QuickPortLogger.QPLogError(this, "The InputActionAsset needs to be linked before scanning for any ActionMaps! \n " +
                                    "Please attach a suitable InputActionAsset!");
                }
                else
                {
                    _actionMapOptions = _script.AutoReadMaps();
                }
            }

            if(GUILayout.Button("Map to UI controls"))
            {
                //Check whether options have been selected
                if (_selectedController < 0 || _selectedMap < 0 || _actionMapOptions.Count <= 0)
                {
                    QuickPortLogger.QPLogError(this, "You need to select a controller type and an ActionMap before Quickport can map controls to UI! \n" + 
                        "Please select a controller type and an ActionMap to scan for! \n " + 
                        "If there is no ActionMap selectable you will need to Scan for ActionMaps first.");
                }
                else
                {
                    _script.GenerateUIControls(_controllerOptions[_selectedController], _actionMapOptions[_selectedMap]);
                }
            }
            #endif
        }
    }
}
