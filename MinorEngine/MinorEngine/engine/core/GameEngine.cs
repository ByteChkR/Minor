﻿using System;
using System.Reflection;
using Common;
using MinorEngine.debug;
using MinorEngine.engine.audio;
using MinorEngine.engine.physics;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MinorEngine.engine.core
{
    public class GameEngine
    {
        private GameWindow Window;
        protected Renderer Renderer;
        public EngineSettings Settings { get; }
        public static GameEngine Instance { get; private set; }
        private AbstractScene currentScene;
        public World World { get; private set; }
        public int Width => Window.Width;
        public int Height => Window.Height;
        private bool _changeScene;
        private Type _nextScene;

        private DebugSettings EngineDefault => new DebugSettings
        {
            Enabled = true,
            InternalErrorMask = (int)DebugChannel.Internal_Error,
            InternalLogMask = (int)DebugChannel.Log,
            InternalWarningMask = (int)DebugChannel.Warning,
            SendInternalWarnings = true,
            MaskPrefixes = new []{ "[ERROR]" , "[WARNING]" , "[LOG]" , "[INTERNAL_ERROR]" , "[PROGRESS]" },
            WildcardPrefix = "[ALL]",
            NonePrefix = "[NONE]"
        };

        public GameEngine(EngineSettings settings)
        {
            Instance = this;
            Settings = settings;

            DebugHelper.Initialize(EngineDefault);

            if (Settings.DebugNetwork && Settings.ProgramID != -1)
            {
                if (Settings.ProgramVersion == null)
                    Settings.ProgramVersion = Assembly.GetExecutingAssembly().GetName().Version;
                DebugHelper.SetDebugLoggingInformation(Settings.ProgramID, Settings.NetworkMask,
                    Settings.ProgramVersion);
            }
        }

        public void Initialize()
        {
            this.Log("Initialization started..", DebugChannel.Log);
            initializeWindow();
            initializeRenderer();

            AudioManager.Initialize();

            Physics.Init();
        }

        private void initializeWindow()
        {
            this.Log("Initializing Window..", DebugChannel.Log);
            this.Log(
                $"Width: {Settings.InitWidth} Height: {Settings.InitHeight}, Title: {Settings.Title}, FSAA Samples: {Settings.Mode.Samples}, Physics Threads: {Settings.PhysicsThreadCount}",
                DebugChannel.Log);
            Window = new GameWindow(Settings.InitWidth, Settings.InitHeight, Settings.Mode, Settings.Title,
                Settings.WindowFlags);

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


        public void InitializeScene<T>() where T : AbstractScene
        {
            _changeScene = true;
            _nextScene = typeof(T);
            this.Log("Initializing Scene..", DebugChannel.Log);
        }


        public void AddRenderTarget(RenderTarget target)
        {
            Renderer.AddRenderTarget(target);
        }

        public void RemoveRenderTarget(RenderTarget target)
        {
            Renderer.RemoveRenderTarget(target);
        }


        public void Run()
        {
            this.Log("Running GameEngine Loop..", DebugChannel.Log);
            Window.VSync = VSyncMode.Off;
            Window.Run(0, 0);
        }

        protected virtual void Update(object sender, FrameEventArgs e)
        {
            currentScene?.Update((float) e.Time);
            World?.Update((float) e.Time);
            Physics.Update((float) e.Time);

            if (_changeScene)
            {
                _changeScene = false;

                World?.Destroy();
                currentScene?.Destroy();
                currentScene = (AbstractScene) Activator.CreateInstance(_nextScene);
                World = new World();
                currentScene._initializeScene(World);
            }
        }


        private void OnResize(object o, EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }


        private void OnRender(object o, EventArgs e)
        {
            Renderer.RenderAllTargets(World);

            Window.SwapBuffers();
        }
    }


    //public class AbstractGame
    //{
    //    public World World { get; set; }
    //    protected Renderer Renderer;
    //    protected GameWindow Window;
    //    public EngineSettings Settings { get; private set; }
    //    public static AbstractGame Instance { get; private set; }
    //    public int Width => Window.Width;
    //    public int Height => Window.Height;
    //    public AbstractGame(EngineSettings settings)
    //    {
    //        Instance = this;

    //        this.Settings = settings;
    //    }

    //    ~AbstractGame()
    //    {

    //    }


    //    public void Initialize()
    //    {
    //        this.Log("Initialization started..", DebugChannel.Log);
    //        initializeWindow();
    //        initializeRenderer();

    //        AudioManager.Initialize();

    //        Physics.Init();

    //        initializeWorld();
    //        initializeScene();

    //    }

    //    private void initializeWindow()
    //    {

    //        this.Log("Initializing Window..", DebugChannel.Log);
    //        this.Log(
    //            $"Width: {Settings.InitWidth} Height: {Settings.InitHeight}, Title: {Settings.Title}, FSAA Samples: {Settings.Mode.Samples}, Physics Threads: {Settings.PhysicsThreadCount}", DebugChannel.Log);
    //        Window = new GameWindow(Settings.InitWidth, Settings.InitHeight, Settings.Mode, Settings.Title, Settings.WindowFlags);

    //        #region WindowHandles

    //        Window.UpdateFrame += Update;
    //        Window.Resize += OnResize;
    //        Window.KeyDown += GameObject._KeyDown;
    //        Window.KeyUp += GameObject._KeyUp;
    //        Window.KeyPress += GameObject._KeyPress;

    //        #endregion

    //    }


    //    private void initializeRenderer()
    //    {
    //        //TODO

    //        this.Log("Initializing Renderer..", DebugChannel.Log);
    //        Renderer = new Renderer();
    //        Window.RenderFrame += OnRender;
    //    }
    //    private void initializeWorld()
    //    {

    //        this.Log("Initializing World..", DebugChannel.Log);
    //        World = new World();
    //        UIHelper.InitializeUI();
    //    }

    //    protected virtual void initializeScene()
    //    {

    //        this.Log("Initializing Scene..", DebugChannel.Log);
    //    }


    //    public void Run()
    //    {
    //        this.Log("Running GameEngine Loop..", DebugChannel.Log);
    //        Window.VSync = VSyncMode.Off;
    //        Window.Run(0, 0);
    //    }

    //    protected virtual void Update(object sender, FrameEventArgs e)
    //    {

    //        Physics.Update((float)e.Time);

    //        World.Update((float)e.Time);


    //        //TODO: Decide on how to Update the components
    //    }


    //    private void OnResize(object o, System.EventArgs e)
    //    {
    //        GL.Viewport(0, 0, Window.Width, Window.Height);
    //    }


    //    private void OnRender(object o, EventArgs e)
    //    {

    //        Renderer.RenderAllTargets(World);

    //        Window.SwapBuffers();

    //    }

    //}
}