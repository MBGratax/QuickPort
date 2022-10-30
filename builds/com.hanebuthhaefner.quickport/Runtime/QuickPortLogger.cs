using System.Diagnostics;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace QuickPortAPI
{
    /// <summary>
    /// This class is a fancy logging system that is only enabled if the QUICKPORT_DEBUG symbol is present in the project. 
    /// This symbol can be enabled or disabled in the dropdown menu "QuickPort".
    /// </summary>
    public static class QuickPortLogger
    {
        [Conditional("QUICKPORT_DEBUG")]
        private static void Log(Action<string, Object> LogFunction, string prefix, Object myObj, params object[] message)
        {

            LogFunction($"{prefix}[<color=lightblue>{myObj.name}</color>]: {String.Join(", ",message)}", myObj);

        }

        /// <summary>
        /// Uses Unities Debug.Log to write a fancy message.
        /// </summary>
        /// <param name="myObj">The UnityObject that is sending the log.</param>
        /// <param name="message">The message as C# object params.</param>
        [Conditional("QUICKPORT_DEBUG")]
        public static void QPLog(this Object myObj,params object[] message)
        {
            Log(UnityEngine.Debug.Log, "[QuickPort]: ", myObj, message);
        }

        /// <summary>
        /// Uses Unities Debug.LogError to write a fancy message.
        /// </summary>
        /// <param name="myObj">The UnityObject that is sending the log.</param>
        /// <param name="message">The message as C# object params.</param>
        [Conditional("QUICKPORT_DEBUG")]
        public static void QPLogError(this Object myObj,params object[] message)
        {
            Log(UnityEngine.Debug.LogError, "<color=red><!></color> [QuickPort]: ", myObj, message);
        }

        /// <summary>
        /// Uses Unities Debug.LogWarning to write a fancy message.
        /// </summary>
        /// <param name="myObj">The UnityObject that is sending the log.</param>
        /// <param name="message">The message as C# object params.</param>
        [Conditional("QUICKPORT_DEBUG")]
        public static void QPLogWarning(this Object myObj, params object[] message)
        {
            Log(UnityEngine.Debug.LogWarning, "<color=yellow><!></color> [QuickPort]: ", myObj, message);
        }

        /// <summary>
        /// Uses Debug.LogException to throw an exception.
        /// </summary>
        /// <param name="myObj">The UnityObject that is throwing the exception.</param>
        /// <param name="exception">The C# exception.</param>
        [Conditional("QUICKPORT_DEBUG")]
        public static void QPLogException(this Object myObj, System.Exception exception)
        {
            UnityEngine.Debug.LogException(exception, myObj);
        }

        /// <summary>
        /// Uses Debug.Log to log from a non UnityObject.
        /// </summary>
        /// <param name="myObj">The C# object that is logging an error.</param>
        /// <param name="message">The message as C# object params.</param>
        [Conditional("QUICKPORT_DEBUG")]
        public static void QPLogErrorNative(this object myObj, params object[] message)
        {
            UnityEngine.Debug.LogError($"<color=red><!></color> [QuickPort]: [<color=lightblue>{myObj.GetType()}</color>]: {String.Join(",", message)}");
        }
    }
}
