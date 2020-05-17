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

namespace Spiral.Core
{
    public static class EnumTools
    {
        public const string msgNotEnum = "Не является перечисляемым типом!";

        /// <summary>
        /// Взять Enum именами
        /// </summary>
        /// <param name="someEnum"></param>
        /// <returns></returns>
        public static List<string> GetEnumNamespace(this Enum someEnum)
        {
            Type T = someEnum.GetType();
            var names = Enum.GetNames(T);
            var answer = new List<string>();
            answer.AddRange(names);
            return answer;
        }

        /// <summary>
        /// Взять Enum именами
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetEnumNamespace<T>() where T : Enum
        {
            var names = Enum.GetNames(typeof(T));
            var answer = new List<string>();
            answer.AddRange(names);
            return answer;
        }

        /// <summary>
        /// Взять Enum именами
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<string> GetEnumNamespace(Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentException("Не является перечисляемым типом");
            var names = Enum.GetNames(enumType);
            var answer = new List<string>();
            answer.AddRange(names);
            return answer;
        }

        /// <summary>
        /// Взять название Enum'a у объекта его типа
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumObjectName<T>(this T enumObj) where T : Enum
        {
            Type type = enumObj.GetType();
            return Enum.GetName(type, enumObj);
        }

        public static List<string> GetEnumNames(this Enum someEnum)
        {
            Type T = someEnum.GetType();
            var names = Enum.GetNames(T);
            var answer = new List<string>();
            answer.AddRange(names);
            return answer;
        }

        public static List<string> GetEnumNames<T>() where T : Enum
        {
            var names = Enum.GetNames(typeof(T));
            var answer = new List<string>();
            answer.AddRange(names);
            return answer;
        }

        public static List<string> GetEnumNames(Type enumType)
        {
            if (!enumType.IsEnum) return null;
            var names = Enum.GetNames(enumType);
            var answer = new List<string>();
            answer.AddRange(names);
            return answer;
        }

        public static List<T> GetEnumAsList<T>() where T : Enum
        {
            List<T> answer = new List<T>();
            var values = Enum.GetValues(typeof(T));
            for (int i = 0; i < values.Length; i++)
            {
                object smth = values.GetValue(i);
                var value = (T)smth;
                answer.Add(value);
            }
            return answer;
        }

        public static string GetEnumName(object obj)
        {
            Type type = obj.GetType();

            if (!type.IsEnum) throw new ArgumentException(msgNotEnum);

            return Enum.GetName(type, obj);
        }

        public static T SelectEnumByName<T>(string str) where T : Enum
        {
            List<string> names = GetEnumNames<T>();
            int idx = names.FindIndex(x => x == str);
            var values = Enum.GetValues(typeof(T));

            if (idx >= 0)
            {
                var outputIDX = values.GetValue(idx);
                return (T)outputIDX;
            }
            else
            {
                return (T)values.GetValue(0);
            }
        }
    }
}
