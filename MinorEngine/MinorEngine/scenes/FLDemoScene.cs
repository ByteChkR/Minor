
using System.Collections.Generic;
using GameEngine.components;
using GameEngine.components.fldemo;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using GameEngine.engine.ui.utils;
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


        private static string cmd_ChangeCameraPos(string[] args)
        {
            if (args.Length != 3)
            {
                return "Invalid Arguments";
            }

            float x, y, z;
            if (!float.TryParse(args[0], out x))
            {
                return "Invalid Arguments";

            }
            if (!float.TryParse(args[1], out y))
            {

                return "Invalid Arguments";
            }
            if (!float.TryParse(args[2], out z))
            {

                return "Invalid Arguments";
            }

            Vector3 pos = new Vector3(x, y, z);
            AbstractGame.Instance.World.Camera.Translate(pos);
            pos = AbstractGame.Instance.World.Camera.GetLocalPosition();
            return "New Position: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }

        private static string cmd_ChangeCameraRot(string[] args)
        {
            if (args.Length != 4)
            {
                return "Invalid Arguments";
            }
            float x, y, z, angle;
            if (!float.TryParse(args[0], out x))
            {
                return "Invalid Arguments";

            }
            if (!float.TryParse(args[1], out y))
            {

                return "Invalid Arguments";
            }
            if (!float.TryParse(args[2], out z))
            {

                return "Invalid Arguments";
            }
            if (!float.TryParse(args[3], out angle))
            {

                return "Invalid Arguments";
            }

            Vector3 pos = new Vector3(x, y, z);
            AbstractGame.Instance.World.Camera.Rotate(pos, MathHelper.DegreesToRadians(angle));

            return "Rotating " + angle + " degrees on Axis: " + pos.X + ":" + pos.Z + ":" + pos.Y;
        }


        protected override void initializeScene()
        {
            GameTexture runic = TextureProvider.Load("textures/runicfloor.png");
            GameModel sphere = new GameModel("models/sphere_smooth.obj");
            GameModel plane = new GameModel("models/plane.obj");



            GameModel bgBox = new GameModel("models/cube_flat.obj");
            GameObject tt = new GameObject("Test");

            GameTexture bg = TextureProvider.Load("textures/ground4k.png");
            bgBox.Meshes[0].Textures = new[] { bg };
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
            tt.AddComponent(new MeshRendererComponent(shader, new GameModel("models/cube_flat.obj"), 0));

            GameObject objSphere = new GameObject(new Vector3(1, 1, 0), "SphereDisplay");
            objSphere.AddComponent(new MeshRendererComponent(shader, sphere, 0));
            objSphere.AddComponent(new RotatingComponent());

            GameObject objQuad = new GameObject(new Vector3(-1, 1, 0), "QuadDisplay");
            objQuad.AddComponent(new MeshRendererComponent(shader, plane, 0));
            objQuad.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(90));

            GameObject uiText = new GameObject(new Vector3(0), "UIText");
            uiText.AddComponent(new FLGeneratorComponent(new List<MeshRendererComponent>
                {objSphere.GetComponent<MeshRendererComponent>(), objQuad.GetComponent<MeshRendererComponent>()}, runic));

            PhysicsDemoComponent phys = new PhysicsDemoComponent();

            World.AddComponent(phys); //Adding Physics Component to world.
            

            World.Add(uiText);
            DebugConsoleComponent dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
            dbg.AddCommand("mov", cmd_ChangeCameraPos);
            dbg.AddCommand("rot", cmd_ChangeCameraRot);
            World.Add(dbg.Owner);
            World.Add(objSphere);
            World.Add(objQuad);

            GameObject bgObj = new GameObject(Vector3.UnitY*-3, "BG");
            bgObj.Scale(new Vector3(25, 1, 25));
            bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, 0));
            World.Add(bgObj);

            Camera c = new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f), 4 / 3f, 0.01f, 1000f), Vector3.Zero);
            c.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-25));
            c.Translate(new Vector3(0, 2, 2));
            World.Add(c);
            World.SetCamera(c);


        }


    }
}