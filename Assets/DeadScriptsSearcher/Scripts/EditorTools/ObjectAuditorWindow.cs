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

using System;
using System.Collections.Generic;
using UnityEngine;
using static Spiral.EditorTools.DeadScriptsSearcher.Localization;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class ObjectAuditorWindow : EditorWindow
    {
        private Vector2 scrollPos;

        [NonSerialized]private GUILayoutOption labelOption = GUILayout.Height(20);
        [NonSerialized]private Color colorNormal = new Color(0.5f, 0.8f, 0.5f);
        [NonSerialized]private Color colorAlert  = new Color(0.8f, 0.5f, 0.5f);
        [NonSerialized]private Color colorGood   = new Color(0.9f, 0.9f, 0.9f);
        private Color defaultColor = Color.white;

        private readonly List<ObjectID> oids = new List<ObjectID>();

        [MenuItem("Spiral Tools/Object Inspector")]
        public static void Init()
        {
            ObjectAuditorWindow window = (ObjectAuditorWindow)GetWindow(typeof(ObjectAuditorWindow));
            window.Show();
        }

        private void OnEnable()
        {
            CheckSelection();
            Repaint();
        }

        private void OnSelectionChange()
        {
            CheckSelection();
            Repaint();
        }

        private void OnGUI()
        {
            defaultColor = GUI.color;
            titleContent.text = strMonoView_Caption;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height));
           
            DrawLanguageSelect();
            SceneFile.DrawSceneReloadButton();

            if (oids.Count == 0)
            {
                EditorGUILayout.LabelField(strMonoView_SelectObject, labelOption);
            }

            for (int objIDX = 0; objIDX < oids.Count; objIDX++)
            {
                DrawObject(oids[objIDX]);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUI.color = defaultColor;
        }

        private void CheckSelection()
        {
            var selected = Selection.gameObjects;
            oids.Clear();
            if (selected != null)
            {
                for (int i = 0; i < selected.Length; i++)
                {
                    ObjectID oid = new ObjectID(selected[i], false);
                    oids.Add(oid);
                }
            }
        }

        private void DrawObject(ObjectID oid)
        {
            bool dead = oid.missingScriptsCount > 0;

            GUI.color = dead ? colorAlert : colorGood;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = defaultColor;

            EditorGUILayout.LabelField($"Game Object: {oid.gameObject.name}", EditorStyles.boldLabel, labelOption);
            EditorGUILayout.SelectableLabel($"File ID: {oid.globalID.targetObjectId}", labelOption);

            string captionIDX;
            if (oid.fileCaptionStringIDX > 0)
            {
                captionIDX = oid.fileCaptionStringIDX.ToString();
            }
            else
            {
                if (oid.isPartOfPrefab)
                {
                    captionIDX = "[prefabed]";
                }
                else
                {
                    captionIDX = "[not found]";
                }
            }

            EditorGUILayout.SelectableLabel($"File caption IDX: {captionIDX}", labelOption);

            string strShowInfo = oid.showInfo ? strMonoView_HideObjectInfo : strMonoView_ShowObjectInfo;
            oid.showInfo = EditorGUILayout.Foldout(oid.showInfo, strShowInfo);
            if (oid.showInfo)
            {
                DrawComponentList(oid);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawComponentList(ObjectID oid)
        {
            if (oid.componentIDs != null)
            {
                for (int comIDX = 0; comIDX < oid.componentIDs.Count; comIDX++)
                {
                    var cid = oid.componentIDs[comIDX];
                    Color drawColor = cid.alive ? colorNormal : colorAlert;

                    GUI.color = drawColor;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = defaultColor;

                    ulong fileID = oid.componentFileIDs[comIDX]; // соответствует cid.fileID
                    string guid = oid.componentGUIDs[comIDX];
                    // TODO: эта замена временная, так как функционал ещё не перенесён в ComponentID

                    if (cid.component == null)
                    {
                        EditorGUILayout.SelectableLabel($"Component #{comIDX} is missing!", labelOption);
                    }
                    else
                    {
                        EditorGUILayout.SelectableLabel($"Component #{comIDX}: {cid.type.Name}", labelOption);
                    }

                    EditorGUILayout.SelectableLabel($"Fild ID: {fileID}", labelOption);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        EditorGUILayout.SelectableLabel($"Script GUID: {guid}", labelOption);
                    }
                    else
                    {
                        bool isPrefabed = oid.isPartOfPrefab;
                        string message = "No GUID found";
                        if (cid.mScript == null)
                        {
                            message += " [Is not MonoBehaviour]";
                        }
                        if (isPrefabed)
                        {
                            message += " [Prefabed]"; // может одновременно быть и то, и то
                        }
                        EditorGUILayout.LabelField(message, labelOption);
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}
#endif
