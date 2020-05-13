using UnityEngine;

namespace Spiral.Core
{
    public static class ComponentTools
    {
        /// <summary>
        /// Получить компонент из дочерних
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="mono">Моно</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия</returns>
        public static T TakeInChildren<T>(this MonoBehaviour mono, ref T m) where T : Component
        {
            if (m == null) m = mono.gameObject.GetComponentInChildren<T>();
            return m;
        }

        /// <summary>
        /// Получить компонент из дочерних
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="mono">Компонент</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия</returns>
        public static T TakeInChildren<T>(this Component component, ref T m) where T : Component
        {
            if (m == null) m = component.gameObject.GetComponentInChildren<T>();
            return m;
        }

        /// <summary>
        /// Получить компонент из дочерних
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="mono">Объект</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия</returns>
        public static T TakeInChildren<T>(this GameObject gameObject, ref T m) where T : Component
        {
            if (m == null) m = gameObject.GetComponentInChildren<T>();
            return m;
        }

        public static T TakeInParent<T>(this Component component, ref T m) where T : Component
        {
            if (m == null) m = component.gameObject.GetComponentInParent<T>();
            return m;
        }

        public static T TakeInParent<T>(this MonoBehaviour mono, ref T m) where T : Component
        {
            if (m == null) m = mono.gameObject.GetComponentInParent<T>();
            return m;
        }

        public static T TakeInParent<T>(this GameObject gameObject, ref T m) where T : Component
        {
            if (m == null) m = gameObject.GetComponentInParent<T>();
            return m;
        }

        /// <summary>
        /// Взять компонент
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="mono">Моно</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия</returns>
        public static T Take<T>(this MonoBehaviour mono, ref T m) where T : Component
        {
            if (m == null) m = mono.gameObject.GetComponent<T>();
            return m;
        }

        /// <summary>
        /// Взять компонент
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="mono">Компонент</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия</returns>
        public static T Take<T>(this Component component, ref T m) where T : Component
        {
            if (m == null) m = component.gameObject.GetComponent<T>();
            return m;
        }

        /// <summary>
        /// Взять компонент
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="gameObject">Объект</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия</returns>
        public static T Take<T>(this GameObject gameObject, ref T m) where T : Component
        {
            if (m == null) m = gameObject.GetComponent<T>();
            return m;
        }

        /// <summary>
        /// Взять компонент, при отсутствии - добавить его
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="mono">Моно</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия; было ли произведено добавление</returns>
        public static (T component, bool added) AddTake<T>(this MonoBehaviour mono, ref T m) where T : Component
        {
            Take(mono, ref m);
            bool added = false;
            if (m == null)
            {
                m = mono.gameObject.AddComponent<T>();
                added = true;
            }
            return (m, added);
        }

        /// <summary>
        /// Взять компонент, при отсутствии - добавить его
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="mono">Объект</param>
        /// <param name="m">Приватная переменная типа T:Component, в которую идёт запись</param>
        /// <returns>Компонент, содержащийся в m после процедуры взятия; было ли произведено добавление</returns>
        public static (T component, bool added) AddTake<T>(this GameObject gameObject, ref T m) where T : Component
        {
            Take(gameObject, ref m);
            bool added = false;
            if (m == null)
            {
                m = gameObject.AddComponent<T>();
                added = true;
            }
            return (m, added);
        }
    }
}