using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class DeadWindow : EditorWindow
    {
        private readonly DeadScripts deadscript = new DeadScripts() { debug = false };
        public Local lang = Local.RU;

        // LOCALIZATION ===========================================================================
        // Simple localization to those who do not understand Russian :)
        //=========================================================================================
        #region Localization
        private readonly static LocalString strLocal = new LocalString(
            // ru
            "ЯЗЫК:",
            // en
            "LANGUAGE:");
        private readonly static LocalString strDebugMode = new LocalString(
            // ru
            "Режим отладки",
            // en
            "Debug mode");
        private readonly static LocalString strDebugModeHelp = new LocalString(
            // ru
            "Режим отладки будет выводить в консоль все действия, что может " +
            "существенно замедлить осмотр сцен с большим количеством объектов",
            // en
            "Debug mode will output all actions to the console. This can " +
            "significantly slow down the checkup of scenes " +
            "with a large number of objects");
        private readonly static LocalString strObjectsOnly = new LocalString(
            // ru
            "Только объекты",
            // en
            "Objects only:"
            );
        private readonly static LocalString strObjectsOnlyButton = new LocalString(
            // ru
            "Найти и выделить объекты с мёртвыми скриптами",
            // en
            "Find and select objects with dead scripts"
            );
        private readonly static LocalString strSceneFileCheckout = new LocalString(
            // ru
            "Проверка файла сцены:",
            // en
            "Scene file checkout:"
            );
        private readonly static LocalString strSceneWasChanged = new LocalString(
           // ru
           "СЦЕНА БЫЛА ИЗМЕНЕНА (сохраните изменения)",
           // en
           "SCENE WAS CHANGED (save changes first)"
           );
        private readonly static LocalString strSceneClear = new LocalString(
           // ru
           "СЦЕНА СОХРАНЕНА",
           // en
           "SCENE SAVED"
           );
        private readonly static LocalString strShowHelp = new LocalString(
            // ru
            "Показать/Скрыть справку",
            // en
            "Show/Hide help");
        private readonly static LocalString strSceneHelpWarning = new LocalString(
            // ru
            "Убедитесь, что сцена была сохранена!\n" +
            "Для сцен с большим количеством объектов поиск может идти медленно.\n" +
            "Если объектов больше сотни, это может занять время.",
            // en
            "Make sure the scene has been saved!\n" +
            "For scenes with a large number of objects, the search may go slowly.\n" +
            "If there are more than a hundred objects, this may take some time.");
        private readonly static LocalString strSceneHelpExplanation = new LocalString(
            // ru
            "Поиск идёт по файлу сцены, сопоставляя объекты с " +
            "битыми скриптами с их записями в файле. " +
            "Обратите внимание, что из поиска исключены скрипты, " +
            "не являющиеся MonoBehaviour, а также дочерние объекты в составе префабов.",
            // en
            "The search goes through the scene file, matching objects with broken scripts " +
            "with their entries in the file. " +
            "Note that scripts that are not MonoBehaviour, as well as child objects in prefabs, " +
            "are excluded from the search.");
        private readonly static LocalString strFindDeadGUIDs = new LocalString(
            // ru
            "Найти мёртвые GUID в сцене:",
            // en
            "Find dead GUIDs in the scene:");
        private readonly static LocalString strFoundGUIDs = new LocalString(
            // ru
            "Найдено уникальных GUID:",
            // en
            "Unique GUIDs found:"
            );
        private readonly static LocalString strShowList = new LocalString(
            // ru
            "Показать/Скрыть список",
            // en
            "Show/Hide list"
            );
        private readonly static LocalString strDeadObjectsCount = new LocalString(
            // ru
            "Объектов",
            // en
            "Objects count");
        private readonly static LocalString strSelectObjects = new LocalString(
            // ru
            "Выделить все объекты с этим скриптом",
            // en
            "Select all objects with this script");
        private readonly static LocalString strSelectObject = new LocalString(
            // ru
            "Выделить объект",
            // en
            "Select target object");
        #endregion

        // MENU INITIALIZATION ====================================================================
        // Simply call it from menu
        //=========================================================================================
        [MenuItem("Spiral Tools/Dead Scripts Searcher")]
        public static void Init()
        {
            DeadWindow window = (DeadWindow)GetWindow(typeof(DeadWindow));
            window.Show();
        }

        // DRAWING FUNCTIONS ======================================================================
        // Draw interface block-by-block
        //=========================================================================================
        private void DrawLanguageSelect()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            lang = (Local)EditorGUILayout.EnumPopup(strLocal.Read(lang), lang);
            EditorGUILayout.EndVertical();
        }

        private void DrawDebugMode()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            deadscript.debug = EditorGUILayout.Toggle(strDebugMode.Read(lang), deadscript.debug);
            if (deadscript.debug)
            {
                EditorGUILayout.HelpBox(strDebugModeHelp.Read(lang), MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSimpleMode()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(strObjectsOnly.Read(lang), EditorStyles.boldLabel);
            if (GUILayout.Button(strObjectsOnlyButton.Read(lang)))
            {
                deadscript.UpdateDeadList();
                deadscript.SelectDeads();
            }
            EditorGUILayout.EndVertical();
        }

        private bool foldoutSceneSearchHelp = false;
        private void DrawBoxSceneState()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(strSceneFileCheckout.Read(lang), EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUIStyle styleSceneIsDirty = new GUIStyle(EditorStyles.boldLabel);
            string sceneIsDirty = deadscript.isDirty ? 
                                  strSceneWasChanged.Read(lang) : 
                                  strSceneClear.Read(lang);
            styleSceneIsDirty.normal.textColor = deadscript.isDirty ? new Color(0.8f, 0.0f, 0.0f) : Color.gray;
            EditorGUILayout.LabelField(sceneIsDirty, styleSceneIsDirty);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel += 1;
            foldoutSceneSearchHelp = EditorGUILayout.Foldout(foldoutSceneSearchHelp, 
                                                             strShowHelp.Read(lang), 
                                                             true, EditorStyles.foldout);
            EditorGUI.indentLevel -= 1;
            if (foldoutSceneSearchHelp)
            {
                EditorGUILayout.HelpBox(strSceneHelpWarning.Read(lang), MessageType.Warning);
                EditorGUILayout.HelpBox(strSceneHelpExplanation.Read(lang), MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button(strFindDeadGUIDs.Read(lang)))
            {
                deadscript.SearchForDeads();
                if (deadscript.deadGUIDs.Count > 0) foldoutDeads = true;
            }
            ShowDeadGUIDs();

            EditorGUILayout.EndVertical();
        }

        private bool foldoutDeads = false;
        private void ShowDeadGUIDs()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"{strFoundGUIDs.Read(lang)}: {deadscript.deadGUIDs.Count}", 
                                       EditorStyles.miniBoldLabel);

            if (deadscript.deadGUIDs.Count != 0)
            {
                EditorGUI.indentLevel += 1;
                foldoutDeads = EditorGUILayout.Foldout(foldoutDeads, 
                                                       strShowList.Read(lang), 
                                                       true, EditorStyles.foldout);
                EditorGUI.indentLevel -= 1;
            }
            else
            {
                foldoutDeads = false;
            }

            if (foldoutDeads)
            {
                for (int i = 0; i < deadscript.deadGUIDs.Count; i++)
                {
                    DeadGUID dead = deadscript.deadGUIDs[i];
                    DrawDeadGUIDEntry(dead);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawDeadGUIDEntry(DeadGUID dead)
        {
            GUI.color = new Color(0.5f, 0.5f, 0.5f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = defaultColor;

            EditorGUILayout.SelectableLabel($"GUID: {dead.guid}", GUILayout.MinWidth(250));

            string strDeadCount = $"{strDeadObjectsCount.Read(lang)}: {dead.oids.Count}";

            dead.showInfo = EditorGUILayout.Foldout(dead.showInfo, strDeadCount);
            if (dead.showInfo)
            {
                for (int i = 0; i < dead.gids.Count; i++)
                {
                    var dgid = dead.gids[i];
                    var dgidID = dgid.gid;

                    string strGID = $"{dgidID}";
                    string strButtonName = $"#{i} MonoBehaviour ID: {strGID}";
                    if (EditorGUILayout.DropdownButton(new GUIContent(strButtonName), FocusType.Passive))
                    {
                        dgid.showInfo = !dgid.showInfo;
                    }
                    if (dgid.showInfo)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.SelectableLabel(strGID);
                        if (GUILayout.Button(strSelectObject.Read(lang)))
                        {
                            Selection.objects = new Object[1] { dead.oids[i].gameObject }; 
                        }
                        GUI.enabled = false;
                        EditorGUILayout.TextArea(dgid.entry);
                        GUI.enabled = true;
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            if (GUILayout.Button(strSelectObjects.Read(lang)))
            {
                ObjectID.Select(dead.oids);
            }

            EditorGUILayout.EndVertical();
        }


        // MONO BEHVAIOUR =========================================================================
        // Editor Window's Mono
        //=========================================================================================
        Vector2 scrollPos;
        Color defaultColor = Color.white;
        private void OnGUI()
        {
            defaultColor = GUI.color;
            switch (lang)
            {
                case Local.RU:
                    titleContent.text = "Поиск мёртвых скриптов";
                    break;
                case Local.ENG:
                    titleContent.text = "Dead Scripts Searcher";
                    break;
                default:
                    titleContent.text = "Dead Scripts Searcher";
                    break;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
                                                        GUILayout.Height(position.height));

            DrawLanguageSelect();
            DrawDebugMode();
            DrawSimpleMode();
            DrawBoxSceneState();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
            GUI.color = defaultColor;
        }
    }
}
#endif

