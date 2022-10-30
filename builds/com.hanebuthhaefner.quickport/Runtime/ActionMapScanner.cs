using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QuickPortAPI
{
    /// <summary>
    /// The ActionMapScanner serves as additional functionality to the AutoInputMapper.
    /// This script simply scans a given InputActionAsset for ActionMaps and fills a List<string> with their names.
    /// Those names can be used to reference the given ActionMaps and gain access to the individual actions, bindings, etc.
    /// </summary>
    public class ActionMapScanner
    {
        /// <summary>
        /// A List<string> that holds all names of ActionMaps for a given InputActionAsset.
        /// It has a public getter.
        /// </summary>
        public List<string> _actionMapNames
        {
            get;
            private set;
        }

        /// <summary>
        /// The InputActionAsset must be set when constructing an object of this class.
        /// </summary>
        InputActionAsset _inputActionScript;

        /// <summary>
        /// The constructor requires an InputActionAsset.
        /// It will call the SearchInputActionScript() method, which populates the List<string> _actionMapNames.
        /// </summary>
        /// <param name="_newScript">Inputactionasset to find inputactionscript for</param>
        public ActionMapScanner(InputActionAsset _newScript)
        {
            _actionMapNames = new List<string>();
            _inputActionScript = _newScript;
            SearchInputActionScript();
        }

        void SearchInputActionScript()
        {
            foreach( InputActionMap _map in _inputActionScript.actionMaps)
            {
                _actionMapNames.Add(_map.name);
            }
        }
    }
}
