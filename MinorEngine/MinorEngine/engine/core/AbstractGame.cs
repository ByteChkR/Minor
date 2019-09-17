using System;
using Common;
using GameEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.core
{
    public struct EngineSettings
    {
        public GraphicsMode graphicsMode;
        public int width;
        public int height;
        public string title;
        public GameWindowFlags gameWindowFlags;

    }

    public class AbstractGame
    {
        public World world;
        protected Renderer renderer;
        protected GameWindow window;
        protected EngineSettings settings;

        public AbstractGame(EngineSettings settings)
        {
            this.settings = settings;


        }

        public virtual void Initialize()
        {
            this.Log("Initialization started..", DebugChannel.Log);
            initializeWindow();
            initializeRenderer();
            initializeWorld();
            initializeScene();
        }

        private void initializeWindow()
        {

            this.Log("Initializing Window..", DebugChannel.Log);
            this.Log(
                $"Width: {settings.width} Height: {settings.height}, Title: {settings.title}, FSAA Samples: {settings.graphicsMode.Samples}", DebugChannel.Log);
            window = new GameWindow(settings.width, settings.height, settings.graphicsMode, settings.title, settings.gameWindowFlags);
            window.UpdateFrame += Update;
            window.Resize += OnResize;
        }



        private void initializeRenderer()
        {
            //TODO

            this.Log("Initializing Renderer..", DebugChannel.Log);
            renderer = new Renderer();
            window.RenderFrame += OnRender;
        }
        private void initializeWorld()
        {

            this.Log("Initializing World..", DebugChannel.Log);
            world = new World();
        }

        protected virtual void initializeScene()
        {

            this.Log("Initializing Scene..", DebugChannel.Log);
        }

        private bool paused = false;



        public void Run()
        {
            this.Log("Running Game Loop..", DebugChannel.Log);
            window.VSync = VSyncMode.Off;
            window.Run(0, 0);
        }

        protected virtual void Update(object sender, FrameEventArgs e)
        {
            if (!paused)
            {
                world.Update((float)e.Time);
                time += (float)e.Time;

            }
            //TODO: Decide on how to Update the components
        }


        private void OnResize(object o, System.EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
        }

        private int frameCounter = 0;
        private float time = 0;

        private void OnRender(object o, EventArgs e)
        {
            frameCounter++;

            renderer.Render(world);

            if (time >= 1)
            {
                time = 0;
                frameCounter = 0;
            }

            window.SwapBuffers();

        }

    }
}