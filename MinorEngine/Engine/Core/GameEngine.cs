using System;
using Common;
using Engine.Audio;
using Engine.Debug;
using Engine.Physics;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Core
{
    /// <summary>
    /// Central class that is the "heart" of the Engine
    /// </summary>
    public class GameEngine
    {
        /// <summary>
        /// The Window used to render
        /// </summary>
        private GameWindow Window;

        /// <summary>
        /// Renderer Instance
        /// </summary>
        protected Renderer Renderer;

        /// <summary>
        /// The Settings the engine has been started with
        /// </summary>
        public EngineSettings Settings { get; }

        /// <summary>
        /// Static Singleton Instance
        /// </summary>
        public static GameEngine Instance { get; private set; }

        /// <summary>
        /// The current scene
        /// </summary>
        public AbstractScene CurrentScene { get; private set; }

        /// <summary>
        /// Window Width
        /// </summary>
        public int Width => Window.Width;

        /// <summary>
        /// Window Height
        /// </summary>
        public int Height => Window.Height;

        /// <summary>
        /// Private flag if the there is a scene change in progress
        /// </summary>
        private bool _changeScene;

        /// <summary>
        /// The Next scene to be initialized
        /// </summary>
        private Type _nextScene;

        /// <summary>
        /// An internal update frame counter
        /// </summary>
        private int FrameCounter;

        /// <summary>
        /// An internal render frame counter
        /// </summary>
        private int RenderFrameCounter;

        /// <summary>
        /// Default Settings
        /// </summary>
        private DebugSettings EngineDefault => DebugSettings.Default;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="settings">Settings to be used</param>
        public GameEngine(EngineSettings settings)
        {
            Logger.SetDebugStage(DebugStage.Startup);

            Instance = this;
            Settings = settings;

            DebugHelper.ApplySettings(settings?.DebugSettings ?? EngineDefault);
        }

        /// <summary>
        /// Initializes the Game Systems
        /// </summary>
        public void Initialize()
        {
            Logger.SetDebugStage(DebugStage.Init);

            Logger.Log("Init started..", DebugChannel.Log);
            initializeWindow();
            initializeRenderer();

            AudioManager.Initialize();

            PhysicsEngine.Initialize();
        }


        /// <summary>
        /// Initializes the OpenGL Window and registers some handles
        /// </summary>
        private void initializeWindow()
        {
            Logger.Log("Initializing Window..", DebugChannel.Log);
            Logger.Log(
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


        /// <summary>
        /// Initializes the renderer
        /// </summary>
        private void initializeRenderer()
        {
            //TODO

            Logger.Log("Initializing Renderer..", DebugChannel.Log);
            Renderer = new Renderer();
            Window.RenderFrame += OnRender;
            Window.MouseMove += Window_MouseMove;
        }

        /// <summary>
        /// Wrapper to update the mouse position property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            MousePosition = new Vector2(e.X, e.Y);
        }

        /// <summary>
        /// Function used to Load a new scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void InitializeScene<T>() where T : AbstractScene
        {
            _changeScene = true;
            _nextScene = typeof(T);
            Logger.Log("Initializing Scene..", DebugChannel.Log);
        }

        /// <summary>
        /// Adds a Render Target to the Renderer
        /// </summary>
        /// <param name="target">Target to add</param>
        public void AddRenderTarget(RenderTarget target)
        {
            Renderer.AddRenderTarget(target);
        }

        /// <summary>
        /// Removes a Render Target from the Renderer
        /// </summary>
        /// <param name="target">Target to remove</param>
        public void RemoveRenderTarget(RenderTarget target)
        {
            Renderer.RemoveRenderTarget(target);
        }

        /// <summary>
        /// Runs the Engine
        /// </summary>
        public void Run()
        {
            Logger.SetDebugStage(DebugStage.General);
            Logger.Log("Running GameEngine Loop..", DebugChannel.Log);
            Window.VSync = VSyncMode.Off;
            Window.Run(0, 0);
        }

        /// <summary>
        /// Mouse Position in pixels
        /// </summary>
        public Vector2 MousePosition { get; private set; }

        /// <summary>
        /// Converts the Current Screen position to world space coordinates
        /// </summary>
        /// <param name="x">x in pixels</param>
        /// <param name="y">y in pixels</param>
        /// <returns></returns>
        public Vector3 ConvertScreenToWorldCoords(int x, int y)
        {
            Vector2 mouse;
            mouse.X = x;
            mouse.Y = y;
            Matrix4 proj = CurrentScene.Camera.Projection;
            Vector4 vector = UnProject(ref proj, CurrentScene.Camera.ViewMatrix, new Size(Width, Height), mouse);
            Vector3 coords = new Vector3(vector);
            return coords;
        }

        /// <summary>
        /// Unprojects a 2D vector with the specified Projection matrix
        /// </summary>
        /// <param name="projection">Projection to be unprojected</param>
        /// <param name="view">View</param>
        /// <param name="viewport">Viewport(Width/Height of the screen)</param>
        /// <param name="mouse">The mouse position in pixels</param>
        /// <returns></returns>
        private static Vector4 UnProject(ref Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / viewport.Width - 1;
            vec.Y = -2.0f * mouse.Y / viewport.Height + 1;
            vec.Z = 0;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec;
        }

        /// <summary>
        /// Gets called from OpenTK whenever it is time for an update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Update(object sender, FrameEventArgs e)
        {
            FrameCounter++;
            Logger.SetDebugStage(DebugStage.Update);

            MemoryTracer.NextStage("Update Frame: " + FrameCounter);

            MemoryTracer.AddSubStage("Scene Update");
            CurrentScene?.Update((float) e.Time);
            MemoryTracer.NextStage("World Update");
            CurrentScene?.Update((float) e.Time);

            Logger.SetDebugStage(DebugStage.Physics);
            MemoryTracer.NextStage("Physics Update");
            PhysicsEngine.Update((float) e.Time);

            if (_changeScene)
            {
                Logger.SetDebugStage(DebugStage.SceneInit);
                MemoryTracer.NextStage("Scene Intialization");
                _changeScene = false;


                MemoryTracer.AddSubStage("Removing Old Scene");

                CurrentScene?.Destroy();

                CurrentScene?._Destroy();

                CurrentScene?.DestroyScene(); //Call on destroy on the scene itself.

                MemoryTracer.NextStage("Removing World");

                CurrentScene?.RemoveDestroyedObjects();


                MemoryTracer.NextStage("Create New Scene");

                CurrentScene = (AbstractScene) Activator.CreateInstance(_nextScene);

                MemoryTracer.NextStage("Initialize New Scene");

                CurrentScene._initializeScene();

                MemoryTracer.ReturnFromSubStage();
            }


            Logger.SetDebugStage(DebugStage.CleanUp);
            //Cleanup
            MemoryTracer.NextStage("Clean up Destroyed Objects");
            CurrentScene?.RemoveDestroyedObjects();
            MemoryTracer.ReturnFromSubStage(); //Returning to root.
            //ResourceManager.ProcessDeleteQueue();
        }

        /// <summary>
        /// Event handler that changes the viewport of the Engine when it gets resitzed
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void OnResize(object o, EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }

        /// <summary>
        /// Gets called by opentk when it is time for a render update
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void OnRender(object o, EventArgs e)
        {
            RenderFrameCounter++;
            Logger.SetDebugStage(DebugStage.Render);


            MemoryTracer.NextStage("Render Frame: " + RenderFrameCounter);

            MemoryTracer.AddSubStage("Rendering Render Targets");

            Renderer.RenderAllTargets(CurrentScene);

            MemoryTracer.NextStage("Swapping Window Buffers");

            Window.SwapBuffers();

            MemoryTracer.ReturnFromSubStage();
        }
    }
}