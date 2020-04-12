using System.Collections.Generic;
using Spiral.Core;

#if UNITY_EDITOR
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    /// <summary>
    /// Данные, ассоциированные с конкретным экземпляром компонента на сцене
    /// </summary>
    public class ScriptInstanceGID
    {
        /// <summary>
        /// GID экземпляра компонента, позволяющий найти его в файле сцены
        /// </summary>
        public ulong gid { get; }

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
        /// <param name="gid">GID (уникальный идентификатор) экземпляра компонента</param>
        /// <param name="sceneFile">Файл сцены</param>
        public ScriptInstanceGID(ulong gid, SceneFile sceneFile)
        {
            this.gid = gid;
            fileEntry = "";
            List<string> entryList = sceneFile.GetComponentEntry(gid);
            if (entryList == null) return;
            fileEntry = entryList.List2String();
        }
    }
}
#endif
