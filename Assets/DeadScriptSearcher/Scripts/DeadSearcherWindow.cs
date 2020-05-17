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

            OpenStandartBack();
            SpiralLocalization.DrawLanguageSelect();
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
            SpiralEditor.BeginPanel(GroupType.Vertical);
            DeadScripts.isDebugMode = EditorGUILayout.Toggle(strDeadScriptSearcher_DebugMode, DeadScripts.isDebugMode);
            if (DeadScripts.isDebugMode)
            {
                EditorGUILayout.HelpBox(strDeadScriptSearcher_DebugModeHelp, MessageType.Warning);
            }
            SpiralEditor.EndPanel();
        }

        private void DrawSimpleMode()
        {
            SpiralEditor.BeginPanel(strObjectsOnly);
            if (SpiralEditor.Button(strObjectsOnlyButton))
            {
                DeadScripts.UpdateDeadList();
                DeadScripts.SelectDeads();
            }
            SpiralEditor.EndPanel();
        }

        private bool foldoutSceneSearchHelp = false;
        private void DrawBoxSceneState()
        {
            SpiralEditor.BeginPanel(strSceneFileCheckout);

            SpiralEditor.BeginPanel(GroupType.Vertical);

            SpiralEditor.BeginPanel(GroupType.Horizontal);
            GUIStyle styleSceneIsDirty = new GUIStyle(SpiralStyles.labelBold);
            string sceneIsDirty = DeadScripts.isDirty ? 
                                  strSceneWasChanged : 
                                  strSceneClear;
            styleSceneIsDirty.normal.textColor = DeadScripts.isDirty ? new Color(0.8f, 0.0f, 0.0f) : Color.gray;
            EditorGUILayout.LabelField(sceneIsDirty, styleSceneIsDirty);
            SpiralEditor.EndPanel();

            SpiralEditor.BeginPanel(GroupType.Vertical);
            EditorGUI.indentLevel += 1;
            foldoutSceneSearchHelp = EditorGUILayout.Foldout(foldoutSceneSearchHelp, 
                                                             strShowHelp, 
                                                             true, SpiralStyles.foldoutNormal);
            EditorGUI.indentLevel -= 1;
            if (foldoutSceneSearchHelp)
            {
                EditorGUILayout.HelpBox(strSceneHelpWarning, MessageType.Warning);
                EditorGUILayout.HelpBox(strSceneHelpExplanation, MessageType.Info);
            }
            SpiralEditor.EndPanel();
            SpiralEditor.EndPanel();

            if (SpiralEditor.Button(strFindDeadGUIDs))
            {
                DeadScripts.SearchForDeads();
                if (DeadScripts.deadGUIDs.Count > 0) foldoutDeads = true;
            }
            ShowDeadGUIDs();

            SpiralEditor.EndPanel();
        }

        private bool foldoutDeads = false;
        private void ShowDeadGUIDs()
        {
            GUILayoutOption labelOption = GUILayout.Height(20);
            SpiralEditor.BeginPanel(strFoundGUIDs + $"{DeadScripts.deadGUIDs.Count}", true, labelOption);

            if (DeadScripts.deadGUIDs.Count != 0)
            {
                EditorGUI.indentLevel += 1;
                foldoutDeads = EditorGUILayout.Foldout(foldoutDeads, 
                                                       strShowList, 
                                                       true, SpiralStyles.foldoutNormal);
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
            SpiralEditor.EndPanel();
        }

        private void DrawDeadGUIDEntry(ComponentGUID dead)
        {
            GUILayoutOption labelOption = GUILayout.Height(20);
            SpiralEditor.BeginPanel(GroupType.Vertical);

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
                        SpiralEditor.BeginPanel(GroupType.Vertical);
                        EditorGUILayout.SelectableLabel(strGID);
                        if (SpiralEditor.Button(strSelectObject))
                        {
                            Selection.objects = new Object[1] { dead.oids[i].gameObject }; 
                        }
                        GUI.enabled = false;
                        EditorGUILayout.TextArea(dgid.fileEntry);
                        GUI.enabled = true;
                        EditorGUILayout.Space();
                        SpiralEditor.EndPanel();
                    }
                }
            }

            if (SpiralEditor.Button(strSelectObjects))
            {
                ObjectID.Select(dead.oids);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif

