using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class ScriptGUID
    {
        /// <summary>
        /// GUID, ассоциированный со скриптом
        /// </summary>
        public string guid { get; }

        /// <summary>
        /// Все мёртвые объекты со скриптом этого вида
        /// </summary>
        public List<ObjectID> oids { get; } = new List<ObjectID>();

        /// <summary>
        /// Все MonoBehaviour со скриптом этого вида
        /// </summary>
        public List<ScriptInstanceGID> gids { get; } = new List<ScriptInstanceGID>();

        // EDITOR WINDOW STUFF --------------------------------------------------------------------
        public bool isDead { get; }
        public bool showInfo { get; set; } = false;

        public ScriptGUID(string guid, bool isDead)
        {
            this.isDead = isDead;
            this.guid = guid;
        }
    }
}
#endif
