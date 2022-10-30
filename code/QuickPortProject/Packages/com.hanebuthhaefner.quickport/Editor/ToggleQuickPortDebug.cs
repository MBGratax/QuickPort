using UnityEditor;

namespace QuickPortAPI
{
    /// <summary>
    /// This Editor class is used to add the QuickPortDebug Symbol to the project. The symbol can be activated or deactivated in the MenuItem QuickPort.
    /// The Symbol is only added to buildgroups iOS and Android and the symbol is only used to enable or disable QuickPort Logs.
    /// </summary>
    public class ToggleQuickPortDebug : Editor
    {
        /// <summary>
        /// The BuildTargetGroups for which the QuickPortDebug Symbol will be added via the MenuItem.
        /// </summary>
        public static readonly BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
        {
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS
        };

        /// <summary>
        /// Adds QuickPortDebug Symbol to all specified BuildTargetGroups.
        /// </summary>
        [MenuItem("QuickPort/Debug/Activate")]
        public static void ActivateDebug()
        {
            foreach (BuildTargetGroup buildTargetGroup in buildTargetGroups)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

                if (!symbols.Contains("QUICKPORT_DEBUG"))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(
                        buildTargetGroup, symbols + ";QUICKPORT_DEBUG");
                }
            }
        }

        /// <summary>
        /// Removes QuickPortDebug Symbol to all specified BuildTargetGroups.
        /// </summary>
        [MenuItem("QuickPort/Debug/Deactivate")]
        public static void DeactivateDebug()
        {

            foreach (BuildTargetGroup buildTargetGroup in buildTargetGroups)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(
               buildTargetGroup);

                if (symbols.Contains("QUICKPORT_DEBUG"))
                {
                    symbols = symbols.Replace(";QUICKPORT_DEBUG", "");
                    symbols = symbols.Replace("QUICKPORT_DEBUG", "");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(
                        buildTargetGroup, symbols);
                }
            }
        }

    }
}
