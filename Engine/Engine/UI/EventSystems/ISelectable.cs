using OpenTK;

namespace Engine.UI.EventSystems
{
    /// <summary>
    /// ISelectable Interface for UI Elements
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Bounding Box of the Selectable
        /// </summary>
        Box2 BoundingBox { get; }
        /// <summary>
        /// A Function to set the state of the object
        /// </summary>
        /// <param name="state">the state to be set</param>
        void SetState(SelectableState state);
    }
}