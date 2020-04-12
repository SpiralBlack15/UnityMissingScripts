using System;
using System.Collections;
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
        [NonSerialized]private Color colorNormal = new Color(0.5f, 0.5f, 0.5f);
        [NonSerialized]private Color colorAlert  = new Color(0.8f, 0.5f, 0.5f);
        [NonSerialized]private Color colorGood   = new Color(0.8f, 0.8f, 0.8f);
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

            GUI.color = dead ? colorAlert : colorNormal;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = defaultColor;

            EditorGUILayout.LabelField($"Game Object: {oid.gameObject.name}", EditorStyles.boldLabel, labelOption);
            EditorGUILayout.SelectableLabel($"File ID: {oid.globalID.targetObjectId}", labelOption);

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
                    Color drawColor = cid.alive ? colorGood : colorAlert;

                    GUI.backgroundColor = drawColor;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = defaultColor;

                    if (cid.component == null)
                    {
                        EditorGUILayout.SelectableLabel($"Component #{comIDX} is missing!", labelOption);
                    }
                    else
                    {
                        EditorGUILayout.SelectableLabel($"Component #{comIDX}: {cid.type.Name}", labelOption);
                        EditorGUILayout.SelectableLabel($"Fild ID: {cid.gid}", labelOption);
                        if (!string.IsNullOrEmpty(cid.guid))
                        {
                            EditorGUILayout.SelectableLabel($"Script GUID: {cid.guid}", labelOption);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No GUID found", labelOption);
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}
#endif
