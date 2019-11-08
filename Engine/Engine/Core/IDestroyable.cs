namespace Engine.Core
{
    /// <summary>
    /// Interface used by Objects in the engine that have to be removed from some kind of system
    /// </summary>
    public interface IDestroyable
    {
        /// <summary>
        /// The Destroy function that start the removal of the game systems this object is attached to
        /// </summary>
        void Destroy();
    }
}