using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Spiral.Core
{
    public static class Misc
    {
        /// <summary>
        /// Апнуть регистр первой буквы в строке
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Строка с апнутым регистром первой буквы, если есть</returns>
        public static string FirstLetterCapitalization(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (str.Length == 1) return str.ToUpper();
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Лист и/или массив в текст. 
        /// </summary>
        /// <param name="entry">Лист или массив</param>
        /// <returns>Текст</returns>
        public static string GetText(this IList<string> entry)
        {
            StringBuilder stringBuilder = new StringBuilder("");

            int count = entry.Count;
            for (int e = 0; e < count; e++)
            {
                stringBuilder.Append(entry[e]);
                if (e != count - 1) stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Любой массив (да, даже массив, см. как они реализованы) в лист
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IList<T> ts) where T : class
        {
            if (ts == null) return new List<T>();
            if (ts.Count == 0) return new List<T>();
            return new List<T>(ts);
        }

        /// <summary>
        /// Шорткат для взятия последнего элемента (для ленивых)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static T GetLast<T>(this IList<T> ts)
        {
            return ts[ts.Count - 1];
        }

        /// <summary>
        /// Шорткат для удаления последнего элемента (для ленивых)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        public static void RemoveLast<T>(this IList<T> ts)
        {
            ts.RemoveAt(ts.Count - 1);
        }
    }
}

