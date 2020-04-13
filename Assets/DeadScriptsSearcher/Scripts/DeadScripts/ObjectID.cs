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
        public List<ulong> liveComponentFileIDs { get; } = new List<ulong>();

        /// <summary>
        /// Статус компонента из листа LiveComponentFileIDs
        /// </summary>
        public List<bool> componentLiveStatus { get; } = new List<bool>();

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
        public List<ulong> deadComponentFileIDs { get; } = new List<ulong>();

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
                // TODO: сейчас CID формируется здесь, имеет смысл совместить с тем, что дальше
                ComponentID cid = new ComponentID(components[i]); 
                componentIDs.Add(cid);
            }
            if (debugMode)
            {
                Debug.Log($"<b>GameObject</b>: <color=blue>{obj.name}</color>; " +
                          $"FileID: <color=blue>{fileID}</color>; " + 
                          $"Scripts Count: <color=blue>{componentIDs.Count}</color>");
                // fileID у нас есть автоматом после того, как мы получили globalID
                // если fileID == 0, то у нас какие-то проблемы с globalID
            }

            // Попытка взять все компонент ID соответственно файлу сцены
            // Если объект находится в составе префаба и не является корнем префаба,
            // то попытка может провалиться, и тогда лист будет заполняться по ходу следующей
            // процедуры, а вместо битых скриптов будут обозначены нули.
            // Обратите внимание, что в файле сцены все FileID прикреплённых скриптов идут в том же
            // порядке, что и компоненты, взятые через GetComponents<>!
            var (fileIDs, strIDX) = SceneFile.current.GetAllComponentsFileIDs(fileID, debugMode);
            componentFileIDs = fileIDs;
            fileCaptionStringIDX = strIDX;

            // FileID могут отсутствовать по следующим причинам:
            // - объект находится в составе префаба
            // - сцена не была сохранена
            bool noFileIDFound = componentFileIDs == null;
            if (noFileIDFound)
            {
                if (debugMode) DebugObjectNotFound();
                componentFileIDs = new List<ulong>();
            }

            // WARNING : костыль из костылей, стоит потом весь этот функционал аккуратно запихать в ComponentID
            for (int i = 0; i < componentIDs.Count; i++)
            {
                ComponentID cid = componentIDs[i];

                if (cid.alive) // CID соответствует "живому" скрипту
                {
                    ulong fileID = cid.fileID;
                    liveComponentFileIDs.Add(fileID);
                    if (noFileIDFound) componentFileIDs.Add(fileID);

                    // мы добавляем статусы чтобы избежать ненужных медленных проверок в DeadScripts
                    componentLiveStatus.Add(true);
                }
                else // CID соответствует "мёртвому" скрипту
                {
                    if (debugMode)
                    {
                        Debug.Log($"<color=red><b>Object</b> <i>{obj}</i>: {strDebug_DeadScriptAtThePosition} #{i}</color>");
                    }
                    missingScriptsCount++; // чтобы лишний раз не вызывать встроенную функцию, считающую количество мёртвых скриптов

                    // добавляем 0 для случая префабов и пр., когда мы никаким образом не сможем восстановить FileID
                    if (noFileIDFound) componentFileIDs.Add(0);

                    // если FileID != 0 (см. выше), то он будет уже взят ранее, из перечисления с файла объекта!
                    deadComponentFileIDs.Add(componentFileIDs[i]);

                    componentLiveStatus.Add(false);
                }

                // FileID мы только что получили (см. выше)
                ulong currentFileID = componentFileIDs[i];
                if (currentFileID != 0)
                {
                    // Попытка взять GUID производится только в этом случае, 
                    // потому что иначе мы в принципе не можем взять GUID никакими доступными путями
                    // (кроме того, что это, вероятно, не имеет смысла, если у нас FileID == 0)
                    string guid = SceneFile.current.GetComponentGUID(currentFileID, debugMode);

                    // GUID не был найден в файле сцены. Это может произойти, если вы не сохранили сцену после изменений,
                    // если файл сцены повреждён. Также это происходит для скриптов, не имеющих поля m_Script в файле 
                    // сцены. Это скрипты вроде скрипта камеры, трансформа и т.п.
                    if (string.IsNullOrEmpty(guid) && debugMode) Debug.Log($"FileID: {currentFileID}; GUID not found");
                    componentGUIDs.Add(guid);
                }
                else
                {
                    if (debugMode) Debug.Log($"Cannot take GUID for zero FileID");
                }

                // Собственно, здесь вывод саммари по этому объекту
                if (debugMode)
                {
                    string dbgObjID = (cid.fileID == 0) ? $"<color=red>0</color>" : $"<color=blue>{cid.fileID}</color>";
                    string dbgScriptType, dbgConclusion;
                    if (cid.alive)
                    {
                        dbgScriptType = $"<color=blue>{cid.type}</color> with GUID: [<color=blue>{cid.guid}</color]";
                        dbgConclusion = (cid.mScript != null) ? $"<color=grey>MONO BEHVAIOUR</color>" : $"<color=grey>NOT MONO BEHVAIOUR</color>";
                    }
                    else
                    {
                        dbgScriptType = $"<color=grey><b>NULL</b></color>";
                        dbgConclusion = $"<color=grey><b>BROKEN</b></color>";
                    }
                    Debug.Log($"Component {i} with FileID: {dbgObjID}; Type: {dbgScriptType}; Conclusion: {dbgConclusion}");
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

