using System.Diagnostics;

namespace MultiplayerARPG.Server.Time
{
    // ServerClock
    // Authoritative time source for the server.
    // - Uses Stopwatch (monotonic, high precision)
    // - Independent of Unity Time, timeScale, and rendering FPS
    // - Safe for headless and non-Unity server loops
    // The scheduler calls Update() once per server frame.
    public static class ServerClock
    {
        
        private static bool initialized;

        static ServerClock()
        {
            // Initialize lastTicks so the first Update() does not generate a huge DeltaTime.
            lastTicks = stopwatch.ElapsedTicks;
            initialized = true;
        }
// High-precision monotonic timer
        private static readonly Stopwatch stopwatch = Stopwatch.StartNew();

        // Last recorded stopwatch ticks
        private static long lastTicks;

        // DeltaTime
        // Time (in seconds) since last Update() call.
        // This is NOT frame time — it is real elapsed time.
        public static double DeltaTime { get; private set; }

        // Total elapsed server time in seconds since startup.
        public static double Time { get; private set; }

        // Update
        // Must be called exactly once per server frame / loop iteration.
        // Advances the authoritative server clock.
        public static void Update()
        {
            long currentTicks = stopwatch.ElapsedTicks;

            // First update (or after Reset) should establish a baseline without producing a large dt.
            if (!initialized)
            {
                lastTicks = currentTicks;
                DeltaTime = 0;
                initialized = true;
                return;
            }

            long deltaTicks = currentTicks - lastTicks;
            lastTicks = currentTicks;

            // Convert ticks to seconds
            DeltaTime = (double)deltaTicks / Stopwatch.Frequency;
            Time += DeltaTime;
        }

        // Resets the clock.
        // Useful for server restarts or test harnesses.
        public static void Reset()
        {
            stopwatch.Restart();
            lastTicks = stopwatch.ElapsedTicks;
            initialized = true;
            DeltaTime = 0;
            Time = 0;
        }
    }
}
