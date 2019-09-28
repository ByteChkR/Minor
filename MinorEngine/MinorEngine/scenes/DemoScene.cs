using System;
using System.Collections.Generic;
using GameEngine.engine.physics;
using GameEngine.engine.ui;
using GameEngine.components;
using GameEngine.engine.audio;
using GameEngine.engine.audio.sources;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.scenes
{
    public class DemoScene : AbstractGame
    {
        public DemoScene(EngineSettings settings) : base(settings)
        {

        }

        private static float ToRadians(float angle)
        {
            return MathF.PI * angle / 180;
        }

        protected override void Update(object sender, FrameEventArgs e)
        {
            Physics.Update((float)e.Time);
            base.Update(sender, e);
        }

        protected override void initializeScene()
        {
            base.initializeScene();


            Physics.Init();
            Physics.AddBoxStatic(System.Numerics.Vector3.Zero, new System.Numerics.Vector3(50, 10, 50), 1, 3);

            GameModel sphere = new GameModel("models/sphere_smooth.obj");
            GameModel plane = new GameModel("models/plane.obj");
            GameModel box = new GameModel("models/cube_flat.obj");
            GameModel bgBox = new GameModel("models/cube_flat.obj");

            GameTexture runic = GameTexture.Load("textures/runicfloor.png");
            GameTexture bg = GameTexture.Load("textures/ground4k.png");
            plane.Meshes[0].Textures = new[] { runic };
            sphere.Meshes[0].Textures = new[] { runic };
            box.Meshes[0].Textures = new[] { runic };
            bgBox.Meshes[0].Textures = new[] { bg };
            //car.Meshes[1].Textures = new[] { GameTexture.Load("textures/car.png") };

            Matrix4.CreatePerspectiveFieldOfView(ToRadians(60), 4f / 3f, 0.1f, 1000f, out Matrix4 projection);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"},
            }, out ShaderProgram shader);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UITextRender.fs"},
                {ShaderType.VertexShader, "shader/UITextRender.vs"},
            }, out ShaderProgram textShader);

            UITextRendererComponent uitElement = new UITextRendererComponent("Arial",  textShader)
            {
                Position = new Vector2(-0.5f, 0.43f),
                Scale = new Vector2(2f, 2f)
            };
                                                     

            GameObject uiContainer = new GameObject("UIContainer");
            uiContainer.AddComponent(uitElement);
            uiContainer.AddComponent(new TimeDisplay());
            Camera c = new Camera(projection, Vector3.Zero);
            c.AddComponent(new AudioListener());
            c.Rotate(Vector3.UnitX, ToRadians(-40));
            c.Translate(new Vector3(0, 60, 70));

            GameObject planeObj = new GameObject(Vector3.UnitZ * 2, "Plane");
            planeObj.Scale(new Vector3(5, 5, 5));
            planeObj.AddComponent(new MeshRendererComponent(shader, plane, 0));
            
            planeObj.AddComponent(new RotateAroundComponent());

            GameObject bgObj = new GameObject(Vector3.Zero, "BG");
            bgObj.Scale(new Vector3(25, 1, 25));
            bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, 0));
            bgObj.AddComponent(new TextureChanger());
            bgObj.AddComponent(new AudioSourceComponent());

            World.Add(planeObj);
            World.Add(c);
            World.Add(bgObj);
            World.SetCamera(c);
            World.Add(uiContainer);
            Renderer.ClearColor = new Color((int)(0.2f * 255), (int)(0.3f * 255), (int)(255 * 0.3f), 255);

        }
    }
}