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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Spiral.EditorToolkit
{
    public enum Language { RU, ENG }
    public struct LocalString
    {
        public string RU { get; private set; }
        public string ENG { get; private set; }

        public LocalString(string RU, string ENG)
        {
            this.RU = RU;
            this.ENG = ENG;
        }
        private string Read(Language local)
        {
            switch (local)
            {
                case Language.RU: return RU;
                case Language.ENG: return ENG;

                default: Debug.LogWarning($"Language {local} not found"); return ENG;
            }
        }

        public static implicit operator string(LocalString local)
        {
            return local.Read(SpiralLocalization.language);
        }
    }

    public static class SpiralLocalization
    {
        private static Language m_language = Language.ENG;
        public static Language language
        {
            get { return m_language; }
            set
            {
                if (m_language == value) return;
                onLanguageChanged?.Invoke();
                m_language = value;
            }
        }

        public static event Action onLanguageChanged;

        public readonly static LocalString strLocal = new LocalString(
            // ru
            "ЯЗЫК:",
            // en
            "LANGUAGE:"
            );

#if UNITY_EDITOR
        public static void DrawLanguageSelect()
        {
            SpiralEditor.BeginPanel(GroupType.Vertical);
            language = (Language)EditorGUILayout.EnumPopup(strLocal, language);
            SpiralEditor.EndPanel();
        }
#endif
    }
}
