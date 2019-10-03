using MinorEngine.engine.core;
using OpenTK;

namespace MinorEngine.engine.rendering
{
    public interface ICamera
    {
        Matrix4 Projection { get; }
        Matrix4 GetWorldTransform();
        Matrix4 ViewMatrix { get; }
    }
}