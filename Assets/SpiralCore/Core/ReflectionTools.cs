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
using System.Linq;
using System.Reflection;

namespace Spiral.Core
{
    public static class ReflectionTools
    {
        /// <summary>
        /// Преобразует лист класса-наследника от к листу класса-родителя
        /// </summary>
        /// <typeparam name="ParentType">Родителський тип</typeparam>
        /// <typeparam name="ChildType">Дочерний тип</typeparam>
        /// <param name="childTypeList">Класс дочернего типа</param>
        /// <returns></returns>
        public static List<ParentType> ToParentType<ParentType, ChildType>(this IList<ChildType> childTypeList)
            where ChildType : ParentType
            where ParentType : class
        {
            List<ParentType> parentTypeList = new List<ParentType>();
            int count = childTypeList.Count;
            for (int i = 0; i < count; i++)
            {
                ParentType parentTypeObject = childTypeList[i] as ParentType;
                parentTypeList.Add(parentTypeObject);
            }
            return parentTypeList;
        }

        /// <summary>
        /// Взять дочерние классы этого класса (использует рефлексию)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static List<Type> GetChildTypes(this Type target)
        {
            IEnumerable<Type> list = Assembly.GetAssembly(target).GetTypes().Where(type => type.IsSubclassOf(target));
            return new List<Type>(list);
        }

        public static List<MethodInfo> GetEveryMethodInfo(this Type type)
        {
            // может взять далеко не все классы, игнорит перегрузки
            var methods = type.GetMethods(BindingFlags.Public |
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance |
                                          BindingFlags.Static | 
                                          BindingFlags.DeclaredOnly);
            return Enumerable.ToList(methods);
        }

        public static List<string> GetCaptions(this List<MethodInfo> methods)
        {
            // может взять далеко не все классы, игнорит перегрузки
            List<string> answer = new List<string>();
            for (int i = 0; i < methods.Count; i++)
            {
                var method = methods[i];
                var parameters = method.GetParameters();
                string methodstr = $"{method.Name}"; 
                string paramstr = "";
                for (int j = 0; j < parameters.Length; j++)
                {
                    if (j != 0) paramstr += ", ";
                    paramstr += $"{parameters[j].ParameterType.Name} {parameters[j].Name}";
                }
                methodstr += $"({paramstr})";
                answer.Add(methodstr);
            }
            return answer;
        }
    }
}

