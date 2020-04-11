using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Spiral.EditorTools.DeadScriptsSearcher.Localization;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class MonoView : EditorWindow
    {
        private Vector2 scrollPos;
        private Color defaultColor = Color.white;

        [MenuItem("Spiral Tools/Object Inspector")]
        public static void Init()
        {
            MonoView window = (MonoView)GetWindow(typeof(MonoView));
            window.Show();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnGUI()
        {
            GUILayoutOption labelOption = GUILayout.Height(20); 
            Color colorGray  = new Color(0.5f, 0.5f, 0.5f);
            Color colorAlert = new Color(0.8f, 0.5f, 0.5f); 
            Color colorGood  = new Color(0.8f, 0.8f, 0.8f);

            //-------------------------------------------------------------------------------------
            defaultColor = GUI.color;
            titleContent.text = strEditorWindow_MonoView;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,
                                                        GUILayout.Height(position.height));
            DrawLanguageSelect();

            var selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                EditorGUILayout.LabelField("Выделите объект в инспекторе сцены, чтобы посмотреть данные",
                                           labelOption);
            }
            for (int objIDX = 0; objIDX < selected.Length; objIDX++)
            {
                ObjectID oid = new ObjectID(selected[objIDX], false);
                bool dead = oid.missingScriptsCount > 0;

                GUI.color = dead ? colorAlert : colorGray;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = defaultColor;

                EditorGUILayout.LabelField($"Game Object: {oid.gameObject.name}", EditorStyles.boldLabel, labelOption);
                EditorGUILayout.SelectableLabel($"File ID: {oid.globalID.targetObjectId}", labelOption);
                
                if (oid.componentIDs != null)
                {
                    EditorGUILayout.LabelField($"Components:", EditorStyles.boldLabel, labelOption);
                    for (int comIDX = 0; comIDX < oid.componentIDs.Count; comIDX++)
                    {
                        var cid = oid.componentIDs[comIDX];
                        Color drawColor = cid.alive ? colorGood : colorAlert;

                        GUI.backgroundColor = drawColor;
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        GUI.backgroundColor = defaultColor;

                        if (cid.component == null)
                        {
                            EditorGUILayout.SelectableLabel($"Component is missing!", labelOption);
                        }
                        else
                        {
                            EditorGUILayout.SelectableLabel($"Component: {cid.type.Name}", labelOption);
                            EditorGUILayout.SelectableLabel($"Fild ID: {cid.goid.targetObjectId}", labelOption);
                        }

                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUI.color = defaultColor;
        }
    }
}
#endif
