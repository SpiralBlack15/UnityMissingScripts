using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    /// <summary>
    /// Информация об объекте и висящем на нём скриптах
    /// </summary>
    public class ObjectID
    {
        /// <summary>
        /// Привязанный объект
        /// </summary>
        public GameObject gameObject { get; }

        /// <summary>
        /// Global ID объекта
        /// </summary>
        public GlobalObjectId globalID { get; }

        /// <summary>
        /// Все прикреплённые компоненты, включая мёртвые!
        /// </summary>
        public List<ComponentID> componentIDs { get; }

        /// <summary>
        /// ID, с помощью которого мы можем найти объект в файле сцены!
        /// </summary>
        public ulong id { get { return globalID.targetObjectId; } }

        /// <summary>
        /// Живые ID компонентов
        /// </summary>
        public List<ulong> liveIDs { get; } = new List<ulong>();

        /// <summary>
        /// Мёртвые скрипты
        /// </summary>
        public int missingScriptsCount { get; } = 0;

        /// <summary>
        /// Получить информацию об этом объекте, чтобы найти его в файле сцен
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="dbg">Использовать отладку</param>
        public ObjectID(GameObject obj, bool dbg = false)
        {
            gameObject = obj;
            globalID = GlobalObjectId.GetGlobalObjectIdSlow(obj);
            componentIDs = ComponentID.GetComponentIDs(obj);

            if (dbg)
            {
                Debug.Log($"<b>Object</b>: <color=blue>{obj.name}</color>; " +
                          $"Global ID: <color=blue>{globalID.targetObjectId}</color>; " +
                          $"Scripts Count: <color=blue>{componentIDs.Count}</color>");
            }

            for (int i = 0; i < componentIDs.Count; i++)
            {
                ComponentID cid = componentIDs[i];

                if (cid.alive)
                {
                    ulong id = cid.id;
                    liveIDs.Add(id); // сохраняем живой ID
                }
                else
                {
                    if (dbg)
                    {
                        Debug.Log($"<color=red><b>Object</b> <i>{obj}</i>: dead script detected at the position #{i}</color>");
                    }
                    missingScriptsCount++;
                }

                if (dbg)
                {
                    string dbgObjID = (cid.id == 0) ?
                                       $"<color=red>{cid.id}</color>" :
                                    
                                       $"<color=blue>{cid.id}</color>";
                    string dbgScriptType;
                    string dbgConclusion;

                    if (cid.alive)
                    {
                        dbgScriptType = $"<color=blue>{cid.type}</color> with metadata token: " +
                                        $"<color=blue>{cid.metadataToken}</color>";
                        dbgConclusion = (cid.monoScript != null) ? 
                                        $"<color=grey>MONO</color>" : 
                                        $"<color=grey>NOT MONO</color>";
                    }
                    else
                    {
                        dbgScriptType = $"<color=grey><b>NULL</b></color>";
                        dbgConclusion = $"<color=grey><b>BROKEN</b></color>";
                    }
                    Debug.Log($"Component {i} with ID: {dbgObjID}; Type: {dbgScriptType}; Is {dbgConclusion}");
                }
            }
        }

        public void DebugObjectNotFound() // объекта нет в файле сцены (возможно, это префаб?)
        {
            string messageNoObject = $"<color=brown>Object ID <b>{id}</b> does not found in the Scene file;</color> ";
            messageNoObject += (globalID.targetPrefabId == 0) ? SceneFile.messageSaveSceneWarning : "[Prefab]";
            Debug.Log(messageNoObject);
        }
    }
}
#endif

