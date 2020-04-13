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
using UnityEngine;
using UnityEngine.SceneManagement;
using Spiral.Core;
using static Spiral.EditorTools.DeadScriptsSearcher.Localization;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public static class DeadScripts
    {
        public static bool isDebugMode = false;
        private static SceneFile sceneFile { get { return SceneFile.current; } }
        public static List<ObjectID> deadOIDs  { get; private set; } = new List<ObjectID>();
        public static List<ComponentGUID> deadGUIDs { get; private set; } = new List<ComponentGUID>();
        public static bool sceneFileLoaded
        {
            get
            {
                if (sceneFile == null) return false;
                if (sceneFile.count == 0) return false;
                return true;
            }
        }
        public static bool isDirty { get { return SceneManager.GetActiveScene().isDirty; } }

        // FUNCTIONALITY --------------------------------------------------------------------------
        public static void SelectDeads()
        {
            ObjectID.Select(deadOIDs);
        }

        public static void UpdateDeadList()
        {
            SceneFile.ReloadCurrentSceneFile();
            var objects = CoreFunctions.CollectScene().Transforms2GameObjects();
            int count = objects.Count;

            if (deadOIDs == null) deadOIDs = new List<ObjectID>();
            else if (deadOIDs.Count != 0) deadOIDs.Clear();

            for (int i = 0; i < count; i++)
            {
                float progress = i * 1f / count;
                EditorUtility.DisplayProgressBar(strProgressBar_SearchDeadObject,
                                                 strProgressBar_SearchingScene + $"{i} / {count} " + strProgressBar_SearchingSceneObjects,
                                                 progress);

                GameObject go = objects[i];
                int missings = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (missings == 0) continue;

                // мы создаём ObjectID в этом режиме ТОЛЬКО для объектов, у которых
                // обнаужены MissingScripts, чтобы обыск сцены не занимал слишком много
                // времени
                ObjectID objectID = new ObjectID(go, isDebugMode);
                deadOIDs.Add(objectID);
            }
            EditorUtility.ClearProgressBar();

            if (deadOIDs.Count == 0)
            {
                Debug.Log($"<color=green>Everything is okay :)</color>");
            }
        }

        public static void SearchForDeads()
        {
            UpdateDeadList();
            deadGUIDs = new List<ComponentGUID>();

            int count = deadOIDs.Count; // список deadOIDs сформирован функцией UpdateDeadList()
            for (int i = 0; i < count; i++)
            {
                EditorUtility.DisplayProgressBar(strProgressBar_ObjectsFound + $"{count}",
                                                 strProgressBar_InspectedObject + $"{i} / {count}", 
                                                 i * 1f / count);
                ObjectID oid = deadOIDs[i];

                // идём по всем компонентам, т.к. у нас списки листов по индексам ассоциированы
                // TODO: возможно, имеет смысл оптимизировать всё, что ниже
                for (int g = 0; g < oid.componentFileIDs.Count; g++)
                {
                    if (oid.componentLiveStatus[g]) continue; // пропускаем живые компоненты

                    string guid = oid.componentGUIDs[g];
                    if (string.IsNullOrEmpty(guid)) continue; // пропускаем компоненты из префабов и/или по другим причинам не имеющих GUID 

                    // создаём учётку для этого компонента 
                    ulong fileID = oid.componentFileIDs[g];
                    ComponentData deadFileID = new ComponentData(fileID, sceneFile);

                    // проверяем, есть ли уже учётка с таким GUID или нет 
                    // не путайте GID- и GUID- учётки: первая описывает конкретный экземпляр скрипта,
                    // а вторая - группу скриптов, имеющих одинаковый GUID
                    int guidIDX = deadGUIDs.FindIndex(x => x.guid == guid);
                    if (guidIDX >= 0) // добавляем мёртвый компонент и его объект к уже существующей учётке
                    {
                        deadGUIDs[guidIDX].oids.Add(oid);
                        deadGUIDs[guidIDX].gids.Add(deadFileID);
                    }
                    else // создаём новую GUID-учётку
                    {
                        ComponentGUID deadGUID = new ComponentGUID(guid, true);
                        deadGUIDs.Add(deadGUID);
                        deadGUID.oids.Add(oid);
                        deadGUID.gids.Add(deadFileID);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            if (isDebugMode)
            {
                for (int i = 0; i < deadGUIDs.Count; i++)
                {
                    Debug.Log($"Dead GUID <b>#{i}</b>: <i><color=red>{deadGUIDs[i]}</color></i> " +
                              $"(Scripts Broken: {deadGUIDs[i].oids.Count})");
                }
            }
        }
    }
}
#endif