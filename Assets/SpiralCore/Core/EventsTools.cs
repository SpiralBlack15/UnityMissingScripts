using System;

namespace Spiral.Core
{
    public static class EventsTools
    {
        public static int CountOf(this MulticastDelegate multicast, Delegate action)
        {
            if (multicast == null) return 0;
            var delegates = multicast.GetInvocationList();
            int count = 0;
            foreach (var del in delegates)
            {
                if (del.Equals(action)) count++;
            }
            return count;
        }

        public static int Count(this MulticastDelegate multicast)
        {
            if (multicast == null) return 0;
            var delegates = multicast.GetInvocationList();
            return delegates.Length;
        }

        public static bool HasDelegate(this MulticastDelegate multicast, Delegate action)
        {
            if (multicast == null) return false;
            var delegates = multicast.GetInvocationList();
            foreach (var del in delegates) { if (del.Equals(action)) return true; }
            return false;
        }

        public static void KillInvokations(ref Action action)
        {
            if (action == null) return;
            var delegates = action.GetInvocationList();
            foreach (var del in delegates)
            {
                action -= del as Action;
            }
        }

        public static void KillInvokations<T>(ref Action<T> action) 
        {
            if (action == null) return;
            var delegates = action.GetInvocationList();
            foreach (var del in delegates)
            {
                action -= del as Action<T>;
            }
        }

        public static void KillInvokations<T1, T2>(ref Action<T1, T2> action)
        {
            if (action == null) return;
            var delegates = action.GetInvocationList();
            foreach (var del in delegates)
            {
                action -= del as Action<T1, T2>;
            }
        }
    }
}
