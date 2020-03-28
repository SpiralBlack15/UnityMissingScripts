using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Spiral.EditorTools.DeadScriptsSearcher.Localization;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class DeadWindow : EditorWindow
    {
        private DeadScripts deadscript { get { return DeadScripts.instance; } }

        private Vector2 scrollPos;
        private Color defaultColor = Color.white;

        // MENU INITIALIZATION ====================================================================
        // Simply call it from menu
        //=========================================================================================
        [MenuItem("Spiral Tools/Dead Scripts Searcher")]
        public static void Init()
        {
            DeadWindow window = (DeadWindow)GetWindow(typeof(DeadWindow));
            window.Show();
        }

        private void OnGUI()
        {
            defaultColor = GUI.color;
            titleContent.text = strEditorWindow_DeadScriptSearcher;
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

        // DRAWING FUNCTIONS ======================================================================
        // Draw interface block-by-block
        //=========================================================================================
        private void DrawDebugMode()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            deadscript.debug = EditorGUILayout.Toggle(strDebugMode, deadscript.debug);
            if (deadscript.debug)
            {
                EditorGUILayout.HelpBox(strDebugModeHelp, MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSimpleMode()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(strObjectsOnly, EditorStyles.boldLabel);
            if (GUILayout.Button(strObjectsOnlyButton))
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
            EditorGUILayout.LabelField(strSceneFileCheckout, EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUIStyle styleSceneIsDirty = new GUIStyle(EditorStyles.boldLabel);
            string sceneIsDirty = deadscript.isDirty ? 
                                  strSceneWasChanged : 
                                  strSceneClear;
            styleSceneIsDirty.normal.textColor = deadscript.isDirty ? new Color(0.8f, 0.0f, 0.0f) : Color.gray;
            EditorGUILayout.LabelField(sceneIsDirty, styleSceneIsDirty);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel += 1;
            foldoutSceneSearchHelp = EditorGUILayout.Foldout(foldoutSceneSearchHelp, 
                                                             strShowHelp, 
                                                             true, EditorStyles.foldout);
            EditorGUI.indentLevel -= 1;
            if (foldoutSceneSearchHelp)
            {
                EditorGUILayout.HelpBox(strSceneHelpWarning, MessageType.Warning);
                EditorGUILayout.HelpBox(strSceneHelpExplanation, MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button(strFindDeadGUIDs))
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
            GUILayoutOption labelOption = GUILayout.Height(20);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(strFoundGUIDs + $"{deadscript.deadGUIDs.Count}", 
                                       EditorStyles.miniBoldLabel, labelOption);

            if (deadscript.deadGUIDs.Count != 0)
            {
                EditorGUI.indentLevel += 1;
                foldoutDeads = EditorGUILayout.Foldout(foldoutDeads, 
                                                       strShowList, 
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
                    ScriptGUID dead = deadscript.deadGUIDs[i];
                    DrawDeadGUIDEntry(dead);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawDeadGUIDEntry(ScriptGUID dead)
        {
            GUILayoutOption labelOption = GUILayout.Height(20);
            GUI.color = new Color(0.5f, 0.5f, 0.5f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = defaultColor;

            EditorGUILayout.SelectableLabel($"GUID: {dead.guid}", GUILayout.MinWidth(250), labelOption);

            string strDeadCount = strDeadObjectsCount + $" {dead.oids.Count}";

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
                        if (GUILayout.Button(strSelectObject))
                        {
                            Selection.objects = new Object[1] { dead.oids[i].gameObject }; 
                        }
                        GUI.enabled = false;
                        EditorGUILayout.TextArea(dgid.fileEntry);
                        GUI.enabled = true;
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            if (GUILayout.Button(strSelectObjects))
            {
                ObjectID.Select(dead.oids);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif

