using System.Collections.Generic;
using UnityEngine;
using static Spiral.EditorTools.DeadScriptsSearcher.Localization;

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
        /// Все прикреплённые компоненты, включая мёртвые
        /// </summary>
        public List<ComponentID> componentIDs { get; }

        /// <summary>
        /// ID, с помощью которого мы можем найти объект в файле сцены
        /// </summary>
        public ulong id { get { return globalID.targetObjectId; } }

        /// <summary>
        /// Живые ID компонентов
        /// </summary>
        public List<ulong> liveIDs { get; } = new List<ulong>();

        /// <summary>
        /// Количество мёртвых криптов на инспектируемом объекте
        /// </summary>
        public int missingScriptsCount { get; } = 0;

        /// <summary>
        /// Флаг для EditorWindow
        /// </summary>
        public bool showInfo { get; set; } = false;

        /// <summary>
        /// Получить информацию об этом объекте, чтобы найти его в файле сцен
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="debugMode">Использовать режим отладки (режим отладки может существено замедлить обыск сцены!)</param>
        public ObjectID(GameObject obj, bool debugMode = false)
        {
            gameObject = obj;
            globalID = GlobalObjectId.GetGlobalObjectIdSlow(obj);
            componentIDs = ComponentID.GetComponentIDs(obj);

            if (debugMode)
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
                    ulong id = cid.gid;
                    liveIDs.Add(id);
                }
                else
                {
                    if (debugMode)
                    {
                        Debug.Log($"<color=red><b>Object</b> <i>{obj}</i>: {strDebug_DeadScriptAtThePosition} #{i}</color>");
                    }
                    missingScriptsCount++;
                }

                if (debugMode)
                {
                    string dbgObjID = (cid.gid == 0) ? $"<color=red>0</color>" :  $"<color=blue>{cid.gid}</color>";
                    string dbgScriptType, dbgConclusion;
                    if (cid.alive)
                    {
                        dbgScriptType = $"<color=blue>{cid.type}</color> with metadata token: " +
                                        $"<color=blue>{cid.metadataToken}</color>";
                        dbgConclusion = (cid.mScript != null) ? 
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

        /// <summary>
        /// Оповещение в консоль о том, что ObjectID не был найден в файле сцены. Две основные причины, почему 
        /// это может произойти: 1. сцена повреждена (или не была сохранена); 2. объект находится в составе 
        /// префаба.
        /// </summary>
        public void DebugObjectNotFound()
        {
            string messageNoObject = $"<color=brown>Object ID <b>{id}</b> {strDebug_ObjectIDNotFound};</color> ";
            messageNoObject += (globalID.targetPrefabId == 0) ? $"<color=red>{strDebug_ObjectIDNotFound}</color>" : "[Prefab]";
            Debug.Log(messageNoObject);
        }

        /// <summary>
        /// Выделить указанные объекты на сцене
        /// </summary>
        /// <param name="oids">Учётки объектов</param>
        public static void Select(List<ObjectID> oids)
        {
            List<GameObject> selectObjects = new List<GameObject>();
            for (int i = 0; i < oids.Count; i++)
            {
                if (oids[i].gameObject == null) continue;
                selectObjects.Add(oids[i].gameObject);
            }
            Selection.objects = selectObjects.ToArray();
        }
    }
}
#endif

