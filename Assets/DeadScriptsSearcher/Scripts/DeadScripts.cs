using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class DeadScripts : MonoBehaviour
    {
        public bool debug = true;

        public List<ObjectID> deadIDs { get; private set; } = new List<ObjectID>();
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

        public void SearchForDeads()
        {
            // обновляемся
            UpdateDeadList();

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

        private void DrawDebugMode()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Режим отладки:", EditorStyles.miniBoldLabel);
            deadmono.debug = EditorGUILayout.Toggle("Включить режим отладки", deadmono.debug);
            EditorGUILayout.HelpBox("Режим отладки будет выводить в консоль все действия, что может " +
                                    "существенно замедлить осмотр сцен с большим количеством объектов;",
                                    MessageType.Warning);
            EditorGUILayout.EndVertical();
        }

        private void DrawSimpleMode()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Только объекты:", EditorStyles.boldLabel);
            if (GUILayout.Button("Найти и выделить"))
            {
                deadmono.UpdateDeadList();
                deadmono.SelectDeads();
            }
            EditorGUILayout.HelpBox("При нажатии выполняется попытка найти и выделить все " +
                                    "объекты с битыми скриптами, находящиеся на сцене",
                                    MessageType.None);
            EditorGUILayout.EndVertical();
        }

        private void DrawBoxSceneState()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Проверка файла сцены:", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUIStyle styleSceneIsDirty = new GUIStyle(EditorStyles.boldLabel);
            string sceneIsDirty = deadmono.isDirty ? "СЦЕНА БЫЛА ИЗМЕНЕНА!" : "СЦЕНА СОХРАНЕНА";
            styleSceneIsDirty.normal.textColor = deadmono.isDirty ? new Color(0.8f, 0.0f, 0.0f) : Color.gray;
            EditorGUILayout.LabelField(sceneIsDirty, styleSceneIsDirty);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Убедитесь, что сцена была сохранена, поскольку поиск будет идти по файлу сцены.\n" +
                                   "Для сцен с большим количеством объектов поиск может идти медленно!",
                                   MessageType.Warning);
            EditorGUILayout.EndVertical();
           
            if (GUILayout.Button("Показать мёртвые GUID"))
            {
                deadmono.SearchForDeads();
            }
            EditorGUILayout.HelpBox("Поиск идёт по файлу сцены, сопоставляя объекты с битыми скриптами " +
                                    "с их записями в файле. Все результаты будут выведены в консоль. " +
                                    "Обратите внимание, что из поиска исключаются скрипты, " +
                                    "не являющиеся MonoBehaviour, а также дочерние объекты в составе префабов!",
                                    MessageType.None);

            EditorGUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("ПОИСК МЁРТВЫХ СКРИПТОВ:", EditorStyles.boldLabel);

            DrawDebugMode();
            DrawSimpleMode();
            DrawBoxSceneState();

            EditorGUILayout.EndVertical();
        }
    }
}
#endif