using System;
using System.Drawing;
using Engine.Audio;
using Engine.Core;
using Engine.Debug;
using OpenTK;

namespace Engine.Rendering
{
    public class LightComponent : AbstractComponent
    {
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


        private float time;
        private bool last;
        private float slow = 0.075f;

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

            LightColor = Color.FromArgb((int) (255 * time), (int) (255 * time), 255, 255);
        }
    }
}