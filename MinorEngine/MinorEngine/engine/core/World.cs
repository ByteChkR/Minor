using System;
using Common;
using OpenTK;

namespace GameEngine.engine.core
{
    public class World : GameObject
    {
        public Camera Camera { get; private set; }
        

        public Matrix4 ViewMatrix
        {
            get
            {
                if (Camera == null)
                {
                    
                    return Matrix4.Identity;
                }
                return Matrix4.Invert(Camera.GetWorldTransform());
            }
        }

        public World() : base(Vector3.Zero, "World", null)
        {
            World = this;
        }

        public void SetCamera(Camera c)
        {
            if (c != null)
            {
                Camera = c;
            }
        }
    }
}