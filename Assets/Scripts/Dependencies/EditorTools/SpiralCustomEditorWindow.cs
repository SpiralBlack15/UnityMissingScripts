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
    public class SpiralCustomEditorWindow : EditorWindow
    {
        protected Color colorDefault = Color.white;

        private void OnEnable()
        {
            colorDefault = GUI.color;
        }

        protected void OpenStandartBack(Color? color = null, bool includeLogo = true, bool includeScript = true)
        {
            EditorGUILayout.Space();
            if (color == null) SpiralEditor.BeginPanel(PanelType.Vertical);
            else SpiralEditor.BeginPanel(PanelType.Vertical, (Color)color);
            if (includeLogo) SpiralEditor.DrawLogoLine();
            if (includeScript) SpiralEditor.DrawEditorWindowScriptField(this);
        }

        protected void CloseStandartBack()
        {
            SpiralEditor.EndPanel();
        }
    }
}
#endif
