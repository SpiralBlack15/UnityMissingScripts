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
        /// FileID, с помощью которого мы можем найти объект в файле сцены
        /// </summary>
        public ulong fileID { get { return globalID.targetObjectId; } }

        /// <summary>
        /// Номер строки с заголовком объекта в файле сцены.
        /// Валидно только на момент создания ObjectID (при условии, что вы обновили файл сцены,
        /// прежде чем запускать поиск).
        /// Если -1 - была попытка создать ObjectID пустого файла.
        /// Если -2 - объект, по всей видимости, является частью префабы
        /// </summary>
        public int fileCaptionStringIDX { get; }

        /// <summary>
        /// Является ли объект префабом (для этого надо брать объект не со сцены, а из Assets!).
        /// Это нужно, чтобы скипать проверку для таких объектов, если понадобится.
        /// </summary>
        public bool isPrefab { get { return gameObject.scene.rootCount == 0; } }

        /// <summary>
        /// Является частью префаба
        /// </summary>
        public bool isPartOfPrefab { get { return PrefabUtility.IsPartOfAnyPrefab(gameObject); } }

        /// <summary>
        /// FileID живых компонентов
        /// </summary>
        public List<ulong> liveComponentIDs { get; } = new List<ulong>();

        /// <summary>
        /// Все FileID компонентов (если удалось выудить)
        /// </summary>
        public List<ulong> componentFileIDs { get; }

        /// <summary>
        /// Все GUID's компонентов, которые удалось найти
        /// </summary>
        public List<string> componentGUIDs { get; } = new List<string>();

        /// <summary>
        /// FileID мёртвых компонентов (если удалось идентифицировать)
        /// </summary>
        public List<ulong> deadComponentIDs { get; } = new List<ulong>();

        /// <summary>
        /// Количество мёртвых криптов на инспектируемом объекте
        /// </summary>
        public int missingScriptsCount { get; } = 0;

        /// <summary>
        /// Флаг для EditorWindow
        /// </summary>
        public bool showInfo { get; set; } = false;

        /// <summary>
        /// Получить информацию об этом объекте, чтобы найти его в файле сцены
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="debugMode">Использовать режим отладки (режим отладки может существено замедлить обыск сцены!)</param>
        public ObjectID(GameObject obj, bool debugMode = false)
        {
            gameObject = obj;
            globalID = GlobalObjectId.GetGlobalObjectIdSlow(obj);

            // формируем список ComponentID, по которым мы сможем пройтись позднее
            componentIDs = new List<ComponentID>();
            Component[] components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                ComponentID cid = new ComponentID(components[i]);
                componentIDs.Add(cid);
            }

            // попытка взять все компонент ID соответственно файлу сцены
            // если объект находится в составе префаба и не является корнем префаба,
            // то попытка может провалиться, и тогда лист будет заполняться по ходу следующей
            // процедуры, а вместо битых скриптов будут обозначены нули
            var (fileIDs, strIDX) = SceneFile.current.GetAllComponentsFileIDs(fileID, debugMode);
            componentFileIDs = fileIDs;
            fileCaptionStringIDX = strIDX;
            bool noMainList = componentFileIDs == null;
            if (noMainList) componentFileIDs = new List<ulong>();

            if (debugMode)
            {
                Debug.Log($"<b>Object</b>: <color=blue>{obj.name}</color>; " +
                          $"Global ID: <color=blue>{globalID.targetObjectId}</color>; " +
                          $"Scripts Count: <color=blue>{componentIDs.Count}</color>");
            }

            // WARNING : костыль из костылей, стоит потом весь этот функционал аккуратно запихать в ComponentID
            for (int i = 0; i < componentIDs.Count; i++)
            {
                ComponentID cid = componentIDs[i];

                if (cid.alive)
                {
                    ulong fileID = cid.fileID;
                    liveComponentIDs.Add(fileID);
                    if (noMainList) componentFileIDs.Add(fileID);
                }
                else
                {
                    if (debugMode)
                    {
                        Debug.Log($"<color=red><b>Object</b> <i>{obj}</i>: {strDebug_DeadScriptAtThePosition} #{i}</color>");
                    }
                    missingScriptsCount++;
                    if (noMainList) componentFileIDs.Add(0);
                    deadComponentIDs.Add(componentFileIDs[i]); // по идее не должно вызывать сбоев
                }

                ulong currentFileID = componentFileIDs[i];
                if (currentFileID != 0)
                {
                    string guid = SceneFile.current.GetComponentGUID(currentFileID, debugMode);
                    componentGUIDs.Add(guid);
                }

                if (debugMode)
                {
                    string dbgObjID = (cid.fileID == 0) ? $"<color=red>0</color>" :  $"<color=blue>{cid.fileID}</color>";
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
            string messageNoObject = $"<color=brown>Object ID <b>{fileID}</b> {strDebug_ObjectIDNotFound};</color> ";
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

