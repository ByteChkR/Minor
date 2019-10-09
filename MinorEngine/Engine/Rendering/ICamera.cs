using OpenTK;

namespace Engine.Rendering
{
    public interface ICamera
    {
        Matrix4 Projection { get; }
        Matrix4 GetWorldTransform();
        Matrix4 ViewMatrix { get; }

    }
}