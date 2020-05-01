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
using System;
using System.Collections.Generic;
using Spiral.Core;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorToolkit
{
    public enum PanelType { Vertical, Horizontal }

    public static class SpiralEditor
    {
        public static Color defaultLogoColor   { get; private set; } = new Color(0.7f, 0.8f, 0.7f);
        public static Color defaultPanelColor  { get; private set; } = new Color(0.7f, 0.7f, 0.7f);
        public static Color defaultButtonColor { get; private set; } = new Color(0.9f, 0.9f, 0.9f);

        public static Color colorLightRed      { get; private set; } = new Color(0.9f, 0.7f, 0.7f);
        public static Color colorLightGreen    { get; private set; } = new Color(0.7f, 0.9f, 0.7f);
        public static Color colorLightBlue     { get; private set; } = new Color(0.5f, 0.7f, 0.8f);

        // PANELS ---------------------------------------------------------------------------------
        public static GUIStyle panel { get; private set; }
        public static GUIStyle indentedPanel { get; private set; }

        // LABELS ---------------------------------------------------------------------------------
        public static GUIStyle logoLabel { get; private set; }
        public static GUIStyle smallLabel { get; private set; }
        public static GUIStyle normalLabel { get; private set; }
        public static GUIStyle boldLabel { get; private set; }
        public static GUIStyle smallBoldLabel { get; private set; }
        public static GUIStyle boxedBoldLabel { get; private set; }
        public static GUIStyle smallBoxedLabel { get; private set; }
        public static GUIStyle boxedBoldLabelCaption { get; private set; }

        // BUTTONS --------------------------------------------------------------------------------
        public static GUIStyle buttonNormal { get; private set; }

        // FOLDOUTS -------------------------------------------------------------------------------
        public static GUIStyle foldout { get; private set; }
        public static GUIStyle indentedFoldout { get; private set; }
        public static GUIStyle indentedBoldFoldout { get; private set; }

        // INDENTS --------------------------------------------------------------------------------
        public static int indentedFoldoutLeftIndent { get; private set; } = 8;
        public static int indentedPanelLeftIndent   { get; private set; } = 12;

        // GUI FUNCTIONS ==========================================================================
        // Заместители GUI
        //=========================================================================================
        public static GUIContent GetLabel(string text, string tooltip)
        {
            GUIContent output = new GUIContent(text, tooltip);
            return output;
        }

        public static bool DrawRoundButton(string name, Color? color = null, params GUILayoutOption[] options)
        {
            Color prevColor = GUI.color;
            GUI.color = color != null ? (Color)color : defaultButtonColor;
            bool result = GUILayout.Button(name, buttonNormal, options);
            GUI.color = prevColor;
            return result;
        }

        public static bool DrawRoundButton(GUIContent name, Color? color = null, params GUILayoutOption[] options)
        {
            Color prevColor = GUI.color;
            GUI.color = color != null ? (Color)color : defaultButtonColor;
            bool result = GUILayout.Button(name, buttonNormal, options);
            GUI.color = prevColor;
            return result;
        }

        public static void DrawCaptionLabel(GUIContent content, bool selectable = false, bool small = false, params GUILayoutOption[] options)
        {
            GUIStyle style = small ? smallBoldLabel : boldLabel;
            if (!selectable) EditorGUILayout.LabelField(content, style, options);
            else EditorGUILayout.SelectableLabel(content.text, style, options);
        }

        public static void DrawCaptionLabel(string content, bool selectable = false, bool small = false, params GUILayoutOption[] options)
        {
            GUIStyle style = small ? smallBoldLabel : boldLabel;
            if (!selectable) EditorGUILayout.LabelField(content, style, options);
            else EditorGUILayout.SelectableLabel(content, style, options);
        }

        public static void DrawCaptionLabel(string content, bool small = false, params GUILayoutOption[] options)
        {
            GUIStyle style = small ? smallBoldLabel : boldLabel;
            EditorGUILayout.LabelField(content, style, options);
        }

        public static void DrawCaptionLabel(GUIContent content, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(content, boldLabel, options);
        }


        private static readonly List<PanelType> panelTypesStack = new List<PanelType>();

        public static void BeginPanel(PanelType panelType, Color? color = null)
        {
            Color prevColor = GUI.color;
            GUI.color = color != null ? (Color)color : defaultPanelColor;
            if (panelType == PanelType.Vertical) EditorGUILayout.BeginVertical(panel);
            else EditorGUILayout.BeginHorizontal(panel);
            panelTypesStack.Add(panelType);
            GUI.color = prevColor;
        }

        public static void BeginPanel(string caption, bool smallCaption, params GUILayoutOption[] options)
        {
            BeginPanel(PanelType.Vertical);
            GUIStyle style = smallCaption ? smallBoldLabel : boldLabel;
            EditorGUILayout.LabelField(caption, style, options);
        }

        public static void BeginPanel(string caption, Color color, params GUILayoutOption[] options)
        {
            BeginPanel(PanelType.Vertical, color);
            EditorGUILayout.LabelField(caption, boldLabel, options);
        }

        public static void BeginPanel(string caption, bool smallCaption = false, Color? color = null, params GUILayoutOption[] options)
        {
            Color sendColor = color != null ? (Color)color : defaultPanelColor;
            BeginPanel(PanelType.Vertical, sendColor);
            GUIStyle style = smallCaption ? smallBoldLabel : boldLabel;
            EditorGUILayout.LabelField(caption, style, options);
        }

        public static void BeginPanel(GUIContent caption, bool smallCaption = false, Color? color = null, params GUILayoutOption[] options)
        {
            Color sendColor = color != null ? (Color)color : defaultPanelColor;
            BeginPanel(PanelType.Vertical, sendColor);
            GUIStyle style = smallCaption ? smallBoldLabel : boldLabel;
            EditorGUILayout.LabelField(caption, style, options);
        }

        public static void EndPanel()
        {
            if (panelTypesStack.Count == 0)
            {
                Debug.LogWarning("No panels to close");
                return;
            }
            PanelType panelType = panelTypesStack.GetLast();
            if (panelType == PanelType.Vertical) EditorGUILayout.EndVertical();
            else EditorGUILayout.EndHorizontal();
            panelTypesStack.RemoveLast();
        }

        public static void DrawScriptField(SerializedObject serializedObject)
        {
            BeginPanel(PanelType.Vertical);
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            GUI.enabled = prop != null;
            EditorGUILayout.PropertyField(prop, true);
            if (!GUI.enabled) GUI.enabled = true;
            EndPanel();
        }

        public static void DrawEditorScriptField(ScriptableObject editor)
        {
            BeginPanel(PanelType.Vertical);
            GUI.enabled = false;
            Type type = editor.GetType();
            MonoScript monoScript = MonoScript.FromScriptableObject(editor);
            if (monoScript != null)
            {
                _ = EditorGUILayout.ObjectField("Editor", monoScript, type, false);
            }
            else
            {
                EditorGUILayout.LabelField("No editor single file found", panel);
            }
            GUI.enabled = true;
            EndPanel();
        }

        public static void DrawEditorWindowScriptField(ScriptableObject editor)
        {
            BeginPanel(PanelType.Vertical);
            GUI.enabled = false;
            Type type = editor.GetType();
            MonoScript monoScript = MonoScript.FromScriptableObject(editor);
            if (monoScript != null)
            {
                _ = EditorGUILayout.ObjectField("Editor", monoScript, type, false);
            }
            else
            {
                EditorGUILayout.LabelField("No editor single file found", panel);
            }
            GUI.enabled = true;
            EndPanel();
        }

        public static void DrawLogoLine(Color? color = null)
        {
            Color defaultColor = GUI.color;
            GUI.color = color != null ? (Color)color : defaultLogoColor;
            EditorGUILayout.BeginVertical(panel);
            EditorGUILayout.LabelField("SpiralBlack Scripts © 2020", logoLabel);
            EditorGUILayout.EndVertical();
            GUI.color = defaultColor;
        }

        public static void ShowHelp(string message, ref bool showHelp, MessageType messageType = MessageType.None)
        {
            BeginPanel(PanelType.Vertical);
            EditorGUI.indentLevel += 1;
            string helpFoldout = showHelp ? "Hide help" : "Show help";
            showHelp = EditorGUILayout.Foldout(showHelp, helpFoldout);
            EditorGUI.indentLevel -= 1;
            if (showHelp)
            {
                EditorGUILayout.HelpBox(message, messageType);
            }
            EndPanel();
        }

        // INITIALIZE STYLES ======================================================================
        // Инициализирует все шорткаты, используемые выше
        //=========================================================================================
        static SpiralEditor()
        {
            // panels -----------------------------------------------------------------------------
            panel = new GUIStyle(EditorStyles.helpBox);

            indentedPanel = new GUIStyle(panel);
            indentedPanel.margin.left += indentedPanelLeftIndent;

            // labels -----------------------------------------------------------------------------
            smallLabel = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                richText = true
            };

            normalLabel = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                richText = true
            };

            boldLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                richText = true
            };

            smallBoldLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 10,
                richText = true
            };

            boxedBoldLabel = new GUIStyle(panel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 10,
                richText = true
            };

            boxedBoldLabelCaption = new GUIStyle(panel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                richText = true
            };

            smallBoxedLabel = new GUIStyle(panel)
            {
                fontSize = 10,
                richText = true
            };

            logoLabel = new GUIStyle(boxedBoldLabel);
            logoLabel.normal.textColor = new Color(0.2f, 0.4f, 0.2f);
            logoLabel.alignment = TextAnchor.MiddleRight;

            // buttons ----------------------------------------------------------------------------
            buttonNormal = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                richText = true
            };
            

            // foldouts ---------------------------------------------------------------------------
            foldout = new GUIStyle(EditorStyles.foldout);

            indentedFoldout = new GUIStyle(EditorStyles.foldout);
            indentedFoldout.margin.left += indentedFoldoutLeftIndent;

            indentedBoldFoldout = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
            indentedBoldFoldout.margin.left += indentedFoldoutLeftIndent;
        }

    }
}
#endif

