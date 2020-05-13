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
        public static string hexDarkRed  { get; private set; } 

        public static Color defaultLogoColor   { get; private set; } = new Color(0.7f, 0.8f, 0.7f);
        public static Color defaultPanelColor  { get; private set; } = new Color(0.7f, 0.7f, 0.7f);
        public static Color defaultButtonColor { get; private set; } = new Color(0.9f, 0.9f, 0.9f);

        public static Color colorLightRed    { get; private set; } = new Color(0.9f, 0.7f, 0.7f);
        public static Color colorLightGreen  { get; private set; } = new Color(0.7f, 0.9f, 0.7f);
        public static Color colorLightBlue   { get; private set; } = new Color(0.5f, 0.7f, 0.8f);
        public static Color colorLightYellow { get; private set; } = new Color(0.9f, 0.9f, 0.7f);
        public static Color colorLightOrange { get; private set; } = new Color(0.8f, 0.9f, 0.7f);

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
        public static int indentedPanelLeftIndent { get; private set; } = 12;

        // POPUP ----------------------------------------------------------------------------------
        public static GUIStyle miniPopupFont { get; private set; }

        // INITIALIZE STYLES ======================================================================
        // Инициализирует все шорткаты, используемые выше
        //=========================================================================================
        static SpiralStyles()
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
            foldout = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 12,
                richText = true
            };

            indentedFoldout = new GUIStyle(foldout);
            indentedFoldout.margin.left += indentedFoldoutLeftIndent;

            indentedBoldFoldout = new GUIStyle(foldout)
            {
                fontStyle = FontStyle.Bold,
            };
            indentedBoldFoldout.margin.left += indentedFoldoutLeftIndent;

            // popups -----------------------------------------------------------------------------
            miniPopupFont = new GUIStyle(EditorStyles.popup)
            {
                fontSize = 11,
                richText = false // lol, we have troubles here
            };

            // colors -----------------------------------------------------------------------------
            hexDarkRed = SpiralEditor.GetColorHex(0.7f, 0.0f, 0.0f, 1.0f);
        }
    }
}
#endif