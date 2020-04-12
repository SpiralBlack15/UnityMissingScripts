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
        /// GUID скрипта, если есть
        /// </summary>
        public string guid { get; private set; } = "";

        /// <summary>
        /// GID компонента (позволяет однозначно идентифицировать экземпляр 
        /// компонента в файле сцены)
        /// </summary>
        public ulong gid { get { return goid.targetObjectId; } }

        public ComponentID(Component comp)
        {
            component = comp;
            goid = GlobalObjectId.GetGlobalObjectIdSlow(comp);
            
            // пациент скорее жив, чем мёртв?
            if (alive)
            {
                type = component.GetType();
                metadataToken = type.MetadataToken;
                var serComp = new SerializedObject(comp);
                mScript = serComp.FindProperty(SceneFile.unitMonoScriptField);
                if (mScript != null)
                {
                    guid = SceneFile.current.GetGUID(gid, false);
                }
            }
            else // пациент мёртв, мы нашли битый скрипт
            {
                type = null;
                metadataToken = -1;
                mScript = null;
            }
        }

        /// <summary>
        /// Получить список компонент на объекте
        /// </summary>
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
