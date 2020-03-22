using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    /// <summary>
    /// Идентификацинный номер компонента в файле сцены
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
        public GlobalObjectId gid { get; }

        /// <summary>
        /// Тип компонента
        /// </summary>
        public Type type { get; } = null;

        /// <summary>
        /// Сериализованное свойство, соотнесённое с компонентом
        /// </summary>
        public SerializedProperty monoScript { get; } = null;

        /// <summary>
        /// Метадата токен, если понадобится
        /// </summary>
        public int metadataToken { get; } = 0;

        /// <summary>
        /// Компонент живой (т.е. не является missing script)
        /// </summary>
        public bool alive { get { return component != null; } }

        /// <summary>
        /// ID компонента
        /// </summary>
        public ulong id { get { return gid.targetObjectId; } }

        public ComponentID(Component comp)
        {
            component = comp;
            gid = GlobalObjectId.GetGlobalObjectIdSlow(comp);

            // пациент скорее жив, чем мёртв?
            if (alive)
            {
                // уточняем тип живого скрипта
                type = component.GetType();
                metadataToken = type.MetadataToken;

                // проверяем, что это MonoBehaviour, ищем у него поле m_Script
                var serComp = new SerializedObject(comp);
                monoScript = serComp.FindProperty(SceneFile.unitMonoScriptField);
            }
            else // пациент однозначно мёртв, мы нашли битый скрипт
            {
                type = null;
                metadataToken = -1;
                monoScript = null;
            }
        }

        /// <summary>
        /// Получить список 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<ComponentID> GetComponentIDs(GameObject obj)
        {
            Component[] components = obj.GetComponents<Component>();
            List<ComponentID> cids = new List<ComponentID>();
            for (int i = 0; i < components.Length; i++)
            {
                ComponentID cid = new ComponentID(components[i]);
                cids.Add(cid);
            }
            return cids;
        }
    }
}
#endif
