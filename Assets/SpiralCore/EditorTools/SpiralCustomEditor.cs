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
    public abstract class SpiralCustomEditor : Editor
    {
        protected Color colorDefault = Color.white;

        private void OnEnable()
        {
            colorDefault = GUI.color;
        }

        protected void OpenStandartBack(Color? color = null, bool includeLogo = true, bool includeScript = true)
        {
            EditorGUILayout.Space();
            if (color == null) SpiralEditor.BeginPanel(GroupType.Vertical);
            else SpiralEditor.BeginPanel(GroupType.Vertical, (Color)color);
            if (includeLogo) SpiralEditor.DrawLogoLine();
            if (includeScript)
            {
                SpiralEditor.DrawScriptField(serializedObject);

                MonoScript editorMono = GetEditorMono();
                if (editorMono != null) SpiralEditor.DrawScriptField(editorMono, "Editor");
            }
        }

        protected void CloseStandartBack()
        {
            SpiralEditor.EndPanel();
        }

        protected static MonoScript CahsedMono<T>(ref MonoScript monoScript) // да, здесь действительно нужен ref
        {
            if (monoScript == null)
            {
                monoScript = SpiralEditorTools.GetMonoScript(typeof(T));
            }
            return monoScript;
        }

        protected abstract MonoScript GetEditorMono();
    }
}
#endif
