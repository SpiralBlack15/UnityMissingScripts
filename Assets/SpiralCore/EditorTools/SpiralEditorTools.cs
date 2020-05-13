
#if UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEditor;
namespace Spiral.EditorToolkit
{
    public static class SpiralEditorTools 
    {

        /// <summary>
        /// Просто убивает юньку вызовом чистой виртуальной функции :)
        /// </summary>
        public static void PressF()
        {
            UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.PureVirtualFunction);
        }

        public static string GetGUID(string assetName)
        {
            string[] answer = AssetDatabase.FindAssets(assetName);
            if (answer.Length == 0) return "";
            else return answer[0];
        }

        public static string GetGUID(Type userType) // убедитесь, что имя класса сходится с именем ассета!
        {
            string[] answer = AssetDatabase.FindAssets(userType.Name);
            if (answer.Length == 0) return "";
            else return answer[0];
        }

        public static string GetAssetPath(Type userType)
        {
            string guid = GetGUID(userType);
            if (guid == "") return "";
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        public static MonoScript GetMonoScript(Type userType)
        {
            string path = GetAssetPath(userType);
            if (path == "") return null;
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            return script;
        }
    }
}
#endif

