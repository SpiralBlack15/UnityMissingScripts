// *********************************************************************************
// The MIT License (MIT)
// Copyright (c) 2020 SpiralBlack https://github.com/SpiralBlack15
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

using UnityEngine;
using static Spiral.EditorToolkit.DeadScriptsSearcher.DeadScriptLocalization;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorToolkit.DeadScriptsSearcher
{
    public class DeadSearcherWindow : SpiralCustomEditorWindow
    {
        private Vector2 scrollPos;

        // MENU INITIALIZATION ====================================================================
        // Simply call it from menu
        //=========================================================================================
        [MenuItem("Spiral Tools/Dead Scripts Searcher")]
        public static void Init()
        {
            DeadSearcherWindow window = (DeadSearcherWindow)GetWindow(typeof(DeadSearcherWindow));
            window.Show();
        }

        private void OnGUI()
        {
            titleContent.text = strDeadScriptSearcher_Caption;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height));

            OpenStandartBack(true);
            Localization.DrawLanguageSelect();
            SceneFile.DrawSceneReloadButton();
            DrawDebugMode();
            DrawSimpleMode();
            DrawBoxSceneState();
            CloseStandartBack();

            EditorGUILayout.EndScrollView();
        }

        // DRAWING FUNCTIONS ======================================================================
        // Draw interface block-by-block
        //=========================================================================================
        private void DrawDebugMode()
        {
            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            DeadScripts.isDebugMode = EditorGUILayout.Toggle(strDeadScriptSearcher_DebugMode, DeadScripts.isDebugMode);
            if (DeadScripts.isDebugMode)
            {
                EditorGUILayout.HelpBox(strDeadScriptSearcher_DebugModeHelp, MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSimpleMode()
        {
            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            EditorGUILayout.LabelField(strObjectsOnly, SpiralStyles.boldLabel);
            if (GUILayout.Button(strObjectsOnlyButton))
            {
                DeadScripts.UpdateDeadList();
                DeadScripts.SelectDeads();
            }
            EditorGUILayout.EndVertical();
        }

        private bool foldoutSceneSearchHelp = false;
        private void DrawBoxSceneState()
        {
            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            EditorGUILayout.LabelField(strSceneFileCheckout, SpiralStyles.boldLabel);

            EditorGUILayout.BeginVertical(SpiralStyles.panel);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUIStyle styleSceneIsDirty = new GUIStyle(SpiralStyles.boldLabel);
            string sceneIsDirty = DeadScripts.isDirty ? 
                                  strSceneWasChanged : 
                                  strSceneClear;
            styleSceneIsDirty.normal.textColor = DeadScripts.isDirty ? new Color(0.8f, 0.0f, 0.0f) : Color.gray;
            EditorGUILayout.LabelField(sceneIsDirty, styleSceneIsDirty);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            EditorGUI.indentLevel += 1;
            foldoutSceneSearchHelp = EditorGUILayout.Foldout(foldoutSceneSearchHelp, 
                                                             strShowHelp, 
                                                             true, SpiralStyles.foldout);
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
                DeadScripts.SearchForDeads();
                if (DeadScripts.deadGUIDs.Count > 0) foldoutDeads = true;
            }
            ShowDeadGUIDs();

            EditorGUILayout.EndVertical();
        }

        private bool foldoutDeads = false;
        private void ShowDeadGUIDs()
        {
            GUILayoutOption labelOption = GUILayout.Height(20);
            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            EditorGUILayout.LabelField(strFoundGUIDs + $"{DeadScripts.deadGUIDs.Count}", 
                                       SpiralStyles.smallBoldLabel, labelOption);

            if (DeadScripts.deadGUIDs.Count != 0)
            {
                EditorGUI.indentLevel += 1;
                foldoutDeads = EditorGUILayout.Foldout(foldoutDeads, 
                                                       strShowList, 
                                                       true, SpiralStyles.foldout);
                EditorGUI.indentLevel -= 1;
            }
            else
            {
                foldoutDeads = false;
            }

            if (foldoutDeads)
            {
                for (int i = 0; i < DeadScripts.deadGUIDs.Count; i++)
                {
                    ComponentGUID dead = DeadScripts.deadGUIDs[i];
                    DrawDeadGUIDEntry(dead);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawDeadGUIDEntry(ComponentGUID dead)
        {
            GUILayoutOption labelOption = GUILayout.Height(20);
            GUI.color = new Color(0.5f, 0.5f, 0.5f);
            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            GUI.color = defaultColor;

            EditorGUILayout.SelectableLabel($"GUID: {dead.guid}", GUILayout.MinWidth(250), labelOption);

            string strDeadCount = strDeadObjectsCount + $" {dead.oids.Count}";

            dead.showInfo = EditorGUILayout.Foldout(dead.showInfo, strDeadCount);
            if (dead.showInfo)
            {
                for (int i = 0; i < dead.gids.Count; i++)
                {
                    var dgid = dead.gids[i];
                    var dgidID = dgid.fileID;

                    string strGID = $"{dgidID}";
                    string strButtonName = $"#{i} MonoBehaviour ID: {strGID}";
                    if (EditorGUILayout.DropdownButton(new GUIContent(strButtonName), FocusType.Passive))
                    {
                        dgid.showInfo = !dgid.showInfo;
                    }
                    if (dgid.showInfo)
                    {
                        EditorGUILayout.BeginVertical(SpiralStyles.panel);
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

