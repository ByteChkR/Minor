using System;
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
        private int FrameCounter;
        private int RenderFrameCounter;

        private DebugSettings EngineDefault => DebugSettings.Default;

        public GameEngine(EngineSettings settings)
        {
            Logger.SetDebugStage(DebugStage.Startup);

            Instance = this;
            Settings = settings;

            DebugHelper.ApplySettings(settings.DebugSettings ?? EngineDefault);
        }

        public void Initialize()
        {
            Logger.SetDebugStage(DebugStage.Init);

            Logger.Log("Init started..", DebugChannel.Log);
            initializeWindow();
            initializeRenderer();

            AudioManager.Initialize();

            Physics.Initialize();
        }

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


        private void initializeRenderer()
        {
            //TODO

            Logger.Log("Initializing Renderer..", DebugChannel.Log);
            Renderer = new Renderer();
            Window.RenderFrame += OnRender;
            Window.MouseMove += Window_MouseMove;
        }

        private void Window_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            MousePosition = new Vector2(e.X, e.Y);
        }

        public void InitializeScene<T>() where T : AbstractScene
        {
            _changeScene = true;
            _nextScene = typeof(T);
            Logger.Log("Initializing Scene..", DebugChannel.Log);
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
            Logger.SetDebugStage(DebugStage.General);
            Logger.Log("Running GameEngine Loop..", DebugChannel.Log);
            Window.VSync = VSyncMode.Off;
            Window.Run(0, 0);
        }

        public Vector2 MousePosition { get; private set; }

        public Vector3 ConvertScreenToWorldCoords(int x, int y)
        {
            Vector2 mouse;
            mouse.X = x;
            mouse.Y = y;
            var proj = World.Camera.Projection;
            var vector = UnProject(ref proj, World.Camera.ViewMatrix, new Size(Width, Height), mouse);
            var coords = new Vector3(vector);
            return coords;
        }

        private static Vector4 UnProject(ref Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / viewport.Width - 1;
            vec.Y = -2.0f * mouse.Y / viewport.Height + 1;
            vec.Z = 0;
            vec.W = 1.0f;

            var viewInv = Matrix4.Invert(view);
            var projInv = Matrix4.Invert(projection);

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

        protected virtual void Update(object sender, FrameEventArgs e)
        {
            FrameCounter++;
            Logger.SetDebugStage(DebugStage.Update);

            MemoryTracer.NextStep("Update Frame: " + FrameCounter);

            MemoryTracer.AddSubStep("Scene Update");
            currentScene?.Update((float) e.Time);
            MemoryTracer.NextStep("World Update");
            World?.Update((float) e.Time);

            Logger.SetDebugStage(DebugStage.Physics);
            MemoryTracer.NextStep("Physics Update");
            Physics.Update((float) e.Time);

            if (_changeScene)
            {
                Logger.SetDebugStage(DebugStage.SceneInit);
                MemoryTracer.NextStep("Scene Intialization");
                _changeScene = false;


                MemoryTracer.AddSubStep("Unload World");

                World?.Unload();

                MemoryTracer.NextStep("Removing World");

                World?.RemoveDestroyedObjects();


                MemoryTracer.NextStep("Removing Old Scene");

                currentScene?.Destroy();

                MemoryTracer.NextStep("Create New Scene");

                currentScene = (AbstractScene) Activator.CreateInstance(_nextScene);
                World = new World();

                MemoryTracer.NextStep("Initialize New Scene");

                currentScene._initializeScene(World);

                MemoryTracer.ReturnFromSubStep();
            }


            Logger.SetDebugStage(DebugStage.CleanUp);
            //Cleanup
            MemoryTracer.NextStep("Clean up Destroyed Objects");
            World?.RemoveDestroyedObjects();
            MemoryTracer.ReturnFromSubStep(); //Returning to root.
            //ResourceManager.ProcessDeleteQueue();
        }


        private void OnResize(object o, EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }


        private void OnRender(object o, EventArgs e)
        {
            RenderFrameCounter++;
            Logger.SetDebugStage(DebugStage.Render);


            MemoryTracer.NextStep("Render Frame: " + RenderFrameCounter);

            MemoryTracer.AddSubStep("Rendering Render Targets");

            Renderer.RenderAllTargets(World);

            MemoryTracer.NextStep("Swapping Window Buffers");

            Window.SwapBuffers();

            MemoryTracer.ReturnFromSubStep();
        }
    }
}