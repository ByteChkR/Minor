namespace MinorEngine.debug
{
    public enum DebugStage
    {
        Startup = 1,
        Initialization = 2,
        SceneInitialization = 4,
        GeneralStage = 8,
        WorldUpdate = 16,
        PhysicsUpdate = 32,
        CleanUpPhase = 64,
        OnRenderStage = 128,


    }
}