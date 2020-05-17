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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorToolkit
{
    public static class PropertyTools
    {
        // TODO: не всегда хорошо работает с Unity Fake Null, могут быть проблемы при наличии null-UnityObject
        // TODO: в пути сериализации, а не в его непосредственном корне

        /// <summary>
        /// Возвращает названия узлов сериализации.
        /// Обратите внимание, длина сериализации в ОДИН узел (НЕ в ноль)
        /// означает, что объект принадлежит непосредственно серилизованному объекту Unity, а 
        /// не является, например, вложенным в лист, в структуру, в сериализованный
        /// класс или вообще какую-то особо хтоническую нечисть.
        /// </summary>
        /// <param name="property">Сериализуемое свойство</param>
        /// <returns>Массив строк в порядке сериализации от несущего объекта 
        /// (как правило, компонента, окна редктора и т.п., а не GameObject'a)
        /// к сериализовнному свойству</returns>
        public static string[] GetPathNodes(this SerializedProperty property)
        {
            return property.propertyPath.Split('.');
        }

        /// <summary>
        /// Сразу берёт корень сериализации, для особо ленивых
        /// </summary>
        /// <param name="property">Сериализуемое свойство</param>
        /// <returns>Корневой объект сериализации, очевидно.</returns>
        public static UnityEngine.Object GetRootParent(this SerializedProperty property)
        {
            return property.serializedObject.targetObject;
        }

        /// <summary>
        /// Берёт объект, ассоциированный с данным сериализуемым свойством.
        /// Данная функция нужна для того чтобы выудить объект, не являющийся наследником от UnityEngine.Object,
        /// из сериализованного свойства. Функция использует рефлексию, очень медленная и может не всегда корректно
        /// работать со вложенными массивами и вложенными же свойствами. На свой страх и риск.
        /// </summary>
        /// <param name="property">Сериализуемое свойство</param>
        /// <param name="inverseIDX">Индекс в пути в обратном порядке. 0 вернёт сам объект, 1 - его ближайшего родителя и т.п.</param>
        /// <returns>Объект, соотнесённый с сериализованным свойством</returns>
        public static object GetPropertyObject(this SerializedProperty property, int inverseIDX = 0)
        {
            string[] path = property.propertyPath.Split('.');
            UnityEngine.Object serializationRoot = property.serializedObject.targetObject;
            return GetPathNode(path, serializationRoot, inverseIDX);
        }

        /// <summary>
        /// Берёт объект родительского свойства (по умолчанию - первое за серилизованным свойство).
        /// </summary>
        /// <param name="property">Сериализованное свойство</param>
        /// <param name="parentIDX">Индекс родителя относительно свойства</param>
        /// <param name="ignoreArrays">Автоматом пропускать массивы</param>
        /// <returns>
        /// Возвращает родителя соответственно индексу родителя относительно сериализованного свойства
        /// в обратном порядке пути сериализации. Так, parentIDX = 0 вернёт непосредственного родителя
        /// свойства, 1 - второго родителя в иерархии, и так до корня сериализации.
        /// Если parentIDX = -1, вернёт сам объект сериализованного свойства. 
        /// Если parentIDX < -1, равен или превышает длину пути сериализции, вернёт корень сериализации.
        /// Если сериализованное свойство принадлежит непосредственно компоненту (окну редактора и т.п.),
        /// будет возвращён корен сериализации.</returns>
        public static object GetPropertyParent(this SerializedProperty property, int parentIDX = 0, bool ignoreArrays = true)
        {
            string[] path = property.propertyPath.Split('.');
            UnityEngine.Object serializationRoot = property.serializedObject.targetObject;
            return GetPathNode(path, serializationRoot, parentIDX + 1, ignoreArrays);
        }

        public static List<object> GetSerializationHierarchy(this SerializedProperty property, bool ignoreArrays = true)
        {
            string[] path = property.propertyPath.Split('.');
            UnityEngine.Object serializationRoot = property.serializedObject.targetObject;
            return GetAllPathObjects(path, serializationRoot, ignoreArrays);
        }

        /// <summary>
        /// Берёт объект с указанного узла в пути сериализации.
        /// </summary>
        /// <param name="path">Полный путь сериализации от корня к сериализованному свойству</param>
        /// <param name="serializationRoot">Корень сериализации</param>
        /// <param name="inverseNodeIDX">Индекс узла с конца. 0 соответствует самому объекту</param>
        /// <returns>Объект, на который ссылается указанный узел сериализации</returns>
        public static object GetPathNode(string[] path, UnityEngine.Object serializationRoot, int inverseNodeIDX = 0, bool ignoreArrays = false)
        {
            int pathDepth = path.Length;
            if (pathDepth == 0) throw new ArgumentException("Broken path");
            if (serializationRoot == null) throw new ArgumentException("Serialization root cannot be null");

            if (inverseNodeIDX == 0) // ищем сам объект
            {
                return GetPathNodeUnsafe(path, serializationRoot, pathDepth);
            }

            // поскольку мы не можем искать по несуществующим узлм
            if (inverseNodeIDX < 0) return serializationRoot;

            // ищем свойство, следующее за нами
            int searchDepth = pathDepth - inverseNodeIDX; 
            if (searchDepth <= 0) return serializationRoot;

            // если включен игнор листов
            if (ignoreArrays)
            {
                searchDepth = GetDepthExcludeArraysUnsafe(path, searchDepth);
                if (searchDepth < 0) return serializationRoot;
            }

            return GetPathNodeUnsafe(path, serializationRoot, searchDepth); // TODO: тут немного сомнительный кд
        }

        /// <summary>
        /// Возвращает индекс в обратном порядке от индекса поиска,
        /// автоматом пропуская массивы и обращаясь ко следующему свойству.
        /// </summary>
        /// <param name="path">Полный путь сериализации от корня к сериализованному свойству</param>
        /// <param name="directSearchDepth">Исходная глубина поиска</param>
        /// <returns>Следующий в обратном порядке индекс узла, не содержащего массивы, листы и т.п.</returns>
        private static int GetDepthExcludeArraysUnsafe(string[] path, int directSearchDepth)
        {
            int startFrom = directSearchDepth - 1;
            for (int i = startFrom; i >= 0; i--)
            {
                string node = path[i];
                if (node != "Array") return i;
            }
            return -1;
        }

        /// <summary>
        /// Берёт все объекты в цепочке сериализации
        /// </summary>
        /// <param name="path">Путь</param>
        /// <param name="serializationRoot">Корень сериализации</param>
        /// <param name="ignoreArrays">Игнорировать массивы</param>
        /// <returns></returns>
        public static List<object> GetAllPathObjects(string[] path, UnityEngine.Object serializationRoot, bool ignoreArrays = true)
        {
            List<object> output = new List<object>();
            object currentObject = serializationRoot;
            output.Add(currentObject);

            for (int i = 0; i < path.Length; i++)
            {
                string pathNode = path[i];
                Type objectType = currentObject.GetType();
                FieldInfo objectFieldInfo = objectType.GetField(pathNode);

                if (objectFieldInfo == null)
                {
                    if (pathNode == "Array")
                    {
                        i++;
                        string nextNode = path[i];
                        string idxstr = nextNode.Substring(nextNode.IndexOf("[") + 1);
                        idxstr = idxstr.Replace("]", "");
                        int arrayNumber = Convert.ToInt32(idxstr);
                        IList collection = currentObject as IList;
                        if (collection.Count == 0 || collection.Count <= arrayNumber)
                        {
                            output.Add(null);
                            break;
                        }
                        currentObject = collection[arrayNumber];
                    }
                    else 
                    {
                        throw new NotImplementedException("Данный случай не обрабатывается");
                    }
                }
                else 
                {
                    object nextObject = objectFieldInfo.GetValue(currentObject);
                    if (nextObject == null)
                    {
                        output.Add(null);
                        break;
                    }
                    currentObject = nextObject;
                }

                if (ignoreArrays)
                {
                    if (currentObject is IList) continue;
                }
                output.Add(currentObject);
            }

            return output;
        }

        /// <summary>
        /// Берёт объект из заданной ячейки сериализации
        /// </summary>
        /// <param name="path">ПОЛНЫЙ путь сериализации от корня к сериализованному свойству</param>
        /// <param name="serializationRoot">Корневой объект сериализации</param>
        /// <param name="directSearchDepth"></param>
        /// <returns></returns>
        private static object GetPathNodeUnsafe(string[] path, UnityEngine.Object serializationRoot, int directSearchDepth)
        {
            object currentObject = serializationRoot;

            if (directSearchDepth == 0)
            {
                string pathNode = path[0];
                Type objectType = currentObject.GetType();
                FieldInfo objectFieldInfo = objectType.GetField(pathNode);
                return objectFieldInfo.GetValue(currentObject);
            }

            for (int i = 0; i < directSearchDepth; i++)
            {
                string pathNode = path[i];
                Type objectType = currentObject.GetType();
                FieldInfo objectFieldInfo = objectType.GetField(pathNode);

                // Данный случай может возникнуть, если мы впаялись в array, а не в другой object
                if (objectFieldInfo == null)
                {
                    if (pathNode == "Array")
                    {
                        i++;
                        string nextNode = path[i];
                        string idxstr = nextNode.Substring(nextNode.IndexOf("[") + 1);
                        idxstr = idxstr.Replace("]", "");
                        int arrayNumber = Convert.ToInt32(idxstr);
                        IList collection = currentObject as IList; // C# Array всегда реализует IList
                        if (collection.Count == 0) return null;
                        if (collection.Count <= arrayNumber) return null; // ...или если мы расширяемся
                        currentObject = collection[arrayNumber]; // после чего идём дальше, там может быть и более глубокая вложенность
                    }
                    else // на случай, если придётся ещё какое исключение обрабатывать
                    {
                        throw new NotImplementedException("Данный случай не обрабатывается");
                    }
                }
                else // штатный режим, перебираем объекты в иерархии дальше
                {
                    object nextObject = objectFieldInfo.GetValue(currentObject);
                    if (nextObject == null) return null; // может случаться при только что созданном пустом объекте любого класса в инспекторе
                    currentObject = nextObject;
                }
            }
            return currentObject;
        }
    }
}
#endif
