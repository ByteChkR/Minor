using System;
using Common;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.core
{
    public class EngineSettings
    {
        public GraphicsMode Mode { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Title { get; set; }
        public GameWindowFlags WindowFlags { get; set; }
    }

    public class AbstractGame
    {
        public World World { get; set; }
        protected Renderer Renderer;
        protected GameWindow Window;
        protected EngineSettings Settings;
        

        public AbstractGame(EngineSettings settings)
        {
            this.Settings = settings;


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
                $"Width: {Settings.Width} Height: {Settings.Height}, Title: {Settings.Title}, FSAA Samples: {Settings.Mode.Samples}", DebugChannel.Log);
            Window = new GameWindow(Settings.Width, Settings.Height, Settings.Mode, Settings.Title, Settings.WindowFlags);
            Window.UpdateFrame += Update;
            Window.Resize += OnResize;
        }



        private void initializeRenderer()
        {
            //TODO

            this.Log("Initializing Renderer..", DebugChannel.Log);
            Renderer = new Renderer();
            Window.RenderFrame += OnRender;
        }
        private void initializeWorld()
        {

            this.Log("Initializing World..", DebugChannel.Log);
            World = new World();
        }

        protected virtual void initializeScene()
        {

            this.Log("Initializing Scene..", DebugChannel.Log);
        }




        public void Run()
        {
            this.Log("Running Game Loop..", DebugChannel.Log);
            Window.VSync = VSyncMode.Off;
            Window.Run(0, 0);
        }

        protected virtual void Update(object sender, FrameEventArgs e)
        {

            World.Update((float)e.Time);



            //TODO: Decide on how to Update the components
        }


        private void OnResize(object o, System.EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }



        private void OnRender(object o, EventArgs e)
        {

            Renderer.Render(World);

            

            Window.SwapBuffers();

        }

    }
}