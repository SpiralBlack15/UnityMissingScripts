// *********************************************************************************
// The MIT License (MIT)
// Copyright (c) 2020 BlackSpiral https://github.com/BlackSpiral15
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    /// <summary>
    /// Данные о конкретно взятом экземпляре компонента в сцене
    /// </summary>
    public class ComponentID
    {
        /// <summary>
        /// Компонент, из которого извлечён ID
        /// </summary>
        public Component component { get; }

        /// <summary>
        /// Global Object ID компонента
        /// </summary>
        public GlobalObjectId goid { get; }

        /// <summary>
        /// Тип компонента (то же, что и GetType())
        /// </summary>
        public Type type { get; } = null;

        /// <summary>
        /// Соотнесённое с компонентом сериализованное свойство.
        /// Это поле может быть равно null для части живых скриптов,
        /// поскольку у них отсутствует поле m_Script (трансформы, камеры и т.п.).
        /// Чтобы определить, живой компонент или нет, достаточно ориентироваться на component != null:
        /// учётки мёртвых компонентов будут выглядеть одинаково в любом случае,
        /// а для живых они выполняют исключительно информационную функцию.
        /// </summary>
        public SerializedProperty mScript { get; } = null;

        /// <summary>
        /// Метадата токен, если понадобится
        /// </summary>
        public int metadataToken { get; } = 0;

        /// <summary>
        /// Компонент живой (не является missing script)
        /// </summary>
        public bool alive { get { return component != null; } }

        /// <summary>
        /// GUID скрипта, если есть.
        /// У таких компонент как Transform, Camera и т.п. GUID'a нет!
        /// </summary>
        public string guid { get; private set; } = "";

        /// <summary>
        /// FileID компонента (позволяет однозначно идентифицировать экземпляр 
        /// компонента в файле сцены)
        /// </summary>
        public ulong fileID { get { return goid.targetObjectId; } }

        public ComponentID(Component comp)
        {
            component = comp;
            goid = GlobalObjectId.GetGlobalObjectIdSlow(comp);
            
            if (alive) // пациент скорее жив, чем мёртв?
            {
                type = component.GetType();
                metadataToken = type.MetadataToken;
                var serComp = new SerializedObject(comp);
                mScript = serComp.FindProperty(SceneFile.unitMonoScriptField);
                if (mScript != null)
                {
                    guid = SceneFile.current.GetComponentGUID(fileID, false);
                }
            }
            else // пациент мёртв, мы нашли битый скрипт
            {
                type = null;
                metadataToken = -1;
                mScript = null;
            }
        }
    }
}
#endif
