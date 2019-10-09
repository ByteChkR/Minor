namespace Engine.Debug
{
    public enum DebugStage
    {
        Startup = 1,
        Init = 2,
        SceneInit = 4,
        General = 8,
        Update = 16,
        Physics = 32,
        CleanUp = 64,
        Render = 128
    }
}