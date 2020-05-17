using System;
using UnityEngine;

namespace Spiral.EditorToolkit
{
    /// <summary>
    /// Для тех кастомных классов, которые могут быть сериализованы и нуждаться в валидации
    /// </summary>
    public abstract class NonUnitySerializableClass
    {
        /// <summary>
        /// Необходимо эту функцию, если дочерний объект может быть сериализован в составе массива
        /// Причина: при создании нового элемента массива ВСЕ его значения ставятся на дефолтные,
        /// даже те, которые определены в конструкторе класса. Нет, писать их значение по умолчанию
        /// не поможет. Нет, переопределение переменных в составе класса не поможет.
        /// Выход - кидать Validation, иначе всё накроется медным тазом.
        /// </summary>
        protected virtual void DefaultEditorObject()
        {
            // virtually do nothing
        }

        [SerializeField]private bool validated = false;
        public void EditorCreated(bool force = false)
        {
            if (validated && !force) return;
            DefaultEditorObject();
            validated = true;
        }

        public NonUnitySerializableClass()
        {
            EditorCreated(true);
            // прокидываем, чтобы для новосозданных нормальным путём объектов
            // не происходила некая дичь
        }
    }
}
