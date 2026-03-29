using System;

namespace MultiplayerARPG.Server.Scheduling
{
    /// <summary>
    /// Scheduling delayed server tasks.
    /// Gameplay systems use this instead of talking to the scheduler directly.
    /// </summary>
    public static class ServerDelayedTasks
    {
        private static ServerDelayedTaskSystem system;

        // Binds the delayed task system.
        internal static void Bind(ServerDelayedTaskSystem delayedTaskSystem)
        {
            system = delayedTaskSystem;
        }

        internal static void Unbind()
        {
            system = null;
        }

        // Schedule a one-shot callback to run after a delay (in seconds).
        public static IScheduledTaskHandle Schedule(
            double delaySeconds,
            Action callback)
        {
            if (system == null)
                throw new InvalidOperationException("ServerDelayedTaskSystem is not bound. " + "Ensure it is registered and bound during server startup.");

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return system.Schedule(delaySeconds, callback);
        }
    }
}
