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
        private bool last;
        private float slow = 0.075f;


        private float time;
        public bool IsPoint { get; set; } = true;
        public Color LightColor { get; set; } = Color.White;
        public Vector3 Attenuation { get; set; } = new Vector3(1, 0, 0);
        public float AmbientContribution { get; set; } = 0.15f;
        public float Intensity { get; set; } = 1;

        protected override void Awake()
        {
            Renderer.Lights.Add(this);
        }

        protected override void OnDestroy()
        {
            Renderer.Lights.Remove(this);
        }

        protected override void Update(float deltaTime)
        {
            float dt = deltaTime * slow;
            if (time <= 0)
            {
                last = false;
                time += dt;
            }
            else if (time >= 1)
            {
                last = true;
                time -= dt;
            }
            else
            {
                time += last ? -dt : dt;
            }
        }
    }
}