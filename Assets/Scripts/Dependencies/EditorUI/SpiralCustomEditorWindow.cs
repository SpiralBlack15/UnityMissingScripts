// *********************************************************************************
// The MIT License (MIT)
// Copyright (c) 2020 SpiralBlack https://github.com/SpiralBlack15
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorToolkit
{
    public class SpiralCustomEditorWindow : EditorWindow
    {
        protected Color defaultColor;

        protected void OpenStandartBack(bool includeLogo = true, bool includeScript = true, Color? background = null)
        {
            defaultColor = GUI.color;
            EditorGUILayout.Space();
            GUI.color = background != null ? (Color)background : SpiralStyles.defaultPanelColor;
            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            GUI.color = defaultColor;
            if (includeLogo) SpiralStyles.DrawLogoLine();
            if (includeScript) DrawEditorWindowScriptField();
        }

        protected void CloseStandartBack()
        {
            EditorGUILayout.EndVertical();
            GUI.color = defaultColor;
        }

        protected void DrawEditorWindowScriptField()
        {
            EditorGUILayout.BeginVertical(SpiralStyles.panel);
            GUI.enabled = false;
            Type type = GetType();
            MonoScript monoScript = MonoScript.FromScriptableObject(this);
            if (monoScript != null)
            {
                _ = EditorGUILayout.ObjectField("Editor", monoScript, type, false);
            }
            else
            {
                EditorGUILayout.LabelField("No editor single file found", SpiralStyles.panel);
            }
            GUI.enabled = true;
            EditorGUILayout.EndVertical();
        }
    }
}
#endif
