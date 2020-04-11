using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Spiral.EditorTools.DeadScriptsSearcher.Localization;

namespace Spiral.EditorTools.DeadScriptsSearcher
{
    public class SceneFile
    {
        /// <summary>
        /// Разделитель
        /// </summary>
        public static readonly string unitSeparator = "--- !u!";

        /// <summary>
        /// Разделитель для GameObject
        /// </summary>
        public static readonly string unitGameObject = "--- !u!1 &";

        /// <summary>
        /// Разделитель для скриптов Mono Behaviour 
        /// </summary>
        public static readonly string unitMonoScript = "--- !u!114 &";

        /// <summary>
        /// m_Script поле компонента
        /// </summary>
        public static readonly string unitMonoScriptField = "m_Script";

        /// <summary>
        /// m_Component поле
        /// </summary>
        public static readonly string unitComponent = "m_Component";

        // FUNCTIONALITY ==========================================================================
        //=========================================================================================
        private Scene inspectedScene;
        private readonly List<string> file = new List<string>();
        public int count { get { return file.Count; } }
        public string this[int idx]
        {
            get
            {
                if (idx >= count) return string.Empty;
                if (idx < 0) return string.Empty;
                return file[idx];
            }
            private set
            {
                if (idx >= count) Debug.LogWarning("IDX is out of bound");
                if (idx < 0) Debug.LogWarning("IDX is out of bound");
                file[idx] = value;
            }
        }

        public void SaveScene(string newName = "")
        {
            string scenePath = inspectedScene.path;
            FileInfo fileInfo = new FileInfo(scenePath);

            if (string.IsNullOrEmpty(newName)) newName = fileInfo.Name;
            if (!newName.Contains(".unity")) newName += ".unity";
            newName = fileInfo.DirectoryName + @"\" + newName;
            scenePath = newName;

            File.WriteAllLines(scenePath, file.ToArray());
        }

        public SceneFile(Scene scene)
        {
            inspectedScene = scene;
            file = GetSceneText(scene);
        }

        public SceneFile()
        {
            inspectedScene = SceneManager.GetActiveScene();
            file = GetSceneText(inspectedScene);
        }

        /// <summary>
        /// Находит индекс строки в файле сцены 
        /// </summary>
        /// <param name="oid">Идентификатор объекта</param>
        /// <returns>Номер строки в файле сцены, откуда начинается объект
        /// -1, если объекта нет
        /// -2, если объект находится в составе префаба</returns>
        public int IndexOfObject(ObjectID oid)
        {
            int strIDX = file.FindIndex(x => x.Contains($"&{oid.id}"));
            if (strIDX < 0)
            {
                if (oid.globalID.targetObjectId == 0) return -1;
                else return -2;
            }
            return strIDX;
        }

        /// <summary>
        /// Находит скрипт по его GID'у
        /// По факту ищет заголовок вида: 
        /// --- !u!114 &2049595391
        /// </summary>
        /// <param name="componentGID"></param>
        /// <returns></returns>
        public int IndexOfComponent(ulong componentGID)
        {
            return file.FindIndex(x => x.Contains($"&{componentGID}"));
        }

        /// <summary>
        /// Получить информацию об экземпляре компонента по его идентификатору
        /// </summary>
        /// <param name="componentGID">GID компонента (идентификатор в файле сцены)</param>
        /// <returns>Ассоциировнная запись в файле сцены</returns>
        public List<string> ComponentInfo(ulong componentGID)
        {
            int cursor = IndexOfComponent(componentGID);
            if (cursor < 0) return null;
            List<string> output = new List<string>(); 
            string currentLine = file[cursor];
            output.Add(currentLine);
            cursor++;
            while (cursor < count)
            {
                currentLine = file[cursor];
                if (currentLine.Contains(unitSeparator)) { break; }
                output.Add(currentLine);
                cursor++;
            }
            return output;
        }

        /// <summary>
        /// Выуживает все GID компонентов с объекта
        /// </summary>
        /// <param name="oid">Объектный ID</param>
        /// <param name="debug">Выводить в строку отладки (замедляет работу!)</param>
        /// <returns>null если объект не был найден, пустой лист, если у объекта нет GIDов</returns>
        public List<ulong> GetCGIDs(ObjectID oid, bool debug)
        {
            int strIDX = IndexOfObject(oid);
            if (strIDX < 0) return null;
            int cursor = strIDX + 2; // сдвигаем сразу на две позиции
            return GetComponentsGIDs(cursor, debug);
        }

        /// <summary>
        /// Берёт guid m_Script'a
        /// </summary>
        /// <param name="componentGID">GID компонента в файле сцены</param>
        /// <param name="debug">Выводить в строку отладки (замедляет работу!)</param>
        /// <returns></returns>
        public string GetGUID(ulong componentGID, bool debug)
        {
            int strIDX = IndexOfComponent(componentGID);
            if (strIDX < 0)
            {
                if (debug) Debug.Log($"Component GID {componentGID} not found");
                return string.Empty;
            }

            string line;
            int cursor = strIDX;
            line = file[cursor];
            if (!IsMono(line, false, 0))
            {
                if (debug) Debug.Log($"Script is not mono: {line}");
                return string.Empty;
            }

            int eof = count - 1;
            cursor += 2;
            bool mscriptFound = false;
            while (cursor < eof)
            {
                cursor++; line = file[cursor];
                if (line.Contains(unitSeparator)) { break; }
                if (line.Contains(unitMonoScriptField)) { mscriptFound = true; break; }
            }

            if (!mscriptFound)
            {
                if (debug) Debug.Log($"m_Script not found: {line}");
                return string.Empty;
            }

            string guid = GetGUIDFromLine(line);
            return guid;
        }

        /// <summary>
        /// Возвращает все GIDs в разделителе, начиная с нужного места
        /// Таким образом мы устанавливаем, на какой MonoBehaviour ссылаются конкретно взятые компоненты,
        /// чтобы потом найти уже их
        /// Пример ниже:
        /// m_Component:
        ///     - component: {fileID: 519420032}
        ///     - component: {fileID: 519420031}
        ///     - component: {fileID: 519420029}
        ///     - component: {fileID: 2049595390}
        ///     - component: {fileID: 2049595391}
        /// </summary>
        /// <param name="fromIDX">С какой строки начинаем чтение</param>
        /// <param name="debug">Выводить в строку отладки (замедляет работу!)</param>
        /// <returns></returns>
        private List<ulong> GetComponentsGIDs(int fromIDX, bool debug)
        {
            string currentLine;   // читалка строки
            int cursor = fromIDX; // текущая позиция в файле

            int eofIDX = count;

            List<ulong> gids = new List<ulong>();

            // идём от GameObject до конца файла
            while (cursor < eofIDX) 
            {
                cursor++; currentLine = file[cursor];

                // дошли до следующего разделителя - дропаемся
                if (currentLine.Contains(unitSeparator)) { break; }

                // нашли компонент?
                if (currentLine.Contains("m_Component"))
                {
                    while (cursor < eofIDX)
                    {
                        cursor++; currentLine = file[cursor];
                        if (!currentLine.Contains("component")) break; // вышли из списка компонент
                        ulong gid = GetFileIDFromLine(currentLine);
                        gids.Add(gid);
                    }
                    break;
                }
            }

            if (gids.Count == 0 && debug) { Debug.Log("GIDs count is 0"); }

            return gids;
        }

        // STATICS ================================================================================
        /// <summary>
        /// Возвращает текст сцены
        /// </summary>
        /// <returns>Сцена в виде списка строк</returns>
        public static List<string> GetSceneText(Scene scene)
        {
            string scenePath = scene.path;
            string[] read = File.ReadAllLines(scenePath);
            return new List<string>(read);
        }

        /// <summary>
        /// Извлекает GUID из файла сцены из строчки вида
        /// 
        /// ВНИМАНИЕ: парсер расчитан под формат версии 2019.2, 
        /// при изменении формата сцены в более поздних версиях 
        /// рекомендуется проверить и переписать.
        /// 
        /// Строчка должна иметь вид:
        /// m_Script: {fileID: 11500000, guid: fcc1ec5a861f29f4c83d69421ec0ce56, type: 3}
        /// </summary>
        /// <param name="line">Строчка файла</param>
        /// <returns>GUID mono script'a строкой</returns>
        public static string GetGUIDFromLine(string line)
        {
            if (!line.Contains("guid")) throw new FormatException(str_DebugGUIDNotFound);

            var strsplit = line.Split(new string[] { "guid:" }, StringSplitOptions.None);
            strsplit = strsplit[1].Split(',');
            string guid = strsplit[0].Trim();

            return guid;
        }

        /// <summary>
        /// Извлекает ScriptID из файла сцены 
        /// 
        /// ВНИМАНИЕ: парсер расчитан под формат версии 2019.2, 
        /// при изменении формата сцены в более поздних версиях 
        /// рекомендуется проверить и переписать.
        /// 
        /// Строка должна иметь следующий вид:
        /// --- !u!114 &64005245
        /// </summary>
        /// <param name="line">Строка файла</param>
        /// <returns>ScriptID строкой</returns>
        public static ulong GetScriptIDFromLine(string line)
        {
            try
            {
                string capstr = line.Split('&')[1]; 
                // отрезаем строковую часть т.к. там может быть stripped: --- !u!114 &1933180445 stripped
                capstr = capstr.Split(' ')[0];
                string scriptID = capstr.Trim();
                ulong answer = Convert.ToUInt64(scriptID);
                return answer;
            }
            catch
            {
                Debug.Log($"<color=red><b>ERROR FORMAT: {line}</b></color>");
                return 0;
            }
        }

        /// <summary>
        /// Извлекает fileID (GID) из строчки файла
        /// </summary>
        /// <param name="line">Входная строка</param>
        /// <param name="splitAfter">Разделитель справа от fileID</param>
        /// <returns>GID (уникальный идентификатор) объекта или компонента в файле сцены</returns>
        public static ulong GetFileIDFromLine(string line, char splitAfter = '}')
        {
            try
            {
                if (!line.Contains("fileID:")) throw new FormatException(str_DebugGUIDNotFound);

                var strsplit = line.Split(new string[] { "fileID:" }, StringSplitOptions.None);
                strsplit = strsplit[1].Split(splitAfter);
                ulong gid = Convert.ToUInt64(strsplit[0].Trim());
                return gid;
            }
            catch
            {
                Debug.Log($"<color=red><b>ERROR FORMAT: {line}</b></color>");
                return 0;
            }
        }

        /// <summary>
        /// Проверяет, что читаемая строчка является заголовком MonoBehaviour'a
        /// </summary>
        /// <param name="line">Читаемая строка</param>
        /// <param name="dbg">(дебаг)</param>
        /// <param name="scriptID">Номер скрипта (для дебага)</param>
        /// <returns></returns>
        private static bool IsMono(string line, bool dbg = false, ulong scriptID = 0)
        {
            bool ismono = line.Contains(unitMonoScript);
            if (!ismono && dbg) { Debug.Log($"ScriptID <color=blue>{scriptID}</color> is not Mono"); }
            return ismono;
        }
    }
}

