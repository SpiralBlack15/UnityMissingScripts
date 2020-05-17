// *********************************************************************************
// The MIT License (MIT)
// Copyright (c) 2020 SpiralBlack https://github.com/SpiralBlack15
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

using Spiral.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorToolkit
{
    public static class SpiralEditorTools
    {
        private static List<string> assetsPaths;
        
        private static void ReloadAssets() // TODO: потом сделать с настройками
        {
            assetsPaths = AssetDatabase.GetAllAssetPaths().Listed();
        }

        static SpiralEditorTools() // при компиляции должно обновиться
        {
            ReloadAssets();
        }

        private static List<string> FindPaths(string filter)
        {
            var findings = assetsPaths.Where(x => x.Contains(filter));
            return new List<string>(findings);
        }

        public static T GetSetializedObject<T>(this Editor editor, ref T target) where T : UnityEngine.Object
        {
            if (target == null) target = editor.serializedObject.targetObject as T;
            return target;
        }

        public static T GetSerializedObject<T>(this SerializedObject serializedObject, ref T target) where T : UnityEngine.Object
        {
            if (target == null) target = serializedObject.targetObject as T;
            return target;
        }

        public static MonoScript CashedMono<T>(ref MonoScript monoScript)
        {
            if (monoScript == null)
            {
                monoScript = GetMonoScript(typeof(T));
            }
            return monoScript;
        }

        public static MonoScript CashedMono(this PropertyDrawer propertyDrawer, ref MonoScript monoScript, bool drawerScript = false)
        {
            if (monoScript == null) 
            {
                UnityEngine.Debug.Log($"Get drawer script {propertyDrawer.GetType()}");
                if (drawerScript)
                {
                    monoScript = GetMonoScript(propertyDrawer.GetType());
                    if (monoScript == null)
                    {
                        Type targetType = propertyDrawer.fieldInfo.FieldType;
                        monoScript = GetMonoScript(targetType);
                    }
                }
                else
                {
                    Type targetType = propertyDrawer.fieldInfo.FieldType;
                    monoScript = GetMonoScript(targetType);
                }
            }
            return monoScript;
        }

        public static MonoScript CahsedMono<T>(ref MonoScript monoScript) // да, здесь действительно нужен ref
        {
            if (monoScript == null)
            {
                monoScript = GetMonoScript(typeof(T));
            }
            return monoScript;
        }

        public static MonoScript CashedMono(this UnityEngine.Object anyUnityObject, ref MonoScript monoScript)
        {
            if (monoScript == null) 
            { 
                monoScript = GetMonoScript(anyUnityObject.GetType()); 
            }
            return monoScript;
        }

        public static string GetGUIDUnity(string assetName)
        {
            string[] answer = AssetDatabase.FindAssets(assetName);
            if (answer.Length == 0) return "";
            else return answer[0];
        }

        public static string GetGUID(Type userType) // убедитесь, что имя класса сходится с именем ассета!
        {
            string filter = $"{userType.Name}.cs";
            List<string> names = FindPaths(filter);
            switch (names.Count)
            {
                case 0: return "";
                default:
                    string firstPath = names[0];
                    return AssetDatabase.AssetPathToGUID(firstPath);
            }
        }

        public static string GetExactAssetPath(Type userType)
        {
            string filter = $"{userType.Name}.cs";
            List<string> names = FindPaths(filter);
            switch (names.Count)
            {
                case 0: return "";
                default: return names[0];
            }
        }

        public static MonoScript GetMonoScript(Type userType)
        {
            string path = GetExactAssetPath(userType);
            if (path == "") return null;
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            return script;
        }

        public static void PressF()
        {
            UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
        }
    }
}
#endif

