using OpenTK;

namespace Engine.Rendering
{
    /// <summary>
    /// Simple Non-GameObject implementation of an ICamera
    /// </summary>
    public class ScreenCamera : ICamera
    {
        /// <summary>
        /// The Public constructor that is Creating an Othographic Projection by default
        /// </summary>
        public ScreenCamera()
        {
            Projection = Matrix4.CreateOrthographic(3,
                3, -10, 100);
        }

        /// <summary>
        /// The projection to be used
        /// </summary>
        public Matrix4 Projection { get; }

        /// <summary>
        /// The View matrix of the camera
        /// </summary>
        public Matrix4 ViewMatrix => Matrix4.Invert(GetWorldTransform());

        /// <summary>
        /// Function that returns the camera world transform(in this case always identity)
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetWorldTransform()
        {
            return Matrix4.Identity;
        }
    }
}