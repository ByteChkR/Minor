namespace Engine.Rendering
{
    /// <summary>
    /// Different merge types for Render Targets
    /// </summary>
    public enum RenderTargetMergeType
    {
        /// <summary>
        /// Will not be merged(can be used for rendering something "outside" of the engine rendering system e.g. custom renderers for special usecases)
        /// </summary>
        None,

        /// <summary>
        /// If selected it will be added on top of the drawn image
        /// </summary>
        Additive,

        /// <summary>
        /// If Selected it will multiply the drawn image pixels with the one in this render target
        /// </summary>
        Multiplicative
    }
}