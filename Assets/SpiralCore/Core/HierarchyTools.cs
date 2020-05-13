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

namespace Spiral.Core
{
    public static class HierarchyTools
    {
        /// <summary>
        /// Возвращает глубину объекта в иерархии сцены
        /// </summary>
        /// <param name="transform">Целевой трансформ</param>
        /// <returns></returns>
        public static int GetHierarchyDepth(this Transform transform)
        {
            int depth = 0;
            Transform track = transform;
            while (track.parent != null) { track = track.parent; depth++; }
            return depth;
        }

        /// <summary>
        /// Возвращает глубину объекта в иерархии сцены
        /// </summary>
        /// <param name="transform">Целевой объект</param>
        /// <returns></returns>
        public static int GetHierarchyDepth(this GameObject gameObject)
        {
            int depth = 0;
            Transform track = gameObject.transform;
            while (track.parent != null) { track = track.parent; depth++; }
            return depth;
        }

        /// <summary>
        /// Возвращает все дочерние трансформы
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public static List<Transform> GetChildTransforms(this Transform transform)
        {
            List<Transform> output = new List<Transform>();

            // собираем детишек первого уровня, дальше будем идти по ним
            output.AddRange(transform.GetFirstLevelChildren());

            // обращаю внимание, что output.Count УВЕЛИЧИВАЕТСЯ в течение цикла до тех пор, пока не будут взяты все
            // дочерние трансформы всех уровней
            for (int i = 0; i < output.Count; i++) 
            {
                Transform child = output[i];
                List<Transform> granchildren = child.GetFirstLevelChildren();
                if (granchildren.Count != 0) { output.AddRange(granchildren); } 
            }

            return output;
        }

        /// <summary>
        /// Возвращает все дочерние трансформы
        /// </summary>
        /// <param name="gameObject">Game Object</param>
        /// <returns></returns>
        public static List<Transform> GetChildren(this GameObject gameObject)
        {
            return gameObject.transform.GetChildTransforms();
        }

        /// <summary>
        /// Собирает дочерние трансформы первого уровня
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public static List<Transform> GetFirstLevelChildren(this Transform transform)
        {
            int childCount = transform.childCount;
            List<Transform> output = new List<Transform>();
            for (int i = 0; i < childCount; i++) output.Add(transform.GetChild(i));
            return output;
        }

        /// <summary>
        /// Собирает дочерние трансформы первого уровня
        /// </summary>
        /// <param name="gameObject">Game Object</param>
        /// <returns></returns>
        public static List<Transform> GetFirstLevelChildren(this GameObject gameObject)
        {
            int childCount = gameObject.transform.childCount;
            List<Transform> output = new List<Transform>();
            for (int i = 0; i < childCount; i++) output.Add(gameObject.transform.GetChild(i));
            return output;
        }

        /// <summary>
        /// Устанавливает уровень для группы трансформов
        /// </summary>
        /// <param name="transforms">Массив трансформов</param>
        /// <param name="layer">Уровень</param>
        /// <param name="includeKinder">Включая дочерние трансформы (всех)</param>
        public static void SetLayer(this List<Transform> transforms, int layer, bool includeKinder)
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                Transform transform = transforms[i];
                transform.gameObject.layer = layer;
                if (includeKinder)
                {
                    List<Transform> kinder = transform.GetChildTransforms();
                    for (int k = 0; k < kinder.Count; k++)
                    {
                        kinder[k].gameObject.layer = layer;
                    }
                }
            }
        }

        /// <summary>
        /// Собрать все трансформы на сцене
        /// </summary>
        /// <returns></returns>
        public static List<Transform> CollectScene()
        {
            var output = new List<Transform>();

            var rootsGO = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
            var rootsTR = rootsGO.ToTransforms();

            for (int i = 0; i < rootsTR.Count; i++)
            {
                Transform target = rootsTR[i];
                output.Add(target);
                var childs = GetChildTransforms(target);
                if (childs.Count != 0) output.AddRange(childs);
            }

            return output;
        }

        /// <summary>
        /// Собрать все трансформы в сцене
        /// </summary>
        /// <param name="scene">Сцена</param>
        /// <returns>Список трансформов</returns>
        public static List<Transform> CollectScene(this Scene scene)
        {
            var output = new List<Transform>();

            var rootsGO = scene.GetRootGameObjects().ToList();
            var rootsTR = rootsGO.ToTransforms();

            for (int i = 0; i < rootsTR.Count; i++)
            {
                Transform target = rootsTR[i];
                output.Add(target);
                var childs = GetChildTransforms(target);
                if (childs.Count != 0) output.AddRange(childs);
            }

            return output;
        }

        /// <summary>
        /// Преобразует лист класса-родителя к листу класса-наследника.
        /// Обрати внимание: если объекты в листе не являются объектами класса-наследника,
        /// то у нас будет просто лист null'ек, так как никакие проверки не проводятся!
        /// </summary>
        /// <typeparam name="ChildType">Дочерний тип</typeparam>
        /// <typeparam name="ParentType">Родительский тип</typeparam>
        /// <param name="parentTypeList"></param>
        /// <returns></returns>
        public static List<ChildType> ToChildType<ChildType, ParentType>(this IList<ParentType> parentTypeList) 
            where ChildType : class // да, именно так, а не наследник от ParentType
            where ParentType : class
        {
            if (parentTypeList == null) throw new ArgumentNullException("Input cannot be null");

            List<ChildType> childTypeList = new List<ChildType>();
            int count = parentTypeList.Count;
            for (int i = 0; i < count; i++)
            {
                ChildType childTypeObject = parentTypeList[i] as ChildType;
                childTypeList.Add(childTypeObject);
            }
            return childTypeList;
        }

        /// <summary>
        /// (НЕ) быстрая конвертация Transform в Game Object
        /// Не использовать в цикле и в апдейте!
        /// </summary>
        /// <param name="transforms"></param>
        /// <returns></returns>
        public static List<GameObject> ToGameObjects(this IList<Transform> transforms)
        {
            List<GameObject> reply = new List<GameObject>();
            for (int i = 0; i < transforms.Count; i++)
            {
                reply.Add(transforms[i].gameObject);
            }
            return reply;
        }

        /// <summary>
        /// (НЕ) быстрая конвертация Game Object в Transform
        /// Не использовать в цикле и в апдейте!
        /// </summary>
        /// <param name="gameObjects">Любого вида массив объектов</param>
        /// <returns></returns>
        public static List<Transform> ToTransforms(this IList<GameObject> gameObjects)
        {
            List<Transform> reply = new List<Transform>();
            for (int i = 0; i < gameObjects.Count; i++)
            {
                reply.Add(gameObjects[i].transform);
            }
            return reply;
        }
    }
}

