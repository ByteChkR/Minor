
using System;
using System.Collections.Generic;
using GameEngine.components;
using GameEngine.components.fldemo;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using GameEngine.engine.ui;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.scenes
{
    public class FLDemoScene : AbstractGame
    {

        public FLDemoScene(EngineSettings settings) : base(settings)
        {

        }

        protected override void Update(object sender, FrameEventArgs e)
        {
            base.Update(sender, e);
        }



        protected override void initializeScene()
        {
            GameTexture runic = GameTexture.Load("textures/runicfloor.png");
            GameModel sphere = new GameModel("models/sphere_smooth.obj");
            GameModel plane = new GameModel("models/plane.obj");


            sphere.Meshes[0].Textures = new[] { runic };
            plane.Meshes[0].Textures = new[] { runic };


            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/UITextRender.fs"},
                {ShaderType.VertexShader, "shader/UITextRender.vs"},
            }, out ShaderProgram textShader);

            ShaderProgram.TryCreate(new Dictionary<ShaderType, string>
            {
                {ShaderType.FragmentShader, "shader/texture.fs"},
                {ShaderType.VertexShader, "shader/texture.vs"},
            }, out ShaderProgram shader);

            GameObject objSphere = new GameObject(new Vector3(2, 2, 0), "SphereDisplay");
            objSphere.AddComponent(new MeshRendererComponent(shader, sphere, 0));

            GameObject objQuad = new GameObject(new Vector3(-2, 2, 0), "QuadDisplay");
            objQuad.AddComponent(new MeshRendererComponent(shader, plane, 0));

            GameObject uiText = new GameObject(new Vector3(0), "UIText");
            UITextRendererComponent tr = new UITextRendererComponent("Arial", textShader)
            {
                Position = new Vector2(-0.5f, 0.43f),
                Scale = new Vector2(2f, 2f)
            };
            uiText.AddComponent(tr);
            uiText.AddComponent(new FLGeneratorConsoleComponent());
            uiText.AddComponent(new FLGeneratorComponent(new List<MeshRendererComponent>
                {objSphere.GetComponent<MeshRendererComponent>(), objQuad.GetComponent<MeshRendererComponent>()}, runic));

            World.Add(objSphere);
            World.Add(objQuad);
            World.Add(uiText);

            Camera c = new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(95f), 4 / 3f, 0.01f, 1000f), new Vector3(0, 5, 8));
            c.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-35f));
            World.Add(c);
            World.SetCamera(c);


        }


    }
}