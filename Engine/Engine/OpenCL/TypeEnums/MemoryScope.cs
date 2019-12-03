namespace Engine.OpenCL.TypeEnums
{
    /// <summary>
    /// The Memory scope that kernel arguments can have
    /// </summary>
    public enum MemoryScope
    {
        None = 0,
        Global = 1,
        Constant = 2
    }
}