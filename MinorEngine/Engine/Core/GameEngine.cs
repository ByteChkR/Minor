﻿using System;
using Common;
using Engine.Audio;
using Engine.Debug;
using Engine.Physics;
using Engine.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.Core
{
    public class GameEngine
    {
        private GameWindow Window;
        protected Renderer Renderer;
        public EngineSettings Settings { get; }
        public static GameEngine Instance { get; private set; }
        public AbstractScene CurrentScene { get; private set; }
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

            PhysicsEngine.Initialize();
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
            var proj = CurrentScene.Camera.Projection;
            var vector = UnProject(ref proj, CurrentScene.Camera.ViewMatrix, new Size(Width, Height), mouse);
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


                MemoryTracer.AddSubStage("Unload World");

                CurrentScene?._Destroy();

                MemoryTracer.NextStage("Removing World");

                CurrentScene?.RemoveDestroyedObjects();


                MemoryTracer.NextStage("Removing Old Scene");

                CurrentScene?.Destroy();

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


        private void OnResize(object o, EventArgs e)
        {
            GL.Viewport(0, 0, Window.Width, Window.Height);
        }


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