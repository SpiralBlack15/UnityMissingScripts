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
    /// Учётная запись для одного GUID'a (в основном для мёртвого, см. DeadScripts.cs).
    /// Собирает в себе все объекты, содержащие компоненты с этим GUID, а также сами
    /// компоненты. 
    /// </summary>
    public class ComponentGUID
    {
        /// <summary>
        /// GUID, ассоциированный с компонентом.
        /// GUID определяет идентификатор типа компонента, т.е. два
        /// компонента одного типа будут иметь одинаковый GUID. Так, 
        /// зная GUID мёртвого компонента, мы можем узнать, что два 
        /// потерянных компонента принадлежат (или не принадлежат)
        /// одному и тому же пропавшему скрипту.
        /// </summary>
        public string guid { get; }

        /// <summary>
        /// Все мёртвые объекты со скриптом этого вида
        /// </summary>
        public List<ObjectID> oids { get; } = new List<ObjectID>();

        /// <summary>
        /// Все экземпляры компонент со скриптом этого вида
        /// </summary>
        public List<ComponentData> gids { get; } = new List<ComponentData>();

        /// <summary>
        /// GUID принадлежит мёртвому скрипту
        /// </summary>
        public bool isDead { get; }

        /// <summary>
        /// Флаг для EditorWindow
        /// </summary>
        public bool showInfo { get; set; } = false;

        public ComponentGUID(string guid, bool isDead)
        {
            this.isDead = isDead;
            this.guid = guid;
        }
    }
}
#endif
