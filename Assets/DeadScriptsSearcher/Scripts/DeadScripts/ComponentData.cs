// *********************************************************************************
// The MIT License (MIT)
// Copyright (c) 2020 BlackSpiral https://github.com/BlackSpiral15
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

using System.Collections.Generic;
using Spiral.Core;

#if UNITY_EDITOR
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    /// <summary>
    /// Данные, ассоциированные с конкретным экземпляром компонента на сцене
    /// </summary>
    public class ComponentData
    {
        /// <summary>
        /// FileID экземпляра компонента, позволяющий найти его в файле сцены
        /// </summary>
        public ulong fileID { get; }

        /// <summary>
        /// Соответствующая компоненту запись в файле сцены
        /// </summary>
        public string fileEntry { get; }

        /// <summary>
        /// Флаг для EditorWindow
        /// </summary>
        public bool showInfo = false;

        /// <summary>
        /// Создаёт учётку скрипта
        /// </summary>
        /// <param name="fileID">GID (уникальный идентификатор) экземпляра компонента</param>
        /// <param name="sceneFile">Файл сцены</param>
        public ComponentData(ulong fileID, SceneFile sceneFile)
        {
            this.fileID = fileID;
            fileEntry = "";
            List<string> entryList = sceneFile.GetComponentEntry(fileID);
            if (entryList == null) return;
            fileEntry = entryList.List2String();
        }
    }
}
#endif
