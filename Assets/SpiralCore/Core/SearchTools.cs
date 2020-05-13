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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Spiral.Core
{
    public static class SearchTools
    {
#if UNITY_EDITOR
        public static void Select(List<UnityEngine.Object> objects)
        {
            Selection.objects = objects.ToArray();
        }
#endif

        public static Scene CurrentScene()
        {
            return SceneManager.GetActiveScene();
        }

        public static List<GameObject> GetCurrentSceneRoots()
        {
            return CurrentScene().GetRootGameObjects().ToList();
        }

        public static List<Component> ExclusiveSearch(Type type, Type exclude)
        {
            List<Component> findings = Find(type);
            List<Component> output = new List<Component>();
            for (int i = 0; i < findings.Count; i++)
            {
                var forbidden = findings[i].GetComponent(exclude);
                if (forbidden == null) output.Add(forbidden);
            }
            return output;
        }

        public static List<Component> IncludiveSearch(Type type, Type include)
        {
            List<Component> findings = Find(type);
            List<Component> output = new List<Component>();
            for (int i = 0; i < findings.Count; i++)
            {
                var addition = findings[i].GetComponent(include);
                if (addition != null) output.Add(addition);
            }
            return output;
        }

        public static List<Component> FindSetOfTypes(List<Type> types)
        {
            if (types == null) return new List<Component>();
            if (types.Count == 0) return new List<Component>();

            List<Component> findings = Find(types[0]); // базовый поиск

            for (int i = 1; i < types.Count; i++) // поиск по типу
            {
                if (findings.Count == 0) break;

                Type type = types[i];
                List<Component> excludeElements = new List<Component>();

                for (int elementIDX = 0; elementIDX < findings.Count; elementIDX++)
                {
                    var addition = findings[elementIDX].GetComponent(type);
                    if (addition == null) excludeElements.Add(findings[elementIDX]);
                }

                for (int elementIDX = 0; elementIDX < excludeElements.Count; elementIDX++)
                {
                    findings.Remove(excludeElements[elementIDX]);
                }
            }

            return findings;
        }

        public static List<T> Find<T>(bool select = false) where T : Component
        {
            var findings = Find(typeof(T), select);
            return findings.ToChildType<T, Component>();
        }

        public static T FindInstance<T>() where T : Component
        {
            var findings = Find(typeof(T), false);
            if (findings.Count == 0) return null;
            else return findings[0] as T;
        }

        public static (T component, bool created) FindInstanceA<T>(bool dontDestroyOnLoad) where T : Component
        {
            var findings = Find(typeof(T), false);
            if (findings.Count == 0)
            {
                GameObject go = new GameObject($"[{typeof(T).Name}]");
                if (dontDestroyOnLoad)
                {
                    UnityEngine.Object.DontDestroyOnLoad(go);
                }
                return (go.AddComponent<T>(), true);
            }
            else return (findings[0] as T, false);
        }

        public static T PickInstance<T>(ref T data) where T : Component
        {
            if (data == null) data = FindInstance<T>();
            return data;
        }

        public static (T component, bool created) PickInstanceA<T>(ref T data, bool dontDestroyOnLoad) where T : Component
        {
            if (data == null)
            {
                var (component, created) = FindInstanceA<T>(dontDestroyOnLoad);
                data = component;
                return (data, created);
            }
            return (data, false);
        }

        /// <summary>
        /// Ищет все компоненты 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Component> Find(this Type type, bool select = false)
        {
            List<Component> output = new List<Component>();
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                var components = roots[i].GetComponentsInChildren(type, true);
                if (components == null) continue;
                output.AddRange(components);
            }

#if UNITY_EDITOR
            if (select)
            {
                List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
                for (int i = 0; i < output.Count; i++)
                {
                    objects.Add(output[i].gameObject);
                }
                Selection.objects = objects.ToArray();
            }
#endif

            return output;
        }

#if UNITY_EDITOR
        public static void Select<T>(this List<T> components) where T : Component
        {
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            for (int i = 0; i < components.Count; i++)
            {
                objects.Add(components[i].gameObject);
            }
            Selection.objects = objects.ToArray();
        }
#endif

        public static List<T> FindOnScene<T>(this Scene scene, List<T> components = null)
        {
            if (components == null) components = new List<T>();

            var rootGameObjects = scene.GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                var rootGOComponents = rootGameObject.GetComponentsInChildren<T>(true);

                components.AddRange(rootGOComponents);
            }

            return components;
        }

        public static List<Component> FindOnScene(this Scene scene, Type t, List<Component> components = null)
        {
            if (components == null) components = new List<Component>();

            var rootGameObjects = scene.GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                var rootGOComponents = rootGameObject.GetComponentsInChildren(t, true);

                components.AddRange(rootGOComponents);
            }

            return components;
        }
    }
}