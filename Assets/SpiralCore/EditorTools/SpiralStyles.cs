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
        public static GUIStyle panelIndented { get; private set; }

        // LABELS ---------------------------------------------------------------------------------
        public static GUIStyle labelLogo { get; private set; }
        public static GUIStyle labelSmall { get; private set; }
        public static GUIStyle labelNormal { get; private set; }
        public static GUIStyle labelBold { get; private set; }
        public static GUIStyle labelSmallBold { get; private set; }
        public static GUIStyle labelSmallBoxedBold { get; private set; }
        public static GUIStyle labelSmallBoxed { get; private set; }
        public static GUIStyle labelBoxedBoldCaption { get; private set; }

        // BUTTONS --------------------------------------------------------------------------------
        public static GUIStyle buttonNormal { get; private set; }

        // FOLDOUTS -------------------------------------------------------------------------------
        public static GUIStyle foldoutNormal { get; private set; }
        public static GUIStyle foldoutIndentedNormal { get; private set; }
        public static GUIStyle foldoutIndentedBold { get; private set; }

        public static GUIStyle foldoutPropertyNormal { get; private set; }
        public static GUIStyle foldoutPropertyBold { get; private set; }
        public static GUIStyle foldoutPropertySmall { get; private set; }
        public static GUIStyle foldoutPropertySmallBold { get; private set; }

        // INDENTS --------------------------------------------------------------------------------
        public static int indentFoldoutLeft { get; private set; } = 8;
        public static int indentPropertyFoldoutLeft { get; private set; } = 10;
        public static int indentPanelLeft { get; private set; } = 12;

        // FONT SIZES -----------------------------------------------------------------------------
        public static int fontSizeNormal { get; private set; } = 12;
        public static int fontSizePopupNormal { get; private set; } = 11;
        public static int fontSizeSmall { get; private set; } = 10;

        // POPUP ----------------------------------------------------------------------------------
        public static GUIStyle popupSmall { get; private set; }

        // INITIALIZE STYLES ======================================================================
        // Инициализирует все шорткаты, используемые выше
        //=========================================================================================
        static SpiralStyles()
        {
            // panels -----------------------------------------------------------------------------
            panel = new GUIStyle(EditorStyles.helpBox);

            panelIndented = new GUIStyle(panel);
            panelIndented.margin.left += indentPanelLeft;

            // labels -----------------------------------------------------------------------------
            labelSmall = new GUIStyle(EditorStyles.label)
            {
                fontSize = fontSizeSmall,
                richText = true
            };

            labelNormal = new GUIStyle(EditorStyles.label)
            {
                fontSize = fontSizeNormal,
                richText = true
            };

            labelBold = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = fontSizeNormal,
                richText = true
            };

            labelSmallBold = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = fontSizeSmall,
                richText = true
            };

            labelSmallBoxedBold = new GUIStyle(panel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = fontSizeSmall,
                richText = true
            };

            labelBoxedBoldCaption = new GUIStyle(panel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = fontSizeNormal,
                richText = true
            };

            labelSmallBoxed = new GUIStyle(panel)
            {
                fontSize = fontSizeSmall,
                richText = true
            };

            labelLogo = new GUIStyle(labelSmallBoxedBold);
            labelLogo.normal.textColor = new Color(0.2f, 0.4f, 0.2f);
            labelLogo.alignment = TextAnchor.MiddleRight;

            // buttons ----------------------------------------------------------------------------
            buttonNormal = new GUIStyle(GUI.skin.button)
            {
                fontSize = fontSizeNormal,
                richText = true
            };

            // foldouts ---------------------------------------------------------------------------
            foldoutNormal = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = fontSizeNormal,
                richText = true
            };

            foldoutIndentedNormal = new GUIStyle(foldoutNormal);
            foldoutIndentedNormal.margin.left += indentFoldoutLeft;

            foldoutIndentedBold = new GUIStyle(foldoutNormal)
            {
                fontStyle = FontStyle.Bold,
            };
            foldoutIndentedBold.margin.left += indentFoldoutLeft;

            // foldout property -------------------------------------------------------------------
            foldoutPropertyNormal = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = fontSizeNormal,
                richText = true
            };
            foldoutPropertyNormal.margin.left += indentPropertyFoldoutLeft;

            foldoutPropertySmall = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = fontSizeSmall,
                richText = true
            };
            foldoutPropertySmall.margin.left += indentPropertyFoldoutLeft;

            foldoutPropertyBold = new GUIStyle(foldoutPropertyNormal)
            {
                fontStyle = FontStyle.Bold
            };

            foldoutPropertySmallBold = new GUIStyle(foldoutPropertySmall)
            {
                fontStyle = FontStyle.Bold
            };

            // popups -----------------------------------------------------------------------------
            popupSmall = new GUIStyle(EditorStyles.popup)
            {
                fontSize = fontSizePopupNormal,
                richText = false // lol, we have troubles here
            };

            // hex colors -------------------------------------------------------------------------
            hexDarkRed = SpiralEditor.GetColorHex(0.7f, 0.0f, 0.0f, 1.0f);
        }
    }
}
#endif