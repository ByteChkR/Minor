using System;
using Common;
using GameEngine.engine.rendering;
using OpenTK;

namespace GameEngine.engine.core
{
    public class World : GameObject
    {
        public Camera Camera { get; private set; }


        public World() : base(Vector3.Zero, "World", null)
        {
            this.World = this;
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