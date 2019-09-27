using BepuPhysics;

namespace GameEngine.engine.physics
{
    public interface IColliderComponent
    {
        BodyReference BodyReference { get; set; }
    }
}