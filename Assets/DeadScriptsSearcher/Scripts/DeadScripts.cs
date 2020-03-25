using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public struct DeadGUID
    {
        public string guid;
        public List<ObjectID> oids;
    }

    public class DeadScripts
    {
        public bool debug = true;

        public List<ObjectID> deadOIDs  { get; private set; } = new List<ObjectID>();
        public List<DeadGUID> deadGUIDs { get; private set; } = new List<DeadGUID>();


        private SceneFile sceneFile = null;

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
            }

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
            deadGUIDs = new List<DeadGUID>();
            for (int i = 0; i < deadOIDs.Count; i++)
            {
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

                    int guidIDX = deadGUIDs.FindIndex(x => x.guid == guid);
                    if (guidIDX >= 0)
                    {
                        deadGUIDs[guidIDX].oids.Add(oid);
                    }
                    else
                    {
                        DeadGUID deadGUID = new DeadGUID
                        {
                            guid = guid,
                            oids = new List<ObjectID>()
                        };
                        deadGUID.oids.Add(oid);
                        deadGUIDs.Add(deadGUID);
                    }
                }
            }

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