using Spiral.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Spiral.EditorToolkit
{
    public struct Borders
    {
        public float xLeft;
        public float xRight;
        public float yTop;
        public float yBottom;

        public float bothY { get { return yTop + yBottom; } }
        public float bothX { get { return xLeft + xRight; } }
        public Borders(float xl, float yt, float xr, float yb)
        {
            xLeft = xl;
            xRight = xr;
            yTop = yt;
            yBottom = yb;
        }
    }

    public class SpiralPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Объект, извлечённый из сериализованного свойства.
        /// </summary>
        protected object target = null; // TODO: подумать, как бы нам не брать его стопицот раз

        // GEOMETRY DATA --------------------------------------------------------------------------
        // Геометрические параметры для свойства
        //-----------------------------------------------------------------------------------------
        #region GEOMETRY
        /// <summary>
        /// Исходный прямоугольник проперти
        /// </summary>
        protected Rect baseRect { get; private set; }

        /// <summary>
        /// Координаты проперти с учётом отступа GUI при вложенных проперти
        /// </summary>
        protected Rect indentedRect { get; private set; }

        /// <summary>
        /// Ширина отступа GUI вправо от корневого при вложенных проперти
        /// </summary>
        protected float guiIndentX { get; private set; }

        /// <summary>
        /// Верхний левый край проперти с учётом отступа GUI
        /// Не учитывает обрамление
        /// </summary>
        protected Vector2 indentedStart { get; private set; }

        /// <summary>
        /// Ширина и высота проперти с учётом отступа GUI
        /// </summary>
        protected Vector2 indentedSize { get; private set; }

        /// <summary>
        /// Ширина и высота проперти с учётом внутреннего отступа
        /// </summary>
        protected Vector2 innerSize { get; private set; }

        /// <summary>
        /// Стандартная высота элементов.
        /// Устанавливается через переписывание ElementHeight и последующий
        /// вызов пересчёта координат
        /// </summary>
        protected float elementHeight { get; private set; } = 18;
        /// <summary>
        /// Размер элемента
        /// </summary>
        /// <returns></returns>
        protected virtual float GetElementHeight() { return 18; }

        /// <summary>
        /// Отступ вовне (ака рамка) для проперти, имеющих подложку
        /// </summary>
        protected Borders outline { get; private set; } = new Borders(2, 3, 2, 3);
        /// <summary>
        /// Обводка, если нужна
        /// </summary>
        /// <returns></returns>
        protected virtual Borders GetOutline() { return new Borders(2, 3, 2, 3); }

        /// <summary>
        /// Ширина одного столбца сетки (устанавливается при инициализации сетки) 
        /// </summary>
        protected float columnWidth { get; private set; } = 1;

        /// <summary>
        /// Отступ вовнутрь для проперти, имеющих подложку
        /// </summary>
        protected Borders innerStroke { get; private set; } = new Borders(5, 7, 5, 5);
        /// <summary>
        /// Отступ вовнутрь поля
        /// </summary>
        /// <returns></returns>
        protected virtual Borders GetInnerStroke() { return new Borders(5, 7, 5, 5); }

        /// <summary>
        /// Отступы в сетке (устанавливается при инициализации сетки) 
        /// </summary>
        protected Vector2 gridSpace { get; private set; } = new Vector2(3, 3);
        /// <summary>
        /// Отступы в сетке
        /// </summary>
        /// <returns></returns>
        protected virtual Vector2 GetGridSpace() { return new Vector2(2, 3); }

        /// <summary>
        /// Координаты X столбцов сетки (если инициализирована)
        /// </summary>
        protected float[] gridColumnsX;

        /// <summary>
        /// Координаты Y столбцов сетки (если инциаилизиована)
        /// </summary>
        protected float[] gridRowsY;
        #endregion

        // SET GEOMETRY ===========================================================================
        // Перезапиши эти функции, если необходимо установить собственную геометрию для проперти
        //=========================================================================================
        /// <summary>
        /// Количество строк для проперти-сетки
        /// </summary>
        /// <returns></returns>
        protected virtual int GridRowCount() { return 3; }

        /// <summary>
        /// Количество колонок для проперти-сетки
        /// </summary>
        /// <returns></returns>
        protected virtual int GridColumnCount() { return 2; }

        // GET GEOMETRY ===========================================================================
        // Различные варианты взятия геометри кастомного проперти
        //=========================================================================================
        /// <summary>
        /// Для проперти с сеткой
        /// </summary>
        /// <param name="offset">Настраиваемый оффсет</param>
        /// <param name="outlined">Имеет обводку</param>
        /// <returns></returns>
        protected float GetGridedPropertyHeight(float offset = 0, bool outlined = true)
        {
            float rows = GridRowCount();

            float height = innerStroke.yTop +
                           elementHeight * rows +
                           gridSpace.y * (rows - 1);
            if (outlined) height += outline.yTop + outline.yBottom;

            return height + offset;
        }

        /// <summary>
        /// Для проперти в одну строчку
        /// </summary>
        /// <param name="offset">Настраиваемый оффсет</param>
        /// <param name="outlined">Имеет обводку</param>
        /// <returns></returns>
        protected float GetSingleLinePropertyHeight(float offset = 0, bool outlined = true)
        {
            float result = elementHeight + innerStroke.bothY;
            if (outlined) result += outline.bothY;
            return result + offset;
        }

        /// <summary>
        /// Берёт верхний угол указанной строки
        /// </summary>
        /// <param name="row"></param>
        /// <param name="indented"></param>
        /// <param name="outlined"></param>
        /// <returns></returns>
        protected float GetDirectRowY(int row, float offset = 0, bool indented = true, bool outlined = true)
        {
            float result = innerStroke.yTop + elementHeight * row + gridSpace.y * (row - 1);
            result += indented ? indentedStart.y : baseRect.y;
            if (outlined) result += outline.yTop;
            return result + offset;
        }

        /// <summary>
        /// Берёт левый угол указанного столбца
        /// </summary>
        /// <param name="col"></param>
        /// <param name="indented"></param>
        /// <param name="outlined"></param>
        /// <returns></returns>
        protected float GetDirectColumnX(int col, float offset = 0, bool indented = true, bool outlined = true)
        {
            float result = innerStroke.xLeft + columnWidth * col + gridSpace.x * (col - 1);
            result += indented ? indentedStart.x : baseRect.x;
            if (outlined) result += outline.xLeft;
            return result + offset;
        }

        /// <summary>
        /// Берёт ячейку напрямую
        /// </summary>
        /// <param name="rowIDX">Строка</param>
        /// <param name="columnIDX">Столбец</param>
        /// <param name="indentedX">С учётом отступа (ставь false, если используется EditorGUI)</param>
        /// <returns>Прямоугольник ячейки</returns>
        protected Rect GetDirectGridCell(int rowIDX, int columnIDX, bool indentedX = true)
        {
            float x = GetDirectColumnX(columnIDX);
            float y = GetDirectRowY(rowIDX);
            float d = indentedX ? 0 : guiIndentX;
            return new Rect(x - d, y, columnWidth + d, elementHeight);
        }

        /// <summary>
        /// Берёт ячейку с уже известными значениями верхнего левого угла
        /// </summary>
        /// <param name="colX">X строки</param>
        /// <param name="rowY">Y столбца</param>
        /// <param name="indentedX">С учётом отступа (false, если используется EditorGUI)</param>
        /// <returns>Прямоугольник ячейки</returns>
        protected Rect GetGridCellFrom(float rowY, float colX, bool indentedX = true)
        {
            float d = indentedX ? 0 : guiIndentX;
            return new Rect(colX - d, rowY, columnWidth + d, elementHeight);
        }

        /// <summary>
        /// Взять прямоугольник ячейки по заранее вычисленным значениям
        /// </summary>
        /// <param name="columnIDX">Номер столбца</param>
        /// <param name="rowIDX">Номер строки</param>
        /// <param name="indentedX">С учётом отступа (false, если используется EditorGUI)</param>
        /// <returns>Прямоугольник ячейки</returns>
        protected Rect GetGridCell(int rowIDX, int columnIDX, bool indentedX = true)
        {
            float d = indentedX ? 0 : guiIndentX;
            float x = gridColumnsX[columnIDX] - d;
            float y = gridRowsY[rowIDX];
            float w = columnWidth + d;
            return new Rect(x, y, w, elementHeight);
        }

        /// <summary>
        /// Взять всю строку сетки
        /// </summary>
        /// <param name="rowIDX">Номер строки</param>
        /// <param name="indentedX">С учётом отступа (false, если используется EditorGUI)</param>
        /// <param name="columnLeftOffset">Сделать отступ слева на N столбцов</param>
        /// <returns></returns>
        protected Rect GetGridRow(int rowIDX, bool indentedX = true, int columnLeftOffset = 0)
        {
            float d = indentedX ? 0 : guiIndentX;
            float x = gridColumnsX[columnLeftOffset] - d;
            float y = gridRowsY[rowIDX];
            float w = innerSize.x - x;
            return new Rect(x, y, w, elementHeight);
        }

        /// <summary>
        /// Взять весь столбец сетки
        /// </summary>
        /// <param name="columnIDX">Номер столбца</param>
        /// <param name="indentedX">С учётом отступа (false, если используется EditorGUI)</param>
        /// <param name="rowUpperOffset">Сделать отступ сверху на N строк</param>
        /// <returns></returns>
        protected Rect GetGridColumn(int columnIDX, bool indentedX = true, int rowUpperOffset = 0)
        {
            float d = indentedX ? 0 : guiIndentX;
            float x = gridColumnsX[columnIDX] - d;
            float y = gridRowsY[rowUpperOffset];
            float w = innerSize.x - x;
            return new Rect(x, y, w, elementHeight);
        }

        // GEOMETRY INITIALIZATION ================================================================
        // Вычисляем размеры для стандартного свойства
        //=========================================================================================
        protected void InitializeSizesGeneral(Rect position)
        {
            baseRect = position;
            indentedRect  = EditorGUI.IndentedRect(position);
            indentedStart = new Vector2(indentedRect.x, indentedRect.y);
            indentedSize  = new Vector2(indentedRect.width, indentedRect.height);
            guiIndentX = indentedStart.x - position.x;

            float innerWidth  = indentedSize.x - innerStroke.bothX;
            float innerHeight = indentedSize.y - innerStroke.bothY;
            innerSize = new Vector2(innerWidth, innerHeight);

            elementHeight = GetElementHeight();
            outline = GetOutline();
            innerStroke = GetInnerStroke();
        }

        protected void InitializeGrid(bool indented = true, bool outlined = true) // вызывать только после SetSizeGeneral!
        {
            gridSpace = GetGridSpace();
            int rows = GridRowCount();
            int columns = GridColumnCount();
            columnWidth = (innerSize.x - (columns - 1) * gridSpace.x) / columns;

            gridColumnsX = new float[columns];
            gridRowsY = new float[rows];

            float startFromY = indented ? indentedStart.y : baseRect.y;
            startFromY += innerStroke.yTop;
            if (outlined) startFromY += outline.yTop;
            for (int row = 0; row < rows; row++)
            {
                gridRowsY[row] = startFromY + elementHeight * row + gridSpace.y * (row - 1);
            }

            float startFromX = indented ? indentedStart.x : baseRect.x;
            startFromX += innerStroke.xLeft;
            if (outlined) startFromX += outline.xLeft;
            for (int column = 0; column < columns; column++)
            {
                gridColumnsX[column] = startFromX + columnWidth * column + gridSpace.x * (column - 1);
            }
        }

        // DRAWING ================================================================================
        // Некоторые функции для быстрого рисования
        //=========================================================================================
        /// <summary>
        /// Рисует стандартную панельку с учётом бордюра-обводки
        /// </summary>
        protected void DrawBackgroundPanel()
        {
            Rect boxRect = new Rect(indentedStart.x - outline.xLeft,
                                    indentedStart.y + outline.yTop,
                                    indentedSize.x  + outline.xRight,
                                    indentedSize.y  - outline.yBottom);
            GUI.Box(boxRect, "", SpiralStyles.panel);
        }

        public static bool DrawFoldout(bool foldout, string content, Rect position, GUIStyle style)
        {
            if (style == null) style = SpiralStyles.foldoutPropertyNormal;
            return EditorGUI.Foldout(position, foldout, content, true, style);
        }

        public static bool DrawFoldout(bool foldout, GUIContent content, Rect position, GUIStyle style)
        {
            if (style == null) style = SpiralStyles.foldoutPropertyNormal;
            return EditorGUI.Foldout(position, foldout, content, true, style);
        }

        public static void DrawScriptFieldRect(Type type, Rect position)
        {
            GUI.enabled = false;
            MonoScript monoScript = SpiralEditorTools.GetMonoScript(type);
            EditorGUI.ObjectField(position, monoScript, typeof(MonoScript), false);
            GUI.enabled = true;
        }

        public static void DrawScriptFieldRect(MonoScript monoScript, Rect position)
        {
            GUI.enabled = false;
            EditorGUI.ObjectField(position, monoScript, typeof(MonoScript), false);
            GUI.enabled = true;
        }

        // CASHED MONO ============================================================================
        // Кешировать моно для конкретного проперти
        //=========================================================================================
        protected static MonoScript CahsedMono<T>(ref MonoScript monoScript) // да, здесь действительно нужен ref
        {
            if (monoScript == null)
            {
                monoScript = SpiralEditorTools.GetMonoScript(typeof(T));
            }
            return monoScript;
        }

        // SERIALIZATION WORKS ====================================================================
        // Извлечение дерева сериализации, если необходимо
        //=========================================================================================
        protected T GetTargetValidation<T>(SerializedProperty serializedProperty) where T : NonUnitySerializableClass
        {
            T target = serializedProperty.GetPropertyObject() as T;
            if (target != null) target.EditorCreated(false);
            return target;
        }

        protected T GetTarget<T>(SerializedProperty serializedProperty) where T : class
        {
            return serializedProperty.GetPropertyObject() as T;
        }

        protected List<object> hierarchy { get; private set; } = null;
        protected List<string> serializationPath { get; private set; } = null;
        protected void InitSeraizliationTree(SerializedProperty property)
        {
            serializationPath = property.GetPathNodes().Listed();
            serializationPath.Insert(0, property.GetRootParent().name);
            hierarchy = property.GetSerializationHierarchy(false);
        }

        protected string[] objectNames = null;
        protected void InitSerializationTreeNames()
        {
            objectNames = new string[hierarchy.Count];

            int d = 0;
            for (int i = 0; i < objectNames.Length; i++)
            {
                if (hierarchy[i] == null)
                {
                    objectNames[i] = "[null]";
                    continue;
                }

                string variableName = serializationPath[i + d];
                if (variableName == "Array")
                {
                    d++;
                    variableName = serializationPath[i + d];
                }
                string typeName = hierarchy[i].GetType().Name;
                variableName = variableName.FirstLetterCapitalization();

                objectNames[i] = $"{variableName} ({typeName})";
            }
        }
    }
}
#endif