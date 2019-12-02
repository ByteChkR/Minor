using Engine.Core;
using OpenTK;

namespace Engine.Rendering
{
    /// <summary>
    /// Basic Camera implementation as GameObject
    /// </summary>
    public class BasicCamera : GameObject, ICamera
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="projection">The projection matrix of the Camera</param>
        /// <param name="localPosition">The position in local space</param>
        public BasicCamera(Matrix4 projection, Vector3 localPosition) : base(localPosition, "Camera")
        {
            Projection = projection;
        }

        /// <summary>
        /// The projection matrix of the Camera
        /// </summary>
        public Matrix4 Projection { get; set; }

        /// <summary>
        /// The view matrix of the Camera
        /// </summary>
        public Matrix4 ViewMatrix => Matrix4.Invert(GetWorldTransform());
    }
}