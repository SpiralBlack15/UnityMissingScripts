using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Spiral.EditorTools.DeadScriptsSearcher.Localization;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class DeadScripts
    {
        public static DeadScripts instance { get; } = new DeadScripts() { debug = false };

        public bool debug = true;
        public List<ObjectID> deadOIDs  { get; private set; } = new List<ObjectID>();
        public List<ScriptGUID> deadGUIDs { get; private set; } = new List<ScriptGUID>();
        private SceneFile sceneFile = null;

        // PROPERTIES -----------------------------------------------------------------------------
        public bool sceneFileLoaded
        {
            get
            {
                if (sceneFile == null) return false;
                if (sceneFile.count == 0) return false;
                return true;
            }
        }

        public bool isDirty { get { return SceneManager.GetActiveScene().isDirty; } }

        // FUNCTIONALITY --------------------------------------------------------------------------
        public void SelectDeads()
        {
            ObjectID.Select(deadOIDs);
        }

        public void UpdateDeadList()
        {
            // Собираем все объекты на сцене
            var objects = CoreFunctions.CollectScene().Transforms2GameObjects();
            int count = objects.Count;

            // Чистим лист объектов с мёртвыми скриптами
            if (deadOIDs == null) deadOIDs = new List<ObjectID>();
            else if (deadOIDs.Count != 0) deadOIDs.Clear();

            // Идём по списку
            for (int i = 0; i < count; i++)
            {
                GameObject go = objects[i];
                int missings = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                
                if (missings == 0) continue;

                // если на объекте есть мёртвые скрипты - добавляем ObjectID в список
                ObjectID objectID = new ObjectID(go, debug);
                deadOIDs.Add(objectID);

                EditorUtility.DisplayProgressBar(strProgressBar_SearchDeadObject,
                                                 strProgressBar_SearchingScene, 
                                                 i * 1f / count);
            }
            EditorUtility.ClearProgressBar();

            // если не найдены
            if (deadOIDs.Count == 0)
            {
                Debug.Log($"<color=green>Everything is okay :)</color>");
            }
        }

        public void SearchForDeads()
        {
            // обновляемся
            UpdateDeadList();

            // прогружаем файл сцены
            sceneFile = new SceneFile();

            // шерстим гиды мёртвых
            deadGUIDs = new List<ScriptGUID>();
            int count = deadOIDs.Count;
            for (int i = 0; i < count; i++)
            {
                EditorUtility.DisplayProgressBar(strProgressBar_SearchingSceneFile,
                                                 strProgressBar_InspectedObject + $"{i} / {count}", 
                                                 i * 1f / count);

                // какой объект мы сейчас инспектируем
                ObjectID oid = deadOIDs[i]; 

                // пытаемся взять список компонентных GID'ов для данного объекта
                List<ulong> componentGIDs = sceneFile.GetCGIDs(oid, debug);

                // компонентные гиды не были взяты (объект в префабе или сцена была изменена)
                if (componentGIDs == null)
                {
                    if (debug) oid.DebugObjectNotFound();
                    continue;
                }

                // список гидов равен нулю
                if (componentGIDs.Count == 0)
                {
                    if (debug) Debug.Log($"GIDs count of {oid.gameObject.name} is 0");
                    continue;
                }

                // идём по всем найденным гидам
                for (int g = 0; g < componentGIDs.Count; g++)
                {
                    ulong gid = componentGIDs[g];

                    if (oid.liveIDs.Contains(gid)) // пропускаем живчиков
                        continue; 

                    string guid = sceneFile.GetGUID(gid, debug);

                    if (string.IsNullOrEmpty(guid)) // GUID не был найден
                        continue;

                    // GUID найден, создаём учётку для скрипта
                    ScriptInstanceGID deadGID = new ScriptInstanceGID(gid, sceneFile);

                    // проверяем, есть GUID в списке или нет
                    int guidIDX = deadGUIDs.FindIndex(x => x.guid == guid);
                    if (guidIDX >= 0) // добавляем новый объект и новый компонент к уже существующему GUID
                    {
                        deadGUIDs[guidIDX].oids.Add(oid);
                        deadGUIDs[guidIDX].gids.Add(deadGID);
                    }
                    else // создаём новую GUID-учёткуы
                    {
                        ScriptGUID deadGUID = new ScriptGUID(guid, true);
                        deadGUIDs.Add(deadGUID);
                        deadGUID.oids.Add(oid);
                        deadGUID.gids.Add(deadGID);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            if (debug)
            {
                for (int i = 0; i < deadGUIDs.Count; i++)
                {
                    Debug.Log($"Dead GUID <b>#{i}</b>: <i><color=red>{deadGUIDs[i]}</color></i> " +
                              $"(Scripts Broken: {deadGUIDs[i].oids.Count})");
                }
            }
        }
    }
}
#endif