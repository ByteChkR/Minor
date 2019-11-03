
using OpenTK;

namespace Engine.UI.EventSystems
{
    public interface ISelectable
    {
        int TabStop { get; set; }
        void SetState(SelectableState state);
        Box2 BoundingBox { get; set; }
    }

    
}