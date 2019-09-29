using System;
using Common;
using GameEngine.engine.audio;
using GameEngine.engine.physics;
using GameEngine.engine.ui;
using GameEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.engine.core
{
    public class EngineSettings
    {
        public GraphicsMode Mode { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int PhysicsThreadCount { get; set; } = 1;
        public string Title { get; set; }
        public GameWindowFlags WindowFlags { get; set; }
    }

    public class AbstractGame
    {
        public World World { get; set; }
        protected Renderer Renderer;
        protected GameWindow Window;
        public EngineSettings Settings { get; private set; }
        public static AbstractGame Instance { get; private set; }
        public AbstractGame(EngineSettings settings)
        {
            Instance = this;
            this.Settings = settings;
        }

        public void Initialize()
        {
            this.Log("Initialization started..", DebugChannel.Log);
            initializeWindow();
            initializeRenderer();

            AudioManager.Initialize();

            Physics.Init();

            initializeWorld();
            initializeScene();

        }

        private void initializeWindow()
        {

            this.Log("Initializing Window..", DebugChannel.Log);
            this.Log(
                $"Width: {Settings.Width} Height: {Settings.Height}, Title: {Settings.Title}, FSAA Samples: {Settings.Mode.Samples}, Physics Threads: {Settings.PhysicsThreadCount}" , DebugChannel.Log);
            Window = new GameWindow(Settings.Width, Settings.Height, Settings.Mode, Settings.Title, Settings.WindowFlags);

            #region WindowHandles

            Window.UpdateFrame += Update;
            Window.Resize += OnResize;
            Window.KeyDown += GameObject._KeyDown;
            Window.KeyUp += GameObject._KeyUp;
            Window.KeyPress += GameObject._KeyPress;

            #endregion

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
            UIHelper.InitializeUI();
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
            
            Physics.Update((float)e.Time);

            World.Update((float)e.Time);



            //TODO: Decide on how to Update the components
        }


        private void OnResize(object o, System.EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }



        private void OnRender(object o, EventArgs e)
        {

            Renderer.RenderAllTargets(World);
            
            Window.SwapBuffers();

        }

    }
}