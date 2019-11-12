namespace Engine.Debug
{
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
        public static float OpenGLObjectsKB { get; private set; }
        public static int OpenGLObjectCount { get; private set; }
        public static float OpenCLObjectsKB { get; private set; }
        public static int OpenCLObjectCount { get; private set; }

        internal static void Update(float deltaTime)
        {
            if (deltaTime > BiggestUpdateTime) BiggestUpdateTime = deltaTime;
            else if (deltaTime < SmallestUpdateTime) SmallestUpdateTime = deltaTime;
            TimeSinceStartup += deltaTime;
            TotalUpdates++;
        }

        internal static void Render(float renderTime)
        {
            if (renderTime > BiggestRenderTime) BiggestRenderTime = renderTime;
            else if (renderTime < SmallestRenderTime) SmallestRenderTime = renderTime;
            TotalFrames++;
            TotalRenderTime += renderTime;
        }

        internal static void GLObjectCreated(long bytes)
        {
            OpenGLObjectCount++;
            OpenGLObjectsKB += bytes / (float)1028;
        }

        internal static void GLObjectDestroyed(long bytes)
        {
            OpenGLObjectCount--;
            OpenGLObjectsKB -= bytes / (float)1028;
        }
        internal static void CLObjectCreated(long bytes)
        {
            OpenCLObjectCount++;
            OpenCLObjectsKB += bytes / (float)1028;
        }

        internal static void CLObjectDestroyed(long bytes)
        {
            OpenCLObjectCount--;
            OpenCLObjectsKB -= bytes / (float)1028;
        }
    }
}