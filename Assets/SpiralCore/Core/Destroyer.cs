using System.Collections.Generic;
using UnityEngine;

namespace Spiral.Core
{
    public static class Destroyer
    {
#if UNITY_EDITOR 
        public static void Undo(this Object obj, string undoComment)
        {
            if (obj == null) return;
            UnityEditor.Undo.RecordObject(obj, undoComment);
        }

        public static void UndoLast()
        {
            UnityEditor.Undo.PerformUndo();
        }
#endif

        public static void FastDestroy<T>(T obj) where T : MonoBehaviour
        {
            if (obj == null) return;
#if UNITY_EDITOR 
            Undo(obj, "Fast destroy");
            Undo(obj.gameObject, "Fast destroy");
#endif
            Object.DestroyImmediate(obj.gameObject);
        }

        public static void FastDestroyComponent(Component component, bool killObject = false)
        {
            if (component == null) return;

            if (killObject)
            {
#if UNITY_EDITOR 
                Undo(component.gameObject, "Fast destroy");
#endif
                Object.DestroyImmediate(component.gameObject);
            }
            else
            {
                Object.DestroyImmediate(component);
            }
        }

        public static void FastDestroy(GameObject go)
        {
            if (go == null) return;
#if UNITY_EDITOR 
            Undo(go, "Fast destroy");
#endif
            Object.DestroyImmediate(go.gameObject);
        }

        public static void DestroyListedObjects<T>(this List<T> list) where T : MonoBehaviour
        {
            int count = list.Count;
            if (count == 0) return;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    FastDestroy(list[i]);
                }
                catch
                {
                    Debug.Log("Object cannot be deleted");
                }
            }
            list.Clear();
        }
    }
}