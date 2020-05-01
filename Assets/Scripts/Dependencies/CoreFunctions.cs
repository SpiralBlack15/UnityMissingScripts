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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spiral.Core
{
    public static class CoreFunctions
    {
        /// <summary>
        /// Конвертирует любой массив в лист
        /// </summary>
        /// <typeparam name="T">Тип массива</typeparam>
        /// <param name="ts">Входной массив</param>
        /// <returns>Лист на основе входногоо массив</returns>
        public static List<T> Array2List<T>(this T[] ts)
        {
            return new List<T>(ts);
        }

        public static T GetLast<T>(this IList<T> ts)
        {
            return ts[ts.Count - 1];
        }

        public static void RemoveLast<T>(this IList<T> ts)
        {
            ts.RemoveAt(ts.Count - 1);
        }


        /// <summary>
        /// Конвертирует лист Transforms в лист Game Objects
        /// </summary>
        public static List<GameObject> Transforms2GameObjects(this List<Transform> transforms)
        {
            List<GameObject> reply = new List<GameObject>();
            for (int i = 0; i < transforms.Count; i++)
            {
                reply.Add(transforms[i].gameObject);
            }
            return reply;
        }

        /// <summary>
        /// Конвертирует лист Game Objects в лист Transforms
        /// </summary>
        public static List<Transform> GameObjects2Transforms(this List<GameObject> gameObjects)
        {
            List<Transform> reply = new List<Transform>();
            for (int i = 0; i < gameObjects.Count; i++)
            {
                reply.Add(gameObjects[i].transform);
            }
            return reply;
        }

        /// <summary>
        /// Собирает дочерние трансформы первого уровня
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public static List<Transform> CollectChildFirstLevelTransforms(this Transform transform)
        {
            int childCount = transform.childCount;
            List<Transform> output = new List<Transform>();
            for (int i = 0; i < childCount; i++)
            {
                output.Add(transform.GetChild(i));
            }
            return output;
        }

        /// <summary>
        /// Возвращает все дочерние трансформы
        /// </summary>
        /// <param name="transform">Трансформ, дочерние трансформы которого собираются</param>
        public static List<Transform> CollectChildTransforms(this Transform transform)
        {
            List<Transform> output = new List<Transform>();
            output.AddRange(transform.CollectChildFirstLevelTransforms());
            // обращаю внимание, что output.Count УВЕЛИЧИВАЕТСЯ в течение цикла,
            // до тех пор, пока не будут взяты все дочерние трансформы всех уровней
            for (int i = 0; i < output.Count; i++)
            {
                Transform child = output[i];
                List<Transform> granchildren = child.CollectChildFirstLevelTransforms();
                if (granchildren.Count != 0) { output.AddRange(granchildren); }
            }

            return output;
        }

        /// <summary>
        /// Собирает все трансформы со сцены
        /// </summary>
        public static List<Transform> CollectScene()
        {
            var output = new List<Transform>();

            var rootsGO = SceneManager.GetActiveScene().GetRootGameObjects().Array2List();
            var rootsTR = rootsGO.GameObjects2Transforms();

            for (int i = 0; i < rootsTR.Count; i++)
            {
                Transform target = rootsTR[i];
                output.Add(target);
                var childs = CollectChildTransforms(target);
                if (childs.Count != 0) output.AddRange(childs);
            }

            return output;
        }

        /// <summary>
        /// Лист в одну строчку
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string List2String(this List<string> entry)
        {
            string output = "";
            int count = entry.Count;
            for (int e = 0; e < count; e++)
            {
                output += entry[e];
                if (e != count - 1) output += "\n";
            }
            return output;
        }
    }
}

