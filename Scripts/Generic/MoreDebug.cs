using System.Diagnostics;
using UnityEngine;

namespace Goldraven.Generic {
    
    public class MoreDebug {
        
        public const string splatRow = "**********************************";
    
        private static string header(){
            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(2).GetMethod().Name;
            string typeName = stackTrace.GetFrame(2).GetMethod().ReflectedType.Name;
            string clock = Time.realtimeSinceStartup.ToString();
            return clock + ": <color=blue>" + typeName + "</color>: <color=green>" + methodName + "</color>: ";
        }
        
        public static void Log(object message){
            UnityEngine.Debug.Log(header() + message );
        }
        
        
        public static void Log(object message, Object context){
            UnityEngine.Debug.Log(header() + message, context);
    
        }
        public static void LogError(object message){
            UnityEngine.Debug.LogError(header() + message );
        }
    
    
        public static void LogError(object message, Object context){
            UnityEngine.Debug.LogError(header() + message, context);
        
        }
    }
}