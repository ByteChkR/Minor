using OpenTK;

namespace Engine.UI.EventSystems
{
    /// <summary>
    /// ISelectable Interface for UI Elements
    /// </summary>
    public interface ISelectable
    {
        int TabStop { get; set; }
        Box2 BoundingBox { get; }
        void SetState(SelectableState state);
    }
}