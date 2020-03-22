using System.Collections.Generic;
using UnityEngine;
using Spiral.Core;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class DeadScripts : MonoBehaviour
    {
        public bool debug = true;

        public List<ObjectID> deadIDs { get; private set; } = new List<ObjectID>();
        private SceneFile sceneFile = null;

        public void SelectDeads()
        {
            List<GameObject> selectObjects = new List<GameObject>();
            for (int i = 0; i < deadIDs.Count; i++)
            {
                if (deadIDs[i].gameObject == null) continue;
                selectObjects.Add(deadIDs[i].gameObject);
            }
            Selection.objects = selectObjects.ToArray();
        }

        public void UpdateDeadList()
        {
            // Собираем все объекты на сцене
            var objects = CoreFunctions.CollectScene().Transforms2GameObjects();
            int count = objects.Count;

            // Чистим лист объектов с мёртвыми скриптами
            if (deadIDs == null) deadIDs = new List<ObjectID>();
            else if (deadIDs.Count != 0) deadIDs.Clear();

            // Идём по списку
            for (int i = 0; i < count; i++)
            {
                GameObject go = objects[i];
                int missings = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                
                if (missings == 0) continue;

                // если на объекте есть мёртвые скрипты - добавляем ObjectID в список
                ObjectID objectID = new ObjectID(go, debug);
                deadIDs.Add(objectID);
            }

            // если не найдены
            if (deadIDs.Count == 0)
            {
                Debug.Log($"<color=green>Everything is okay :)</color>");
            }
        }

        public void SearchForDeads(bool updateDeads = true)
        {
            // обновляемся
            if (updateDeads) UpdateDeadList();

            // прогружаем файл сцены
            sceneFile = new SceneFile();

            // шерстим гиды мёртвых
            List<string> deadGUIDs = new List<string>();
            List<int> deadCounts = new List<int>();

            for (int i = 0; i < deadIDs.Count; i++)
            {
                // какой объект мы сейчас инспектируем
                ObjectID oid = deadIDs[i]; 

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

                    if (deadGUIDs.Contains(guid)) // такой GUID уже зарегистрирован в списке мёртвых GUID'ов
                    {
                        int idx = deadGUIDs.FindIndex(x => x == guid);
                        deadCounts[idx] += 1;
                        continue; 
                    }

                    deadGUIDs.Add(guid);
                    deadCounts.Add(1);
                }
            }

            // Этот участок кода сработает независимо от того, включен DEBUG или нет!
            for (int i = 0; i < deadGUIDs.Count; i++)
            {
                Debug.Log($"Dead GUID <b>#{i}</b>: <i><color=red>{deadGUIDs[i]}</color></i> (Scripts Broken: {deadCounts[i]})");
            }
        }
    }

    [CustomEditor(typeof(DeadScripts))]
    public class DeadScriptsEditor : Editor
    {
        private DeadScripts deadmono = null;

        private void OnEnable()
        {
            deadmono = serializedObject.targetObject as DeadScripts;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("ПОИСК МЁРТВЫХ СКРИПТОВ:", EditorStyles.boldLabel);


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Только объекты:", EditorStyles.miniBoldLabel);
            if (GUILayout.Button("Найти и выделить"))
            {
                deadmono.UpdateDeadList();
                deadmono.SelectDeads();
            }
            if (GUILayout.Button("Выделить"))
            {
                deadmono.SelectDeads();
            }
            EditorGUILayout.HelpBox("При нажатии на [Выделить] выполняется попытка выделить все объекты, " +
                                    "которые ранее были опознаны как содержащие мёртвые скрипты. " +
                                    "Проверьте, что поиск уже был выполнен, " +
                                    "в противном случае используйте опцию [Найти и выделить];",
                                    MessageType.Warning);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Проверка файла сцены:", EditorStyles.miniBoldLabel); 
            EditorGUILayout.HelpBox("Убедитесь, что сцена была сохранена, поскольку поиск будет идти по файлу сцены.\n" +
                                    "Для сцен с большим количеством объектов поиск может идти медленно!",
                                    MessageType.Warning);
            if (GUILayout.Button("Показать мёртвые GUID (с обновлением)"))
            {
                deadmono.SearchForDeads(true);
            }
            if (GUILayout.Button("Показать мёртвые GUID (без обновления)"))
            {
                deadmono.SearchForDeads(false);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Режим отладки:", EditorStyles.miniBoldLabel);
            deadmono.debug = EditorGUILayout.Toggle("Включить режим отладки", deadmono.debug);
            EditorGUILayout.HelpBox("Режим отладки будет выводить в консоль все действия, что может " +
                                    "существенно замедлить осмотр сцен с большим количеством объектов;",
                                    MessageType.Warning);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }
    }
}
#endif