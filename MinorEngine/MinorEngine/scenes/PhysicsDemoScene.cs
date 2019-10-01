using GameEngine.components;
using GameEngine.components.fldemo;
using GameEngine.engine.core;
using GameEngine.engine.rendering;
using GameEngine.engine.ui.utils;

namespace GameEngine.scenes
{

    using System.Collections.Generic;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    namespace GameEngine.scenes
    {
        public class PhysicsDemoScene : AbstractScene
        {


            private string cmd_ReLoadScene(string[] args)
            {
                SceneRunner.Instance.InitializeScene<PhysicsDemoScene>();
                return "Reloaded";
            }

            private string cmd_NextScene(string[] args)
            {
                SceneRunner.Instance.InitializeScene<FLDemoScene>();
                return "Loading FL Demo Scene";
            }

            private string cmd_ChangeCameraPos(string[] args)
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
                World.Camera.Translate(pos);
                pos = World.Camera.GetLocalPosition();
                return "New Position: " + pos.X + ":" + pos.Z + ":" + pos.Y;
            }

            private string cmd_ChangeCameraRot(string[] args)
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
                World.Camera.Rotate(pos, MathHelper.DegreesToRadians(angle));

                return "Rotating " + angle + " degrees on Axis: " + pos.X + ":" + pos.Z + ":" + pos.Y;
            }

            public override void Destroy()
            {

            }

            protected override void InitializeScene()
            {
                


                GameModel bgBox = new GameModel("models/cube_flat.obj");

                GameTexture bg = TextureProvider.Load("textures/ground4k.png");
                bgBox.Meshes[0].Textures = new[] { bg };


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

                PhysicsDemoComponent phys = new PhysicsDemoComponent();

                World.AddComponent(phys); //Adding Physics Component to world.

                DebugConsoleComponent dbg = DebugConsoleComponent.CreateConsole().GetComponent<DebugConsoleComponent>();
                dbg.AddCommand("mov", cmd_ChangeCameraPos);
                dbg.AddCommand("rot", cmd_ChangeCameraRot);
                dbg.AddCommand("reload", cmd_ReLoadScene);
                dbg.AddCommand("next", cmd_NextScene);
                World.Add(dbg.Owner);

                GameObject bgObj = new GameObject(Vector3.UnitY * -3, "BG");
                bgObj.Scale(new Vector3(25, 1, 25));
                bgObj.AddComponent(new MeshRendererComponent(shader, bgBox, 1));
                World.Add(bgObj);

                Camera c = new Camera(Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f), SceneRunner.Instance.Width / (float)SceneRunner.Instance.Height, 0.01f, 1000f), Vector3.Zero);
                c.Rotate(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(-25));
                c.Translate(new Vector3(0, 10, 10));
                World.Add(c);
                World.SetCamera(c);


            }


        }
    }
}