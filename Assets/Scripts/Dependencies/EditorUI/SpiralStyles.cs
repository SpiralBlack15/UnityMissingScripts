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

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorToolkit
{
    public static class SpiralStyles
    {
        public static Color defaultLogoColor = new Color(0.7f, 0.8f, 0.7f);
        public static Color defaultPanelColor = new Color(1.0f, 1.0f, 1.0f);

        public static GUIStyle panel { get; private set; }
        public static GUIStyle logoLabel { get; private set; }
        public static GUIStyle boldLabel { get; private set; }
        public static GUIStyle smallBoldLabel { get; private set; }
        public static GUIStyle boxedBoldLabel { get; private set; }
        public static GUIStyle boxedBoldLabelCaption { get; private set; }
        public static GUIStyle foldout { get; private set; }
        public static GUIStyle indentedFoldout { get; private set; }
        public static GUIStyle indentedBoldFoldout { get; private set; }

        public static GUIContent GetLabel(string text, string tooltip)
        {
            GUIContent output = new GUIContent(text, tooltip);
            return output;
        }

        static SpiralStyles()
        {
            panel = new GUIStyle(EditorStyles.helpBox);

            boldLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12
            };

            smallBoldLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 10
            };

            boxedBoldLabel = new GUIStyle(panel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 10
            };

            boxedBoldLabelCaption = new GUIStyle(panel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            logoLabel = new GUIStyle(boxedBoldLabel);
            logoLabel.normal.textColor = new Color(0.2f, 0.4f, 0.2f);
            logoLabel.alignment = TextAnchor.MiddleRight;

            foldout = new GUIStyle(EditorStyles.foldout);

            indentedFoldout = new GUIStyle(EditorStyles.foldout);
            indentedFoldout.margin.left += 8;

            indentedBoldFoldout = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
            indentedBoldFoldout.margin.left += 8;
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
    }
}
#endif

