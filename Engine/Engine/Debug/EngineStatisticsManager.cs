namespace Engine.Debug
{

    /// <summary>
    /// Class that keeps track of all possible statistics during the run of the engine
    /// </summary>
    public static class EngineStatisticsManager
    {
        public static int TotalUpdates { get; private set; }
        public static int TotalFrames { get; private set; }
        public static float BiggestUpdateTime { get; private set; }
        public static float SmallestUpdateTime { get; private set; }
        public static float BiggestRenderTime { get; private set; }
        public static float SmallestRenderTime { get; private set; }
        public static float TimeSinceStartup { get; private set; }
        public static float TotalRenderTime { get; private set; }
        public static float OpenGlObjectsKb { get; private set; }
        public static int OpenGlObjectCount { get; private set; }
        public static float OpenClObjectsKb { get; private set; }
        public static int OpenClObjectCount { get; private set; }

        internal static void Update(float deltaTime)
        {
            if (deltaTime > BiggestUpdateTime)
            {
                BiggestUpdateTime = deltaTime;
            }
            else if (deltaTime < SmallestUpdateTime)
            {
                SmallestUpdateTime = deltaTime;
            }

            TimeSinceStartup += deltaTime;
            TotalUpdates++;
        }

        internal static void Render(float renderTime)
        {
            if (renderTime > BiggestRenderTime)
            {
                BiggestRenderTime = renderTime;
            }
            else if (renderTime < SmallestRenderTime)
            {
                SmallestRenderTime = renderTime;
            }

            TotalFrames++;
            TotalRenderTime += renderTime;
        }

        internal static void GlObjectCreated(long bytes)
        {
            OpenGlObjectCount++;
            OpenGlObjectsKb += bytes / (float) 1028;
        }

        internal static void GlObjectDestroyed(long bytes)
        {
            OpenGlObjectCount--;
            OpenGlObjectsKb -= bytes / (float) 1028;
        }

        internal static void ClObjectCreated(long bytes)
        {
            OpenClObjectCount++;
            OpenClObjectsKb += bytes / (float) 1028;
        }

        internal static void ClObjectDestroyed(long bytes)
        {
            OpenClObjectCount--;
            OpenClObjectsKb -= bytes / (float) 1028;
        }
    }
}