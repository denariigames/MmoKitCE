//updated
using System;

namespace MultiplayerARPG.Server.Scheduling
{
    /// <summary>
    /// Static facade for one-shot server callbacks. Bind it to the active ServerDelayedTaskSystem at server startup.
    /// </summary>
    public static class ServerDelayedTasks
    {
        private static ServerDelayedTaskSystem system;

        public static bool IsBound { get { return system != null; } }

        internal static void Bind(ServerDelayedTaskSystem delayedTaskSystem)
        {
            system = delayedTaskSystem;
        }

        internal static void Unbind()
        {
            system = null;
        }

        public static IScheduledTaskHandle Schedule(double delaySeconds, Action callback)
        {
            if (system == null)
                throw new InvalidOperationException("ServerDelayedTaskSystem is not bound. Ensure it is registered and bound during server startup.");
            return system.Schedule(delaySeconds, callback);
        }

        public static bool TrySchedule(double delaySeconds, Action callback, out IScheduledTaskHandle handle)
        {
            if (system == null || callback == null)
            {
                handle = null;
                return false;
            }
            handle = system.Schedule(delaySeconds, callback);
            return true;
        }

        public static void Clear()
        {
            if (system != null)
                system.Clear();
        }

        public static DelayedTaskMetrics CaptureMetrics()
        {
            if (system == null)
                return null;
            return system.Metrics;
        }
    }
}
