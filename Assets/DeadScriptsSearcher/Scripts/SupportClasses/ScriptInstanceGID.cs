using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class ScriptInstanceGID
    {
        /// <summary>
        /// GID скрипта (metadata token)
        /// </summary>
        public ulong gid { get; }

        /// <summary>
        /// Запись в файле сцены
        /// </summary>
        public string fileEntry { get; }

        /// <summary>
        /// Показать информацию
        /// </summary>
        public bool showInfo = false;

        public ScriptInstanceGID(ulong gid, SceneFile sceneFile)
        {
            this.gid = gid;
            fileEntry = "";

            List<string> entryList = sceneFile.ComponentInfo(gid);
            if (entryList == null) return;
            fileEntry = entryList.List2String();
        }
    }
}
#endif
