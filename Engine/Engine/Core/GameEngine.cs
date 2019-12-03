using System;
using System.Drawing;
using System.Reflection;
using Engine.Audio;
using Engine.Common;
using Engine.DataTypes;
using Engine.Debug;
using Engine.IO;
using Engine.Physics;
using Engine.Rendering;
using Engine.UI.EventSystems;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace Engine.Core
{
    /// <summary>
    /// Central class that is the "heart" of the Engine
    /// </summary>
    public class GameEngine
    {
        /// <summary>
        /// Private flag if the there is a scene change in progress
        /// </summary>
        private bool changeScene;

        /// <summary>
        /// The Next scene to be initialized
        /// </summary>
        private Type nextScene;

        /// <summary>
        /// An internal update frame counter
        /// </summary>
        private int frameCounter;

        /// <summary>
        /// Renderer Instance
        /// </summary>
        protected Renderer Renderer;

        /// <summary>
        /// An internal render frame counter
        /// </summary>
        private int renderFrameCounter;

        /// <summary>
        /// The Window used to render
        /// </summary>
        private GameWindow window;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="settings">Settings to be used</param>
        public GameEngine(EngineSettings settings)
        {
            Instance = this;

            TextProcessorApi.PpCallback = new PPIOCallbacks();

            if (settings != null)
            {
                SetSettings(settings);
            }

            ManifestReader.RegisterAssembly(Assembly.GetExecutingAssembly());
        }
        /// <summary>
        /// Is true when the Game Window has the Focus from the OS(e.g is in the foreground)
        /// </summary>
        public bool HasFocus => window.Focused;

        /// <summary>
        /// The Window Position
        /// </summary>
        public Vector2 WindowPosition => new Vector2(window.Location.X, window.Location.Y);

        /// <summary>
        /// The Event System used by the Ui Systems
        /// </summary>
        public EventSystem UiSystem { get; private set; }

        /// <summary>
        /// The Settings the engine has been started with
        /// </summary>
        public EngineSettings Settings { get; private set; }

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
        public int Width => window.Width;

        /// <summary>
        /// Window Height
        /// </summary>
        public int Height => window.Height;

        /// <summary>
        /// The Window Size of the Game Window
        /// </summary>
        public Vector2 WindowSize => new Vector2(Width, Height);

        /// <summary>
        /// Returns the Window Info associated with the Game Window
        /// </summary>
        public IWindowInfo WindowInfo => window.WindowInfo;

        /// <summary>
        /// Property that returns the current AspectRatio
        /// </summary>
        public float AspectRatio => Width / (float) Height;

        /// <summary>
        /// Default Settings
        /// </summary>
        private DebugSettings EngineDefault => DebugSettings.GetDefault();

        /// <summary>
        /// Mouse Position in pixels
        /// </summary>
        public Vector2 MousePosition { get; private set; }
        
        /// <summary>
        /// Sets the Game Window Context as active for the Calling thread.
        /// </summary>
        public void MakeCurrent()
        {
            window.MakeCurrent();
        }

        /// <summary>
        /// Applies the Engine Settings (only working when window is not started yet. Undefined behaviour when done so
        /// </summary>
        /// <param name="settings">The settings to be applied</param>
        public void SetSettings(EngineSettings settings)
        {
            Settings = settings;

            DebugHelper.ApplySettings(settings?.DebugSettings ?? EngineDefault);
        }

        /// <summary>
        /// Initializes the Game Systems
        /// </summary>
        public void Initialize()
        {
            Logger.Log("Init started..", DebugChannel.Log | DebugChannel.EngineCore, 10);
            InitializeWindow();
            InitializeRenderer();

            AudioManager.Initialize();

            PhysicsEngine.Initialize();
            UiSystem = new EventSystem();
        }


        /// <summary>
        /// Initializes the OpenGL Window and registers some handles
        /// </summary>
        private void InitializeWindow()
        {
            Logger.Log("Initializing Window..", DebugChannel.Log | DebugChannel.EngineCore, 10);
            Logger.Log(
                $"Width: {Settings.InitWidth} Height: {Settings.InitHeight}, Title: {Settings.Title}, FSAA Samples: {Settings.Mode.Samples}, Physics Threads: {Settings.PhysicsThreadCount}",
                DebugChannel.Log | DebugChannel.EngineCore, 9);
            window = new GameWindow(Settings.InitWidth, Settings.InitHeight, Settings.Mode, Settings.Title,
                Settings.WindowFlags);

            #region WindowHandles

            Input.Initialize(window);
            window.UpdateFrame += Update;
            window.Resize += OnResize;
            window.KeyDown += GameObject._KeyDown;
            window.KeyUp += GameObject._KeyUp;
            window.KeyPress += GameObject._KeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;

            #endregion
        }

        /// <summary>
        /// Closes the Game Window
        /// </summary>
        public void Exit()
        {
            window.Close();
        }

        
        private void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            ManifestReader.ClearUnpackedFiles();
        }


        /// <summary>
        /// Initializes the renderer
        /// </summary>
        private void InitializeRenderer()
        {
            //TODO

            Logger.Log("Initializing Renderer..", DebugChannel.Log | DebugChannel.EngineCore, 10);
            Renderer = new Renderer();
            window.RenderFrame += OnRender;
            window.MouseMove += Window_MouseMove;
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
            InitializeScene(typeof(T));
        }

        /// <summary>
        /// Loads a new Scene
        /// </summary>
        /// <param name="sceneType">The Type of Scene</param>
        public void InitializeScene(Type sceneType)
        {
            if (!typeof(AbstractScene).IsAssignableFrom(sceneType))
            {
                return;
            }

            changeScene = true;
            nextScene = sceneType;
            Logger.Log("Initializing Scene..", DebugChannel.Log | DebugChannel.EngineCore, 9);
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
            Logger.Log("Running GameEngine Loop..", DebugChannel.Log | DebugChannel.EngineCore, 10);
            window.VSync = VSyncMode.Off;
            window.Run(0, 0);
        }

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
            frameCounter++;

            MemoryTracer.NextStage("Update Frame: " + frameCounter);


            UiSystem.Update();

            MemoryTracer.AddSubStage("Scene Update");
            CurrentScene?.Update((float) e.Time);

            MemoryTracer.NextStage("Physics Update");
            PhysicsEngine.Update((float) e.Time);

            EngineStatisticsManager.Update((float) e.Time);

            MemoryTracer.NextStage("ThreadManager Update");
            ThreadManager.CheckManagerStates();

            if (changeScene)
            {
                MemoryTracer.NextStage("Scene Intialization");
                changeScene = false;


                MemoryTracer.AddSubStage("Removing Old Scene");

                CurrentScene?._Destroy();

                CurrentScene?.DestroyScene(); //Call on destroy on the scene itself.

                MemoryTracer.NextStage("Removing World");

                CurrentScene?.RemoveDestroyedObjects();


                MemoryTracer.NextStage("Create New Scene");

                CurrentScene = (AbstractScene) Activator.CreateInstance(nextScene);

                MemoryTracer.NextStage("Initialize New Scene");

                CurrentScene._initializeScene();

                MemoryTracer.ReturnFromSubStage();
            }


            //Cleanup
            MemoryTracer.NextStage("Clean up Destroyed Objects");
            CurrentScene?.RemoveDestroyedObjects();
            MemoryTracer.ReturnFromSubStage(); //Returning to root.
        }

        /// <summary>
        /// Event handler that changes the viewport of the Engine when it gets resitzed
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void OnResize(object o, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
        }

        /// <summary>
        /// Gets called by opentk when it is time for a render update
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void OnRender(object o, FrameEventArgs e)
        {
            renderFrameCounter++;

            MemoryTracer.NextStage("Render Frame: " + renderFrameCounter);

            MemoryTracer.AddSubStage("Rendering Render Targets");

            Renderer.RenderAllTargets(CurrentScene);

            MemoryTracer.NextStage("Swapping Window Buffers");

            window.SwapBuffers();

            EngineStatisticsManager.Render((float) e.Time);

            MemoryTracer.ReturnFromSubStage();
        }
    }
}