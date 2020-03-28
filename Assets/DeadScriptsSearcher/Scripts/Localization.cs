#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Spiral.EditorTools.DeadScriptsSearcher
{
    // SUPER EASY LOCALIZATION TIME
    public enum Language { RU, ENG }
    public struct LocalString
    {
        public string RU  { get; private set; }
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

                default: return ENG;
            }
        }

        public static implicit operator string(LocalString local)
        {
            return local.Read(Localization.lang);
        }
    }

    public static class Localization
    {
        public static Language lang = Language.RU;

#if UNITY_EDITOR
        public static void DrawLanguageSelect()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            lang = (Language)EditorGUILayout.EnumPopup(strLocal, lang);
            EditorGUILayout.EndVertical();
        }
#endif

        // LOCALIZATION ===========================================================================
        // Simple localization to those who do not understand Russian :)
        //=========================================================================================
#region Localization
        public readonly static LocalString strLocal = new LocalString(
            // ru
            "ЯЗЫК:",
            // en
            "LANGUAGE:");

        public readonly static LocalString strEditorWindow_DeadScriptSearcher = new LocalString(
            // ru
            "Поиск мёртвых скриптов",
            // en
            "Dead Scripts Searcher");
        public readonly static LocalString strEditorWindow_MonoView = new LocalString(
            // ru
            "Ревизор",
            // en
            "Auditor");

        public readonly static LocalString strDebugMode = new LocalString(
            // ru
            "Режим отладки",
            // en
            "Debug mode");
        public readonly static LocalString strDebugModeHelp = new LocalString(
            // ru
            "Режим отладки будет выводить в консоль все действия, что может " +
            "существенно замедлить осмотр сцен с большим количеством объектов",
            // en
            "Debug mode will output all actions to the console. This can " +
            "significantly slow down the checkup of scenes " +
            "with a large number of objects");
        public readonly static LocalString strObjectsOnly = new LocalString(
            // ru
            "Только объекты",
            // en
            "Objects only:"
            );
        public readonly static LocalString strObjectsOnlyButton = new LocalString(
            // ru
            "Найти и выделить объекты с мёртвыми скриптами",
            // en
            "Find and select objects with dead scripts"
            );
        public readonly static LocalString strSceneFileCheckout = new LocalString(
            // ru
            "Проверка файла сцены:",
            // en
            "Scene file checkout:"
            );
        public readonly static LocalString strSceneWasChanged = new LocalString(
           // ru
           "СЦЕНА БЫЛА ИЗМЕНЕНА (сохраните изменения)",
           // en
           "SCENE WAS CHANGED (save changes first)"
           );
        public readonly static LocalString strSceneClear = new LocalString(
           // ru
           "СЦЕНА СОХРАНЕНА",
           // en
           "SCENE SAVED"
           );
        public readonly static LocalString strShowHelp = new LocalString(
            // ru
            "Показать/Скрыть справку",
            // en
            "Show/Hide help");
        public readonly static LocalString strSceneHelpWarning = new LocalString(
            // ru
            "Убедитесь, что сцена была сохранена!\n" +
            "Для сцен с большим количеством объектов поиск может идти медленно.\n" +
            "Если объектов больше сотни, это может занять время.",
            // en
            "Make sure the scene has been saved!\n" +
            "For scenes with a large number of objects, the search may go slowly.\n" +
            "If there are more than a hundred objects, this may take some time.");
        public readonly static LocalString strSceneHelpExplanation = new LocalString(
            // ru
            "Поиск идёт по файлу сцены, сопоставляя объекты с " +
            "битыми скриптами с их записями в файле. " +
            "Обратите внимание, что из поиска исключены скрипты, " +
            "не являющиеся MonoBehaviour, а также дочерние объекты в составе префабов.",
            // en
            "The search goes through the scene file, matching objects with broken scripts " +
            "with their entries in the file. " +
            "Note that scripts that are not MonoBehaviour, as well as child objects in prefabs, " +
            "are excluded from the search.");
        public readonly static LocalString strFindDeadGUIDs = new LocalString(
            // ru
            "Найти мёртвые GUID в сцене:",
            // en
            "Find dead GUIDs in the scene:");
        public readonly static LocalString strFoundGUIDs = new LocalString(
            // ru
            "Найдено уникальных GUID: ",
            // en
            "Unique GUIDs found: "
            );
        public readonly static LocalString strShowList = new LocalString(
            // ru
            "Показать/Скрыть список",
            // en
            "Show/Hide list"
            );
        public readonly static LocalString strDeadObjectsCount = new LocalString(
            // ru
            "Объектов",
            // en
            "Objects count");
        public readonly static LocalString strSelectObjects = new LocalString(
            // ru
            "Выделить все объекты с этим скриптом",
            // en
            "Select all objects with this script");
        public readonly static LocalString strSelectObject = new LocalString(
            // ru
            "Выделить объект",
            // en
            "Select target object");

        public readonly static LocalString strProgressBar_SearchDeadObject = new LocalString(
            // ru
            "Поиск мёртвых объектов на сцене",
            // en
            "Search dead objects");
        public readonly static LocalString strProgressBar_SearchingScene = new LocalString(
            // ru
            "Поиск по сцене",
            // en
            "Searching scene");
        public readonly static LocalString strProgressBar_SearchingSceneFile = new LocalString(
            // ru
            "Поиск по файлу сцены",
            // en
            "Searching scene file");
        public readonly static LocalString strProgressBar_InspectedObject = new LocalString(
            // ru
            "Проверяем объект: ",
            // en
            "Inspected object: ");
#endregion
    }

}

