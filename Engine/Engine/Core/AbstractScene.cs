using System;
using Engine.Rendering;

namespace Engine.Core
{
    /// <summary>
    /// Represents an Abstract scene that can be loaded by the game engine
    /// </summary>
    public abstract class AbstractScene : GameObject
    {

        /// <summary>
        /// The Camera that is associated with the Game World
        /// </summary>
        public BasicCamera Camera { get; private set; }

        /// <summary>
        /// Sets the camera of the Scene
        /// </summary>
        /// <param name="c"></param>
        public void SetCamera(BasicCamera c)
        {
            if (c != null)
            {
                Camera = c;
            }
        }


        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="sceneName">The Name of the Scene</param>
        protected AbstractScene(string sceneName = "Scene") : base(sceneName)
        {
            Scene = this;
        }

        /// <summary>
        /// Internal function to initialize the scene
        /// </summary>
        internal void _initializeScene()
        {
            InitializeScene();
        }

        /// <summary>
        /// Removes the Scene from the Game Systems
        /// </summary>
        public void DestroyScene()
        {
            OnDestroy();
        }

        /// <summary>
        /// Abstract function used to initialize scene specific things
        /// </summary>
        protected abstract void InitializeScene();

        /// <summary>
        /// On Destroy function in case something needs disposal at scene unload
        /// </summary>
        public virtual void OnDestroy()
        {
        }
    }
}