using System;
using System.Collections.Generic;
using GameEngine.components;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
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

            GameModel sphere = new GameModel("models/sphere_smooth.obj");
            GameModel plane = new GameModel("models/plane.obj");

            GameTexture runic = GameTexture.Load("textures/runicfloor.png");
            plane.meshes[0].Textures = new[] { runic };
            sphere.meshes[0].Textures = new[] { runic };

            Matrix4.CreatePerspectiveFieldOfView(ToRadians(60), 4f / 3f, 0.1f, 1000f, out Matrix4 projection);

            bool ret = ShaderProgram.TryCreate(new Dictionary<ShaderType, string>()
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"},
            }, out ShaderProgram shader);

            Camera c = new Camera(projection, Vector3.Zero);
            c.Rotate(Vector3.UnitX, ToRadians(-40));
            c.Translate(new Vector3(0, 6, 7));

            GameObject planeObj = new GameObject(Vector3.Zero, "Plane");
            planeObj.Scale(new Vector3(5, 5, 5));
            planeObj.Model = plane;
            planeObj.AddComponent(new TextureChanger(window));
            planeObj.Shader = shader;

            GameObject sphereObj = new GameObject(Vector3.Zero, "Sphere");
            sphereObj.Scale(new Vector3(2.5f, 2.5f, 2.5f));
            sphereObj.AddComponent(new RotatingComponent());

            sphereObj.Model = sphere;
            sphereObj.Shader = shader;


            world.Add(planeObj);
            world.Add(sphereObj);
            world.Add(c);
            world.SetCamera(c);

            renderer.ClearColor = new Color((int)(0.2f * 255), (int)(0.3f * 255), (int)(255 * 0.3f), 255);

        }
    }
}