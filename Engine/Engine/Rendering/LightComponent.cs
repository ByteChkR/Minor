using System.Drawing;
using Engine.Core;
using OpenTK;

namespace Engine.Rendering
{

    /// <summary>
    /// Component used to represent a point light in the current scene
    /// </summary>
    public class LightComponent : AbstractComponent
    {
        /// <summary>
        /// Flag to switch between Point Light and Directional Light
        /// true(Default): Point Light
        /// false: Directional Light(Directional Light Vector = Position)
        /// </summary>
        public bool IsPoint { get; set; } = true;
        public Color LightColor { get; set; } = Color.White;
        /// <summary>
        /// Attenuation used by the shader
        /// x: Base Value
        /// y: Squared Attenuation
        /// z: Cubed Attenuation
        /// </summary>
        public Vector3 Attenuation { get; set; } = new Vector3(1, 0, 0);
        /// <summary>
        /// Overall Ambient Contribution
        /// </summary>
        public float AmbientContribution { get; set; } = 0.15f;
        /// <summary>
        /// The Intensity of the Light Source
        /// </summary>
        public float Intensity { get; set; } = 1;

        /// <summary>
        /// Adds the Light Component to the Renderer
        /// </summary>
        protected override void Awake()
        {
            Renderer.Lights.Add(this);
        }

        /// <summary>
        /// Removes the Light Component From the Renderer
        /// </summary>
        protected override void OnDestroy()
        {
            Renderer.Lights.Remove(this);
        }
    }
}