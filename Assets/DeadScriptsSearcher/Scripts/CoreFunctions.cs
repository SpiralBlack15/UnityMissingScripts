using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spiral.EditorTools.DeadScriptsSearcher
{
    /// <summary>
    /// Некоторые функции из моей основной библиотеки, которую я не буду выкладывать целиком
    /// Нужны здесь исключительно чтобы работало всё остальное
    /// </summary>
    public static class CoreFunctions
    {
        public static List<T> Array2List<T>(this T[] ts) where T : UnityEngine.Object
        {
            List<T> output = new List<T>();
            for (int i = 0; i < ts.Length; i++) { output.Add(ts[i]); }
            return output;
        }

        public static List<GameObject> Transforms2GameObjects(this List<Transform> transforms)
        {
            List<GameObject> reply = new List<GameObject>();
            for (int i = 0; i < transforms.Count; i++)
            {
                reply.Add(transforms[i].gameObject);
            }
            return reply;
        }

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
        /// <param name="transform">Transform</param>
        /// <returns></returns>
        public static List<Transform> CollectChildTransforms(this Transform transform)
        {
            List<Transform> output = new List<Transform>();

            // собираем детишек первого уровня, дальше будем идти по ним
            output.AddRange(transform.CollectChildFirstLevelTransforms());

            // обращаю внимание, что output.Count УВЕЛИЧИВАЕТСЯ в течение цикла до тех пор, пока не будут взяты все
            // дочерние трансформы всех уровней
            for (int i = 0; i < output.Count; i++)
            {
                Transform child = output[i];
                List<Transform> granchildren = child.CollectChildFirstLevelTransforms();
                if (granchildren.Count != 0) { output.AddRange(granchildren); }
            }

            return output;
        }

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
    }
}

