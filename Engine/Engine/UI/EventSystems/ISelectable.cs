using OpenTK;

namespace Engine.UI.EventSystems
{
    public interface ISelectable
    {
        int TabStop { get; set; }
        Box2 BoundingBox { get; }
        void SetState(SelectableState state);
    }
}