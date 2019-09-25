using System;
using System.Collections.Generic;
using GameEngine.engine.ui;
using MinorEngine.components;
using MinorEngine.engine.audio;
using MinorEngine.engine.audio.sources;
using MinorEngine.engine.core;
using MinorEngine.engine.rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
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

        protected override void initializeScene()
        {
            base.initializeScene();

            AudioManager.Initialize();

            GameModel sphere = new GameModel("models/sphere_smooth.obj");
            GameModel plane = new GameModel("models/plane.obj");

            GameTexture runic = GameTexture.Load("textures/runicfloor.png");
            GameTexture face = GameTexture.Load("textures/TEST.png");
            plane.Meshes[0].Textures = new[] { runic };
            sphere.Meshes[0].Textures = new[] { runic };

            Matrix4.CreatePerspectiveFieldOfView(ToRadians(60), 4f / 3f, 0.1f, 1000f, out Matrix4 projection);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"},
            }, out ShaderProgram shader);

            UIRendererComponent uiElement = new UIRendererComponent(face, null)
            {
                Position = new Vector2(0.2f, 0.2f),
                Scale = new Vector2(0.2f, 0.2f)
            };

            GameObject uiContainer = new GameObject("UIContainer");
            uiContainer.AddComponent(uiElement);
            uiContainer.AddComponent(new UIMovingComponent());
            Camera c = new Camera(projection, Vector3.Zero);
            c.AddComponent(new AudioListener());
            c.Rotate(Vector3.UnitX, ToRadians(-40));
            c.Translate(new Vector3(0, 6, 7));

            GameObject planeObj = new GameObject(Vector3.UnitZ*2, "Plane");
            planeObj.Scale(new Vector3(5, 5, 5));
            planeObj.AddComponent(new MeshRendererComponent(shader, plane, 0));
            planeObj.AddComponent(new TextureChanger(Window));
            planeObj.AddComponent(new RotateAroundComponent());
            planeObj.AddComponent(new AudioSourceComponent());

            GameObject sphereObj = new GameObject(Vector3.Zero, "Sphere");
            sphereObj.Scale(new Vector3(2.5f, 2.5f, 2.5f));
            sphereObj.AddComponent(new RotatingComponent());
            sphereObj.AddComponent(new MeshRendererComponent(shader, sphere, 0));



            World.Add(planeObj);
            World.Add(sphereObj);
            World.Add(c);
            World.SetCamera(c);
            World.Add(uiContainer);

            Renderer.ClearColor = new Color((int)(0.2f * 255), (int)(0.3f * 255), (int)(255 * 0.3f), 255);

        }
    }
}