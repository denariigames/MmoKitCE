//updated
using System;
using System.Diagnostics;

namespace MultiplayerARPG.Server.Time
{
    /// <summary>
    /// Monotonic authoritative server clock.
    /// Uses Stopwatch instead of UnityEngine.Time so it is independent of rendering, timeScale, and client frame rate.
    /// </summary>
    public static class ServerClock
    {
        private static readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private static bool initialized;
        private static long lastTicks;

        /// <summary>
        /// Maximum delta applied to simulation time after a hitch/stall.
        /// RawDeltaTime still exposes the real elapsed wall-clock delta.
        /// </summary>
        public static double MaxDeltaTime = 0.25d;

        /// <summary>Clamped delta used by the scheduler.</summary>
        public static double DeltaTime { get; private set; }

        /// <summary>Unclamped real elapsed delta from Stopwatch.</summary>
        public static double RawDeltaTime { get; private set; }

        /// <summary>Authoritative simulation time in seconds.</summary>
        public static double Time { get; private set; }

        /// <summary>Total real monotonic time in seconds since the clock was reset.</summary>
        public static double RealTime
        {
            get { return stopwatch.Elapsed.TotalSeconds; }
        }

        static ServerClock()
        {
            Reset();
        }

        public static void Update()
        {
            long currentTicks = stopwatch.ElapsedTicks;

            if (!initialized)
            {
                lastTicks = currentTicks;
                RawDeltaTime = 0d;
                DeltaTime = 0d;
                initialized = true;
                return;
            }

            long deltaTicks = currentTicks - lastTicks;
            lastTicks = currentTicks;

            if (deltaTicks < 0)
                deltaTicks = 0;

            RawDeltaTime = (double)deltaTicks / Stopwatch.Frequency;

            double maxDelta = MaxDeltaTime;
            if (maxDelta <= 0d)
                maxDelta = RawDeltaTime;

            DeltaTime = RawDeltaTime > maxDelta ? maxDelta : RawDeltaTime;
            Time += DeltaTime;
        }

        public static void Reset()
        {
            stopwatch.Restart();
            lastTicks = stopwatch.ElapsedTicks;
            RawDeltaTime = 0d;
            DeltaTime = 0d;
            Time = 0d;
            initialized = true;
        }
    }
}
